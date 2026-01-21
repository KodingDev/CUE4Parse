.PHONY: build test clean run-utoc run-pak

# Build the reader binary
build:
	go build -o reader ./cmd/reader

# Build for Linux (AWS Lambda)
build-lambda:
	GOOS=linux GOARCH=amd64 go build -o reader-lambda ./cmd/reader

# Build for Windows
build-windows:
	GOOS=windows GOARCH=amd64 go build -o reader.exe ./cmd/reader

# Build for macOS
build-darwin:
	GOOS=darwin GOARCH=amd64 go build -o reader-darwin ./cmd/reader

# Run tests
test:
	go test -v ./...

# Clean build artifacts
clean:
	rm -f reader reader-lambda reader.exe reader-darwin

# Example: List files in UTOC (requires KEY and UTOC environment variables)
run-utoc:
	@if [ -z "$(UTOC)" ]; then echo "Error: UTOC variable not set"; exit 1; fi
	@if [ -z "$(KEY)" ]; then echo "Warning: KEY variable not set, continuing without encryption support"; fi
	./reader --utoc $(UTOC) $(if $(KEY),--key $(KEY),) $(if $(INFO),--info,)

# Example: List files in PAK (requires KEY and PAK environment variables)
run-pak:
	@if [ -z "$(PAK)" ]; then echo "Error: PAK variable not set"; exit 1; fi
	@if [ -z "$(KEY)" ]; then echo "Warning: KEY variable not set, continuing without encryption support"; fi
	./reader --pak $(PAK) $(if $(KEY),--key $(KEY),) $(if $(INFO),--info,)

# Install dependencies
deps:
	go mod download
	go mod tidy

# Format code
fmt:
	go fmt ./...

# Run linter
lint:
	golangci-lint run

# Build all platforms
build-all: build-linux build-windows build-darwin

.DEFAULT_GOAL := build
