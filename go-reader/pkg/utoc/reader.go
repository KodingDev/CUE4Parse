package utoc

import (
	"bytes"
	"encoding/binary"
	"fmt"
	"io"
	"os"
	"path/filepath"
	"strings"
)

const (
	UTOCHeaderSize = 144
	AESBlockSize   = 16
)

var TOCMagic = []byte{0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D}

type TocVersion byte

const (
	TocVersionInvalid TocVersion = iota
	TocVersionInitial
	TocVersionDirectoryIndex
	TocVersionPartitionSize
	TocVersionPerfectHash
	TocVersionPerfectHashWithOverflow
	TocVersionOnDemandMetaData
	TocVersionRemovedOnDemandMetaData
	TocVersionReplaceIoChunkHashWithIoHash
)

type ContainerFlags byte

const (
	FlagNone       ContainerFlags = 0
	FlagCompressed ContainerFlags = 1 << 0
	FlagEncrypted  ContainerFlags = 1 << 1
	FlagSigned     ContainerFlags = 1 << 2
	FlagIndexed    ContainerFlags = 1 << 3
	FlagOnDemand   ContainerFlags = 1 << 4
)

// Header represents the UTOC file header (144 bytes)
type Header struct {
	Magic                            [16]byte
	Version                          TocVersion
	TocHeaderSize                    uint32
	TocEntryCount                    uint32
	TocCompressedBlockEntryCount     uint32
	TocCompressedBlockEntrySize      uint32
	CompressionMethodNameCount       uint32
	CompressionMethodNameLength      uint32
	CompressionBlockSize             uint32
	DirectoryIndexSize               uint32
	PartitionCount                   uint32
	ContainerID                      uint64
	EncryptionKeyGUID                [16]byte
	ContainerFlags                   ContainerFlags
	TocChunkPerfectHashSeedsCount    uint32
	PartitionSize                    uint64
	TocChunksWithoutPerfectHashCount uint32
}

// DirectoryEntry represents a directory in the directory index
type DirectoryEntry struct {
	Name             uint32
	FirstChildEntry  uint32
	NextSiblingEntry uint32
	FirstFileEntry   uint32
}

// FileEntry represents a file entry in the directory index
type FileEntry struct {
	Name          uint32
	NextFileEntry uint32
	UserData      uint32
}

// Reader provides seek-based reading of UTOC/UCAS files
type Reader struct {
	utocFile  *os.File
	ucasFiles []*os.File
	header    *Header
	basePath  string
	files     []string
}

// Open opens a UTOC file and its corresponding UCAS files
func Open(utocPath string) (*Reader, error) {
	utocFile, err := os.Open(utocPath)
	if err != nil {
		return nil, fmt.Errorf("failed to open UTOC: %w", err)
	}

	r := &Reader{
		utocFile: utocFile,
		basePath: strings.TrimSuffix(utocPath, filepath.Ext(utocPath)),
	}

	if err := r.readHeader(); err != nil {
		utocFile.Close()
		return nil, fmt.Errorf("failed to read header: %w", err)
	}

	// Open UCAS files
	if err := r.openUCASFiles(); err != nil {
		utocFile.Close()
		return nil, fmt.Errorf("failed to open UCAS files: %w", err)
	}

	// Read directory index if present
	if r.header.ContainerFlags&FlagIndexed != 0 && r.header.DirectoryIndexSize > 0 {
		if err := r.readDirectoryIndex(); err != nil {
			r.Close()
			return nil, fmt.Errorf("failed to read directory index: %w", err)
		}
	}

	return r, nil
}

// Close closes all open files
func (r *Reader) Close() error {
	r.utocFile.Close()
	for _, f := range r.ucasFiles {
		f.Close()
	}
	return nil
}

// GetHeader returns the UTOC header
func (r *Reader) GetHeader() *Header {
	return r.header
}

// ListFiles returns all files in the container
func (r *Reader) ListFiles() []string {
	return r.files
}

