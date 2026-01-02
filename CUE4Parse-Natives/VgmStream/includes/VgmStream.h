#pragma once

#include <stdint.h>

// Decode audio data from memory to WAV format
// Returns 0 on success, negative on error
// outputData will be allocated and must be freed with VgmStream_FreeBuffer
extern "C" int32_t VgmStream_DecodeToWav(
    const uint8_t* inputData,
    int32_t inputSize,
    uint8_t** outputData,
    int32_t* outputSize,
    const char* filename
);

// Free buffer allocated by VgmStream_DecodeToWav
extern "C" void VgmStream_FreeBuffer(uint8_t* buffer);

// Get vgmstream version string
extern "C" const char* VgmStream_GetVersion();
