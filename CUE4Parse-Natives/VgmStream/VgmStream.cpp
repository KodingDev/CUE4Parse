#include "../common/Framework.h"
#include "includes/VgmStream.h"

extern "C" {
#include "external/vgmstream/src/libvgmstream.h"
#include "external/vgmstream/src/libvgmstream_streamfile.h"
}

#include <stdlib.h>
#include <string.h>
#include <stdio.h>

// Suppress strncpy warning on MSVC
#ifdef _MSC_VER
#pragma warning(disable: 4996)
#endif

// Memory streamfile implementation
typedef struct {
    const uint8_t* data;
    int64_t size;
    int64_t offset;
    char filename[256];
} memory_streamfile_data;

static int memory_read(void* user_data, uint8_t* dst, int64_t offset, int length) {
    memory_streamfile_data* mem = (memory_streamfile_data*)user_data;

    if (offset < 0 || offset > mem->size)
        return 0;

    int64_t bytes_to_read = length;
    if (offset + bytes_to_read > mem->size)
        bytes_to_read = mem->size - offset;

    if (bytes_to_read <= 0)
        return 0;

    memcpy(dst, mem->data + offset, bytes_to_read);
    return (int)bytes_to_read;
}

static int64_t memory_get_size(void* user_data) {
    memory_streamfile_data* mem = (memory_streamfile_data*)user_data;
    return mem->size;
}

static const char* memory_get_name(void* user_data) {
    memory_streamfile_data* mem = (memory_streamfile_data*)user_data;
    return mem->filename;
}

static libstreamfile_t* memory_open(void* user_data, const char* filename) {
    memory_streamfile_data* mem = (memory_streamfile_data*)user_data;

    // Only support reopening the same file
    if (strcmp(filename, mem->filename) != 0)
        return NULL;

    // Create a new streamfile pointing to the same data
    libstreamfile_t* new_sf = (libstreamfile_t*)malloc(sizeof(libstreamfile_t));
    if (!new_sf)
        return NULL;

    memory_streamfile_data* new_mem = (memory_streamfile_data*)malloc(sizeof(memory_streamfile_data));
    if (!new_mem) {
        free(new_sf);
        return NULL;
    }

    memcpy(new_mem, mem, sizeof(memory_streamfile_data));
    new_mem->offset = 0;

    new_sf->user_data = new_mem;
    new_sf->read = memory_read;
    new_sf->get_size = memory_get_size;
    new_sf->get_name = memory_get_name;
    new_sf->open = memory_open;
    new_sf->close = [](libstreamfile_t* libsf) {
        if (libsf && libsf->user_data) {
            free(libsf->user_data);
        }
        free(libsf);
    };

    return new_sf;
}

static libstreamfile_t* create_memory_streamfile(const uint8_t* data, int64_t size, const char* filename) {
    libstreamfile_t* sf = (libstreamfile_t*)malloc(sizeof(libstreamfile_t));
    if (!sf)
        return NULL;

    memory_streamfile_data* mem = (memory_streamfile_data*)malloc(sizeof(memory_streamfile_data));
    if (!mem) {
        free(sf);
        return NULL;
    }

    mem->data = data;
    mem->size = size;
    mem->offset = 0;
    strncpy(mem->filename, filename, sizeof(mem->filename) - 1);
    mem->filename[sizeof(mem->filename) - 1] = '\0';

    sf->user_data = mem;
    sf->read = memory_read;
    sf->get_size = memory_get_size;
    sf->get_name = memory_get_name;
    sf->open = memory_open;
    sf->close = [](libstreamfile_t* libsf) {
        if (libsf && libsf->user_data) {
            free(libsf->user_data);
        }
        free(libsf);
    };

    return sf;
}

// Write WAV header
static void write_wav_header(uint8_t* buffer, int32_t sample_count, int32_t channels, int32_t sample_rate) {
    int32_t data_size = sample_count * channels * sizeof(int16_t);
    int32_t file_size = data_size + 36;

    // RIFF header
    memcpy(buffer + 0, "RIFF", 4);
    *(int32_t*)(buffer + 4) = file_size;
    memcpy(buffer + 8, "WAVE", 4);

    // fmt chunk
    memcpy(buffer + 12, "fmt ", 4);
    *(int32_t*)(buffer + 16) = 16; // fmt chunk size
    *(int16_t*)(buffer + 20) = 1;  // PCM format
    *(int16_t*)(buffer + 22) = (int16_t)channels;
    *(int32_t*)(buffer + 24) = sample_rate;
    *(int32_t*)(buffer + 28) = sample_rate * channels * sizeof(int16_t); // byte rate
    *(int16_t*)(buffer + 32) = (int16_t)(channels * sizeof(int16_t)); // block align
    *(int16_t*)(buffer + 34) = 16; // bits per sample

    // data chunk
    memcpy(buffer + 36, "data", 4);
    *(int32_t*)(buffer + 40) = data_size;
}

DLLEXPORT int32_t VgmStream_DecodeToWav(
    const uint8_t* inputData,
    int32_t inputSize,
    uint8_t** outputData,
    int32_t* outputSize,
    const char* filename
) {
    if (!inputData || inputSize <= 0 || !outputData || !outputSize)
        return -1;

    // Use a default filename if none provided
    const char* actual_filename = filename ? filename : "audio.wem";

    // Create memory streamfile
    libstreamfile_t* sf = create_memory_streamfile(inputData, inputSize, actual_filename);
    if (!sf)
        return -2;

    // Initialize vgmstream
    libvgmstream_t* lib = libvgmstream_init();
    if (!lib) {
        libstreamfile_close(sf);
        return -3;
    }

    // Configure vgmstream
    libvgmstream_config_t cfg = {};
    cfg.ignore_loop = true;
    cfg.force_sfmt = LIBVGMSTREAM_SFMT_PCM16;
    libvgmstream_setup(lib, &cfg);

    // Open stream
    int err = libvgmstream_open_stream(lib, sf, 0);
    libstreamfile_close(sf);

    if (err < 0) {
        libvgmstream_free(lib);
        return -4;
    }

    // Get format info
    int32_t channels = lib->format->channels;
    int32_t sample_rate = lib->format->sample_rate;
    int32_t play_samples = (int32_t)lib->format->play_samples;

    if (channels <= 0 || sample_rate <= 0 || play_samples <= 0) {
        libvgmstream_free(lib);
        return -5;
    }

    // Calculate output size (WAV header + PCM data)
    int32_t pcm_size = play_samples * channels * sizeof(int16_t);
    int32_t total_size = 44 + pcm_size;

    // Allocate output buffer
    uint8_t* buffer = (uint8_t*)malloc(total_size);
    if (!buffer) {
        libvgmstream_free(lib);
        return -6;
    }

    // Write WAV header
    write_wav_header(buffer, play_samples, channels, sample_rate);

    // Decode audio
    int16_t* pcm_buffer = (int16_t*)(buffer + 44);

    // Decode all samples at once using libvgmstream_fill
    err = libvgmstream_fill(lib, pcm_buffer, play_samples);
    if (err < 0) {
        free(buffer);
        libvgmstream_free(lib);
        return -8;
    }

    libvgmstream_free(lib);

    *outputData = buffer;
    *outputSize = total_size;

    return 0;
}

DLLEXPORT void VgmStream_FreeBuffer(uint8_t* buffer) {
    if (buffer)
        free(buffer);
}
