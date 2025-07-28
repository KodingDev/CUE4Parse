cmake_minimum_required(VERSION 3.10)
project(oodle2 CXX)

set(CMAKE_CXX_STANDARD 11)

# Only enable AVX2 for x86/x86_64 architectures
if(CMAKE_SYSTEM_PROCESSOR MATCHES "(x86)|(X86)|(amd64)|(AMD64)")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -mavx2")
endif()

if(CMAKE_SYSTEM_PROCESSOR MATCHES "arm64|aarch64")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -march=armv8-a")
endif()

include_directories(${PROJECT_SOURCE_DIR}/external/oodle2/core)
include_directories(${PROJECT_SOURCE_DIR}/external/oodle2/base)
include_directories(${PROJECT_SOURCE_DIR}/external/oodle2/core/public)

file(GLOB SOURCES ${PROJECT_SOURCE_DIR}/external/oodle2/core/*.cpp)
add_library(${PROJECT_NAME} STATIC ${SOURCES})

add_compile_definitions(OODLE_BUILDING_DLL)

# set_target_properties(${PROJECT_NAME} PROPERTIES 
#     POSITION_INDEPENDENT_CODE ON
#     VISIBILITY_INLINES_HIDDEN ON
#     C_VISIBILITY_PRESET default
#     CXX_VISIBILITY_PRESET default
# )