// readHeader reads and parses the UTOC header
func (r *Reader) readHeader() error {
	buf := make([]byte, UTOCHeaderSize)
	if _, err := io.ReadFull(r.utocFile, buf); err != nil {
		return fmt.Errorf("failed to read header: %w", err)
	}

	reader := bytes.NewReader(buf)
	h := &Header{}

	// Verify magic
	if _, err := io.ReadFull(reader, h.Magic[:]); err != nil {
		return err
	}
	if !bytes.Equal(h.Magic[:], TOCMagic) {
		return fmt.Errorf("invalid TOC magic")
	}

	// Read version and reserved bytes
	binary.Read(reader, binary.LittleEndian, &h.Version)
	reader.Seek(3, io.SeekCurrent) // Skip reserved bytes

	// Read header fields
	binary.Read(reader, binary.LittleEndian, &h.TocHeaderSize)
	binary.Read(reader, binary.LittleEndian, &h.TocEntryCount)
	binary.Read(reader, binary.LittleEndian, &h.TocCompressedBlockEntryCount)
	binary.Read(reader, binary.LittleEndian, &h.TocCompressedBlockEntrySize)
	binary.Read(reader, binary.LittleEndian, &h.CompressionMethodNameCount)
	binary.Read(reader, binary.LittleEndian, &h.CompressionMethodNameLength)
	binary.Read(reader, binary.LittleEndian, &h.CompressionBlockSize)
	binary.Read(reader, binary.LittleEndian, &h.DirectoryIndexSize)
	binary.Read(reader, binary.LittleEndian, &h.PartitionCount)
	binary.Read(reader, binary.LittleEndian, &h.ContainerID)
	binary.Read(reader, binary.LittleEndian, &h.EncryptionKeyGUID)
	binary.Read(reader, binary.LittleEndian, &h.ContainerFlags)
	reader.Seek(3, io.SeekCurrent) // Skip reserved bytes
	binary.Read(reader, binary.LittleEndian, &h.TocChunkPerfectHashSeedsCount)
	binary.Read(reader, binary.LittleEndian, &h.PartitionSize)
	binary.Read(reader, binary.LittleEndian, &h.TocChunksWithoutPerfectHashCount)

	r.header = h
	return nil
}

// openUCASFiles opens all UCAS partition files
func (r *Reader) openUCASFiles() error {
	// Try to open base UCAS file
	ucasPath := r.basePath + ".ucas"
	ucasFile, err := os.Open(ucasPath)
	if err != nil {
		return fmt.Errorf("failed to open UCAS file: %w", err)
	}
	r.ucasFiles = append(r.ucasFiles, ucasFile)

	// Open additional partitions if they exist
	if r.header.PartitionCount > 1 {
		for i := uint32(1); i < r.header.PartitionCount; i++ {
			partPath := fmt.Sprintf("%s_s%d.ucas", r.basePath, i)
			partFile, err := os.Open(partPath)
			if err != nil {
				// Not all partitions may exist
				break
			}
			r.ucasFiles = append(r.ucasFiles, partFile)
		}
	}

	return nil
}

// readDirectoryIndex reads and parses the directory index
func (r *Reader) readDirectoryIndex() error {
	// Calculate offset to directory index
	offset := int64(UTOCHeaderSize)

	// Skip chunk IDs (12 bytes each)
	offset += int64(r.header.TocEntryCount * 12)

	// Skip offset/length pairs (10 bytes each)
	offset += int64(r.header.TocEntryCount * 10)

	// Skip perfect hash seeds
	if r.header.Version >= TocVersionPerfectHash {
		offset += int64(r.header.TocChunkPerfectHashSeedsCount * 4)
	}

	// Skip chunks without perfect hash
	if r.header.Version >= TocVersionPerfectHashWithOverflow {
		offset += int64(r.header.TocChunksWithoutPerfectHashCount * 4)
	}

	// Skip compression blocks (11 bytes each - 5+3+3)
	offset += int64(r.header.TocCompressedBlockEntryCount * 11)

	// Skip compression method names
	offset += int64(r.header.CompressionMethodNameCount * r.header.CompressionMethodNameLength)

	// Now read directory index
	if _, err := r.utocFile.Seek(offset, io.SeekStart); err != nil {
		return fmt.Errorf("failed to seek to directory index: %w", err)
	}

	indexData := make([]byte, r.header.DirectoryIndexSize)
	if _, err := io.ReadFull(r.utocFile, indexData); err != nil {
		return fmt.Errorf("failed to read directory index: %w", err)
	}

	// Parse directory index
	return r.parseDirectoryIndex(indexData)
}

