cmake_minimum_required(VERSION 3.10)
project(vgmstream C)

set(CMAKE_C_STANDARD 99)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

# vgmstream source directory
set(VGMSTREAM_SRC ${PROJECT_SOURCE_DIR}/external/vgmstream/src)

# Include directories
include_directories(${VGMSTREAM_SRC})
include_directories(${VGMSTREAM_SRC}/base)
include_directories(${VGMSTREAM_SRC}/coding)
include_directories(${VGMSTREAM_SRC}/layout)
include_directories(${VGMSTREAM_SRC}/meta)
include_directories(${VGMSTREAM_SRC}/util)
include_directories(${PROJECT_SOURCE_DIR}/external/vgmstream/ext_includes)

# Collect source files
file(GLOB BASE_SOURCES "${VGMSTREAM_SRC}/base/*.c")
file(GLOB CODING_SOURCES "${VGMSTREAM_SRC}/coding/*.c")
file(GLOB CODING_LIBS_SOURCES "${VGMSTREAM_SRC}/coding/libs/*.c")
file(GLOB LAYOUT_SOURCES "${VGMSTREAM_SRC}/layout/*.c")
file(GLOB META_SOURCES "${VGMSTREAM_SRC}/meta/*.c")
file(GLOB UTIL_SOURCES "${VGMSTREAM_SRC}/util/*.c")
file(GLOB MAIN_SOURCES
    "${VGMSTREAM_SRC}/formats.c"
    "${VGMSTREAM_SRC}/streamfile.c"
    "${VGMSTREAM_SRC}/util.c"
    "${VGMSTREAM_SRC}/vgmstream.c"
    "${VGMSTREAM_SRC}/vgmstream_init.c"
)

# Create library
add_library(libvgmstream STATIC
    ${BASE_SOURCES}
    ${CODING_SOURCES}
    ${CODING_LIBS_SOURCES}
    ${LAYOUT_SOURCES}
    ${META_SOURCES}
    ${UTIL_SOURCES}
    ${MAIN_SOURCES}
)

set_target_properties(libvgmstream PROPERTIES PREFIX "")

# Platform-specific definitions
if(WIN32)
    target_compile_definitions(libvgmstream PRIVATE
        _WIN32_WINNT=0x501
        _CRT_SECURE_NO_WARNINGS
    )
    # Suppress common MSVC warnings in vgmstream code
    if(MSVC)
        target_compile_options(libvgmstream PRIVATE
            /wd4267  # conversion from 'size_t' to 'type', possible loss of data
            /wd4244  # conversion from 'type1' to 'type2', possible loss of data
            /wd4018  # signed/unsigned mismatch
            /wd4101  # unreferenced local variable
            /wd4334  # 32-bit shift implicitly converted to 64 bits
            /wd4311  # pointer truncation
        )
    endif()
endif()

# Enable vgmstream features based on available libraries
target_compile_definitions(libvgmstream PUBLIC VGM_LOG_OUTPUT)

# Set up platform-specific library paths
if (APPLE)
    if (CMAKE_SYSTEM_PROCESSOR MATCHES "arm64|aarch64")
        set(BREW_PREFIX /opt/homebrew)
    else()
        set(BREW_PREFIX /usr/local)
    endif()
    set(PKG_CONFIG_PATH "${BREW_PREFIX}/lib/pkgconfig")
endif()

# Find optional dependencies
if(NOT WIN32)
    # Try using pkg-config first
    find_package(PkgConfig QUIET)

    if(PKG_CONFIG_FOUND AND APPLE)
        set(ENV{PKG_CONFIG_PATH} "${PKG_CONFIG_PATH}:$ENV{PKG_CONFIG_PATH}")
    endif()

    if(PKG_CONFIG_FOUND)
        # MPEG support
        pkg_check_modules(MPG123 QUIET libmpg123)
        if(MPG123_FOUND)
            target_compile_definitions(libvgmstream PUBLIC VGM_USE_MPEG)
            target_include_directories(libvgmstream PRIVATE ${MPG123_INCLUDE_DIRS})
        endif()

        # Vorbis support
        pkg_check_modules(VORBIS QUIET vorbisfile vorbis ogg)
        if(VORBIS_FOUND)
            target_compile_definitions(libvgmstream PUBLIC VGM_USE_VORBIS)
            target_include_directories(libvgmstream PRIVATE ${VORBIS_INCLUDE_DIRS})
        endif()

        # FFmpeg support
        pkg_check_modules(FFMPEG QUIET libavcodec libavformat libavutil libswresample)
        if(FFMPEG_FOUND)
            target_compile_definitions(libvgmstream PUBLIC VGM_USE_FFMPEG)
            target_include_directories(libvgmstream PRIVATE ${FFMPEG_INCLUDE_DIRS})
        endif()

        # Speex support
        pkg_check_modules(SPEEX QUIET speex)
        if(SPEEX_FOUND)
            target_compile_definitions(libvgmstream PUBLIC VGM_USE_SPEEX)
            target_include_directories(libvgmstream PRIVATE ${SPEEX_INCLUDE_DIRS})
        endif()
    endif()
endif()

# Built-in G.722.1 support (always enabled)
target_compile_definitions(libvgmstream PUBLIC VGM_USE_G7221)
