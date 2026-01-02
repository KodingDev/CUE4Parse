#pragma once

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

// Decode audio data from memory to WAV format
// Returns 0 on success, negative on error
// outputData will be allocated and must be freed with VgmStream_FreeBuffer
#if defined _WIN32 || defined __CYGWIN__
  #ifdef WIN_EXPORT
    #ifdef __GNUC__
      __attribute__ ((dllexport)) int32_t VgmStream_DecodeToWav(
    #else
      __declspec(dllexport) int32_t VgmStream_DecodeToWav(
    #endif
  #else
    __declspec(dllimport) int32_t VgmStream_DecodeToWav(
  #endif
#else
  #if __GNUC__ >= 4
    __attribute__ ((visibility ("default"))) int32_t VgmStream_DecodeToWav(
  #else
    int32_t VgmStream_DecodeToWav(
  #endif
#endif
    const uint8_t* inputData,
    int32_t inputSize,
    uint8_t** outputData,
    int32_t* outputSize,
    const char* filename
);

// Free buffer allocated by VgmStream_DecodeToWav
#if defined _WIN32 || defined __CYGWIN__
  #ifdef WIN_EXPORT
    #ifdef __GNUC__
      __attribute__ ((dllexport)) void VgmStream_FreeBuffer(uint8_t* buffer);
    #else
      __declspec(dllexport) void VgmStream_FreeBuffer(uint8_t* buffer);
    #endif
  #else
    __declspec(dllimport) void VgmStream_FreeBuffer(uint8_t* buffer);
  #endif
#else
  #if __GNUC__ >= 4
    __attribute__ ((visibility ("default"))) void VgmStream_FreeBuffer(uint8_t* buffer);
  #else
    void VgmStream_FreeBuffer(uint8_t* buffer);
  #endif
#endif

#ifdef __cplusplus
}
#endif