// parseDirectoryIndex parses the directory index structure
func (r *Reader) parseDirectoryIndex(data []byte) error {
	buf := bytes.NewReader(data)

	// Read mount point
	mountPoint, err := readFString(buf)
	if err != nil {
		return fmt.Errorf("failed to read mount point: %w", err)
	}

	// Read directory entry count
	var dirCount uint32
	if err := binary.Read(buf, binary.LittleEndian, &dirCount); err != nil {
		return fmt.Errorf("failed to read directory count: %w", err)
	}

	// Read directory entries
	dirEntries := make([]DirectoryEntry, dirCount)
	for i := uint32(0); i < dirCount; i++ {
		binary.Read(buf, binary.LittleEndian, &dirEntries[i].Name)
		binary.Read(buf, binary.LittleEndian, &dirEntries[i].FirstChildEntry)
		binary.Read(buf, binary.LittleEndian, &dirEntries[i].NextSiblingEntry)
		binary.Read(buf, binary.LittleEndian, &dirEntries[i].FirstFileEntry)
	}

	// Read file entry count
	var fileCount uint32
	if err := binary.Read(buf, binary.LittleEndian, &fileCount); err != nil {
		return fmt.Errorf("failed to read file count: %w", err)
	}

	// Read file entries
	fileEntries := make([]FileEntry, fileCount)
	for i := uint32(0); i < fileCount; i++ {
		binary.Read(buf, binary.LittleEndian, &fileEntries[i].Name)
		binary.Read(buf, binary.LittleEndian, &fileEntries[i].NextFileEntry)
		binary.Read(buf, binary.LittleEndian, &fileEntries[i].UserData)
	}

	// Read string table count
	var stringCount uint32
	if err := binary.Read(buf, binary.LittleEndian, &stringCount); err != nil {
		return fmt.Errorf("failed to read string count: %w", err)
	}

	// Read string table
	stringTable := make([]string, stringCount)
	for i := uint32(0); i < stringCount; i++ {
		str, err := readFString(buf)
		if err != nil {
			return fmt.Errorf("failed to read string %d: %w", i, err)
		}
		stringTable[i] = str
	}

	// Build file list by traversing directory structure
	r.files = r.buildFileList(mountPoint, dirEntries, fileEntries, stringTable, 0, "")

	return nil
}

// buildFileList recursively builds the file list
func (r *Reader) buildFileList(mountPoint string, dirEntries []DirectoryEntry, fileEntries []FileEntry, stringTable []string, dirIdx uint32, currentPath string) []string {
	var files []string

	if dirIdx >= uint32(len(dirEntries)) {
		return files
	}

	dir := dirEntries[dirIdx]

	// Get directory name
	var dirName string
	if dir.Name != 0xFFFFFFFF && dir.Name < uint32(len(stringTable)) {
		dirName = stringTable[dir.Name]
	}

	// Update current path
	if dirName != "" {
		if currentPath != "" {
			currentPath = currentPath + "/" + dirName
		} else {
			currentPath = dirName
		}
	}

	// Add files in this directory
	fileIdx := dir.FirstFileEntry
	for fileIdx != 0xFFFFFFFF && fileIdx < uint32(len(fileEntries)) {
		file := fileEntries[fileIdx]
		if file.Name < uint32(len(stringTable)) {
			fileName := stringTable[file.Name]
			fullPath := mountPoint + currentPath
			if fullPath != "" && !strings.HasSuffix(fullPath, "/") {
				fullPath += "/"
			}
			fullPath += fileName
			files = append(files, fullPath)
		}
		fileIdx = file.NextFileEntry
	}

	// Recurse into child directories
	childIdx := dir.FirstChildEntry
	for childIdx != 0xFFFFFFFF && childIdx < uint32(len(dirEntries)) {
		childFiles := r.buildFileList(mountPoint, dirEntries, fileEntries, stringTable, childIdx, currentPath)
		files = append(files, childFiles...)

		// Move to next sibling
		childIdx = dirEntries[childIdx].NextSiblingEntry
	}

	return files
}

// readFString reads an Unreal Engine FString
func readFString(r io.Reader) (string, error) {
	var length int32
	if err := binary.Read(r, binary.LittleEndian, &length); err != nil {
		return "", err
	}

	if length == 0 {
		return "", nil
	}

	if length < 0 {
		// Unicode string
		length = -length
		buf := make([]uint16, length)
		if err := binary.Read(r, binary.LittleEndian, &buf); err != nil {
			return "", err
		}
		runes := make([]rune, 0, length-1)
		for i := 0; i < len(buf)-1; i++ {
			runes = append(runes, rune(buf[i]))
		}
		return string(runes), nil
	}

	// ASCII string
	buf := make([]byte, length)
	if _, err := io.ReadFull(r, buf); err != nil {
		return "", err
	}
	if len(buf) > 0 && buf[len(buf)-1] == 0 {
		buf = buf[:len(buf)-1]
	}
	return string(buf), nil
}
