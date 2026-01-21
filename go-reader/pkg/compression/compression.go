package compression

import (
	"bytes"
	"compress/zlib"
	"fmt"
	"io"
)

// Method represents a compression method
type Method int

const (
	None Method = iota
	Zlib
	Gzip
	Oodle
	LZ4
	Zstd
)

// Decompress decompresses data using the specified method
func Decompress(compressed []byte, method Method) ([]byte, error) {
	switch method {
	case None:
		return compressed, nil

	case Zlib:
		reader, err := zlib.NewReader(bytes.NewReader(compressed))
		if err != nil {
			return nil, fmt.Errorf("zlib decompress error: %w", err)
		}
		defer reader.Close()
		return io.ReadAll(reader)

	case Oodle:
		// Oodle requires native library bindings via CGO
		// For AWS Lambda deployment, you would need to:
		// 1. Include liblinoodle.so in your Lambda layer
		// 2. Use CGO to call native Oodle decompression
		return nil, fmt.Errorf("oodle decompression requires native library (see README)")

	case LZ4:
		// LZ4 decompression
		return nil, fmt.Errorf("LZ4 decompression not implemented yet")

	case Zstd:
		// Zstd decompression
		return nil, fmt.Errorf("Zstd decompression not implemented yet")

	default:
		return nil, fmt.Errorf("unknown compression method: %d", method)
	}
}

// MethodFromString converts a compression method name to Method
func MethodFromString(name string) Method {
	switch name {
	case "None":
		return None
	case "Zlib":
		return Zlib
	case "Gzip":
		return Gzip
	case "Oodle":
		return Oodle
	case "LZ4":
		return LZ4
	case "Zstd":
		return Zstd
	default:
		return None
	}
}

/*
NOTE: For Oodle decompression with CGO, you would add:

#cgo CFLAGS: -I./oodle
#cgo LDFLAGS: -L./oodle -llinoodle
#include <stdlib.h>
#include "oodle.h"

import "C"
import "unsafe"

func DecompressOodle(compressed []byte, uncompressedSize int) ([]byte, error) {
	output := make([]byte, uncompressedSize)

	result := C.OodleLZ_Decompress(
		(*C.uchar)(unsafe.Pointer(&compressed[0])),
		C.longlong(len(compressed)),
		(*C.uchar)(unsafe.Pointer(&output[0])),
		C.longlong(uncompressedSize),
		0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
	)

	if result <= 0 {
		return nil, fmt.Errorf("oodle decompression failed")
	}

	return output, nil
}
*/
