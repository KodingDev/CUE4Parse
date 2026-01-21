package pak

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"io"
	"os"
	"strings"
)

const (
	PAKFileMagic       = 0x5A6F12E1
	AESAlign           = 16
	MaxHeaderSize      = 300 // Largest header variation
	SHA1HashSize       = 20
	GUIDSize           = 16
	CompressionNameLen = 32
)

type PakVersion uint32

const (
	PakFileVersionInitial PakVersion = iota + 1
	PakFileVersionNoTimestamps
	PakFileVersionCompressionEncryption
	PakFileVersionIndexEncryption
	PakFileVersionRelativeChunkOffsets
	PakFileVersionDeleteRecords
	PakFileVersionEncryptionKeyGuid
	PakFileVersionFNameBasedCompressionMethod
	PakFileVersionFrozenIndex
	PakFileVersionPathHashIndex
	PakFileVersionFnv64BugFix
	PakFileVersionUtf8PakDirectory
)

// PakInfo contains PAK file header metadata
type PakInfo struct {
	Magic          uint32
	Version        PakVersion
	IndexOffset    int64
	IndexSize      int64
	IndexHash      [SHA1HashSize]byte
	EncryptedIndex bool
	IndexIsFrozen  bool
	EncryptionGUID [GUIDSize]byte
	Compressions   []string
}

// PakEntry represents a single file entry in the PAK
type PakEntry struct {
	Path             string
	Offset           int64
	CompressedSize   int64
	UncompressedSize int64
	CompressionIndex uint32
	Flags            uint32
	StructSize       int32
	IsEncrypted      bool
	IsCompressed     bool
}

// Reader provides seek-based reading of PAK files
type Reader struct {
	file *os.File
	info *PakInfo
	size int64
}

// Open opens a PAK file for reading
func Open(path string) (*Reader, error) {
	file, err := os.Open(path)
	if err != nil {
		return nil, fmt.Errorf("failed to open PAK: %w", err)
	}

	stat, err := file.Stat()
	if err != nil {
		file.Close()
		return nil, fmt.Errorf("failed to stat PAK: %w", err)
	}

	r := &Reader{
		file: file,
		size: stat.Size(),
	}

	if err := r.readInfo(); err != nil {
		file.Close()
		return nil, fmt.Errorf("failed to read PAK info: %w", err)
	}

	return r, nil
}

// Close closes the PAK file
func (r *Reader) Close() error {
	return r.file.Close()
}

// GetInfo returns the PAK header information
func (r *Reader) GetInfo() *PakInfo {
	return r.info
}

// ListFiles lists all files in the PAK (reads and parses index)
func (r *Reader) ListFiles() ([]string, error) {
	// Seek to index
	if _, err := r.file.Seek(r.info.IndexOffset, io.SeekStart); err != nil {
		return nil, fmt.Errorf("failed to seek to index: %w", err)
	}

	// Read index data
	indexData := make([]byte, r.info.IndexSize)
	if _, err := io.ReadFull(r.file, indexData); err != nil {
		return nil, fmt.Errorf("failed to read index: %w", err)
	}

	// Parse based on version
	if r.info.Version >= PakFileVersionPathHashIndex {
		return r.readIndexUpdated(indexData)
	}
	return r.readIndexLegacy(indexData)
}

// readInfo reads the PAK file info from the end of the file
func (r *Reader) readInfo() error {
	// Try reading from different offsets
	offsets := []int64{205, 204, 200, 196, 160, 153, 128, 61, 53, 45}

	for _, offset := range offsets {
		if offset > r.size {
			continue
		}

		_, err := r.file.Seek(-offset, io.SeekEnd)
		if err != nil {
			continue
		}

		buf := make([]byte, offset)
		if _, err := io.ReadFull(r.file, buf); err != nil {
			continue
		}

		info, err := r.parseInfo(buf)
		if err == nil && info.Magic == PAKFileMagic {
			r.info = info
			return nil
		}
	}

	return fmt.Errorf("failed to find valid PAK header")
}

