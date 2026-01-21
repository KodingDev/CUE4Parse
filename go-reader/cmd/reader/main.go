package main

import (
	"encoding/hex"
	"flag"
	"fmt"
	"os"
	"strings"

	"github.com/kodingdev/cue4parse-go/pkg/crypto"
	"github.com/kodingdev/cue4parse-go/pkg/pak"
	"github.com/kodingdev/cue4parse-go/pkg/utoc"
)

func main() {
	// Define flags
	utocPath := flag.String("utoc", "", "Path to UTOC file")
	pakPath := flag.String("pak", "", "Path to PAK file")
	keyHex := flag.String("key", "", "AES key in hex format (32 bytes for AES-256)")
	showInfo := flag.Bool("info", false, "Show container information")

	flag.Usage = func() {
		fmt.Fprintf(os.Stderr, "CUE4Parse Go Reader - Minimal Unreal Engine file reader\n\n")
		fmt.Fprintf(os.Stderr, "Usage:\n")
		fmt.Fprintf(os.Stderr, "  %s [options]\n\n", os.Args[0])
		fmt.Fprintf(os.Stderr, "Options:\n")
		flag.PrintDefaults()
		fmt.Fprintf(os.Stderr, "\nExamples:\n")
		fmt.Fprintf(os.Stderr, "  # List files in UTOC container\n")
		fmt.Fprintf(os.Stderr, "  %s --utoc game.utoc --key <hex-key>\n\n", os.Args[0])
		fmt.Fprintf(os.Stderr, "  # List files in PAK archive\n")
		fmt.Fprintf(os.Stderr, "  %s --pak game.pak --key <hex-key>\n\n", os.Args[0])
		fmt.Fprintf(os.Stderr, "  # Show container info\n")
		fmt.Fprintf(os.Stderr, "  %s --utoc game.utoc --info\n", os.Args[0])
	}

	flag.Parse()

	// Parse AES key if provided
	var aesKey []byte
	if *keyHex != "" {
		var err error
		aesKey, err = hex.DecodeString(strings.ReplaceAll(*keyHex, " ", ""))
		if err != nil {
			fmt.Fprintf(os.Stderr, "Error: Invalid hex key: %v\n", err)
			os.Exit(1)
		}
		if len(aesKey) != 32 {
			fmt.Fprintf(os.Stderr, "Error: AES-256 key must be 32 bytes, got %d\n", len(aesKey))
			os.Exit(1)
		}
	}

	// Process UTOC file
	if *utocPath != "" {
		if err := processUTOC(*utocPath, aesKey, *showInfo); err != nil {
			fmt.Fprintf(os.Stderr, "Error processing UTOC: %v\n", err)
			os.Exit(1)
		}
		return
	}

	// Process PAK file
	if *pakPath != "" {
		if err := processPAK(*pakPath, aesKey, *showInfo); err != nil {
			fmt.Fprintf(os.Stderr, "Error processing PAK: %v\n", err)
			os.Exit(1)
		}
		return
	}

	// No file specified
	flag.Usage()
	os.Exit(1)
}

func processUTOC(path string, aesKey []byte, showInfo bool) error {
	reader, err := utoc.Open(path)
	if err != nil {
		return err
	}
	defer reader.Close()

	header := reader.GetHeader()

	if showInfo {
		fmt.Printf("UTOC File: %s\n", path)
		fmt.Printf("Version: %d\n", header.Version)
		fmt.Printf("Entry Count: %d\n", header.TocEntryCount)
		fmt.Printf("Compression Block Count: %d\n", header.TocCompressedBlockEntryCount)
		fmt.Printf("Partition Count: %d\n", header.PartitionCount)
		fmt.Printf("Directory Index Size: %d bytes\n", header.DirectoryIndexSize)
		fmt.Printf("Flags: ")
		flags := []string{}
		if header.ContainerFlags&utoc.FlagCompressed != 0 {
			flags = append(flags, "Compressed")
		}
		if header.ContainerFlags&utoc.FlagEncrypted != 0 {
			flags = append(flags, "Encrypted")
		}
		if header.ContainerFlags&utoc.FlagSigned != 0 {
			flags = append(flags, "Signed")
		}
		if header.ContainerFlags&utoc.FlagIndexed != 0 {
			flags = append(flags, "Indexed")
		}
		if len(flags) > 0 {
			fmt.Printf("%s\n", strings.Join(flags, ", "))
		} else {
			fmt.Printf("None\n")
		}
		fmt.Printf("Encryption GUID: %x\n", header.EncryptionKeyGUID)
		fmt.Println()
	}

	files := reader.ListFiles()
	fmt.Printf("Total Files: %d\n\n", len(files))

	for _, file := range files {
		fmt.Println(file)

		// If Marvel Rivals, show encrypted byte count
		if aesKey != nil {
			encBytes := crypto.CalculateEncryptedBytesForMarvelRivals(strings.ToLower(file))
			if encBytes > 0 {
				fmt.Printf("  [Marvel Rivals: %d bytes encrypted]\n", encBytes)
			}
		}
	}

	return nil
}

func processPAK(path string, aesKey []byte, showInfo bool) error {
	reader, err := pak.Open(path)
	if err != nil {
		return err
	}
	defer reader.Close()

	info := reader.GetInfo()

	if showInfo {
		fmt.Printf("PAK File: %s\n", path)
		fmt.Printf("Version: %d\n", info.Version)
		fmt.Printf("Index Offset: 0x%X\n", info.IndexOffset)
		fmt.Printf("Index Size: %d bytes\n", info.IndexSize)
		fmt.Printf("Encrypted Index: %v\n", info.EncryptedIndex)
		fmt.Printf("Frozen Index: %v\n", info.IndexIsFrozen)
		fmt.Printf("Encryption GUID: %x\n", info.EncryptionGUID)
		fmt.Printf("Compression Methods: %s\n", strings.Join(info.Compressions, ", "))
		fmt.Println()
	}

	files, err := reader.ListFiles()
	if err != nil {
		return err
	}

	fmt.Printf("Total Files: %d\n\n", len(files))

	for _, file := range files {
		fmt.Println(file)

		// If Marvel Rivals, show encrypted byte count
		if aesKey != nil {
			encBytes := crypto.CalculateEncryptedBytesForMarvelRivals(strings.ToLower(file))
			if encBytes > 0 {
				fmt.Printf("  [Marvel Rivals: %d bytes encrypted]\n", encBytes)
			}
		}
	}

	return nil
}