// parseInfo parses PAK info from buffer
func (r *Reader) parseInfo(data []byte) (*PakInfo, error) {
	buf := bytes.NewReader(data)
	info := &PakInfo{}

	// Standard PAK format (simplified, assumes standard layout)
	// Read in reverse order as stored in file

	// Read GUID (16 bytes)
	if err := binary.Read(buf, binary.LittleEndian, &info.EncryptionGUID); err != nil {
		return nil, err
	}

	// Read encrypted index flag
	var encFlag byte
	if err := binary.Read(buf, binary.LittleEndian, &encFlag); err != nil {
		return nil, err
	}
	info.EncryptedIndex = encFlag != 0

	// Read magic
	if err := binary.Read(buf, binary.LittleEndian, &info.Magic); err != nil {
		return nil, err
	}

	if info.Magic != PAKFileMagic {
		return nil, fmt.Errorf("invalid magic: 0x%X", info.Magic)
	}

	// Read version
	var ver uint32
	if err := binary.Read(buf, binary.LittleEndian, &ver); err != nil {
		return nil, err
	}
	info.Version = PakVersion(ver)

	// Read index offset and size
	if err := binary.Read(buf, binary.LittleEndian, &info.IndexOffset); err != nil {
		return nil, err
	}
	if err := binary.Read(buf, binary.LittleEndian, &info.IndexSize); err != nil {
		return nil, err
	}

	// Read index hash
	if err := binary.Read(buf, binary.LittleEndian, &info.IndexHash); err != nil {
		return nil, err
	}

	// Read frozen index flag if version supports it
	if info.Version == PakFileVersionFrozenIndex {
		var frozenFlag byte
		if err := binary.Read(buf, binary.LittleEndian, &frozenFlag); err == nil {
			info.IndexIsFrozen = frozenFlag != 0
		}
	}

	// Read compression methods if version supports it
	if info.Version >= PakFileVersionFNameBasedCompressionMethod {
		info.Compressions = r.readCompressionMethods(buf, 4)
	} else {
		info.Compressions = []string{"None", "Zlib", "Gzip", "Oodle", "LZ4", "Zstd"}
	}

	return info, nil
}

func (r *Reader) readCompressionMethods(buf *bytes.Reader, maxCount int) []string {
	methods := []string{"None"}
	for i := 0; i < maxCount; i++ {
		nameBytes := make([]byte, CompressionNameLen)
		if _, err := io.ReadFull(buf, nameBytes); err != nil {
			break
		}
		name := strings.TrimRight(string(nameBytes), "\x00")
		if name != "" {
			methods = append(methods, name)
		}
	}
	return methods
}

// readIndexLegacy reads legacy format index (versions < 10)
func (r *Reader) readIndexLegacy(data []byte) ([]string, error) {
	buf := bytes.NewReader(data)
	var files []string

	// Read mount point
	mountPoint, err := readFString(buf)
	if err != nil {
		return nil, fmt.Errorf("failed to read mount point: %w", err)
	}

	// Read file count
	var fileCount uint32
	if err := binary.Read(buf, binary.LittleEndian, &fileCount); err != nil {
		return nil, fmt.Errorf("failed to read file count: %w", err)
	}

	// Read each file entry
	for i := uint32(0); i < fileCount; i++ {
		filename, err := readFString(buf)
		if err != nil {
			return nil, fmt.Errorf("failed to read filename %d: %w", i, err)
		}

		// Skip entry data (we only need filenames for listing)
		// Entry structure varies by version, but we can skip it
		var offset, size, uncompressedSize int64
		binary.Read(buf, binary.LittleEndian, &offset)
		binary.Read(buf, binary.LittleEndian, &size)
		binary.Read(buf, binary.LittleEndian, &uncompressedSize)

		// Skip rest of entry (flags, timestamps, etc.)
		buf.Seek(32, io.SeekCurrent)

		files = append(files, mountPoint+filename)
	}

	return files, nil
}

// readIndexUpdated reads updated format index (versions >= 10)
func (r *Reader) readIndexUpdated(data []byte) ([]string, error) {
	// Simplified implementation - this format is complex
	// For a minimal reader, we'd need to implement:
	// 1. Primary index reading
	// 2. Encoded entries parsing
	// 3. Directory index parsing
	// This is a placeholder that returns an error
	return nil, fmt.Errorf("updated index format not yet implemented - use legacy PAK files")
}

// readFString reads an Unreal Engine FString (length-prefixed)
func readFString(r io.Reader) (string, error) {
	var length int32
	if err := binary.Read(r, binary.LittleEndian, &length); err != nil {
		return "", err
	}

	if length == 0 {
		return "", nil
	}

	if length < 0 {
		// Unicode string (UTF-16)
		length = -length
		buf := make([]uint16, length)
		if err := binary.Read(r, binary.LittleEndian, &buf); err != nil {
			return "", err
		}
		runes := make([]rune, 0, length-1)
		for i := 0; i < len(buf)-1; i++ { // Skip null terminator
			runes = append(runes, rune(buf[i]))
		}
		return string(runes), nil
	}

	// ASCII string
	buf := make([]byte, length)
	if _, err := io.ReadFull(r, buf); err != nil {
		return "", err
	}
	// Remove null terminator
	if len(buf) > 0 && buf[len(buf)-1] == 0 {
		buf = buf[:len(buf)-1]
	}
	return string(buf), nil
}
