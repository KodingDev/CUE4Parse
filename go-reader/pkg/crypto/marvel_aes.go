package crypto

import (
	"encoding/binary"
	"fmt"

	"github.com/zeebo/blake3"
)

const (
	AESKeyBits    = 256
	AESBlockBytes = 16
	RKLength      = AESKeyBits/8 + 28 // 60 for AES-256
)

// MarvelAESDecryptor implements Marvel Rivals' custom AES-256 ECB decryption
// Based on Rijndael reference implementation
type MarvelAESDecryptor struct {
	key []byte
}

// NewMarvelAESDecryptor creates a new decryptor with the provided 32-byte key
func NewMarvelAESDecryptor(key []byte) (*MarvelAESDecryptor, error) {
	if len(key) != 32 {
		return nil, fmt.Errorf("key must be 32 bytes for AES-256, got %d", len(key))
	}
	return &MarvelAESDecryptor{key: key}, nil
}

// Decrypt decrypts data using Marvel Rivals' AES-256 ECB
func (d *MarvelAESDecryptor) Decrypt(ciphertext []byte) ([]byte, error) {
	if len(ciphertext)%AESBlockBytes != 0 {
		return nil, fmt.Errorf("ciphertext length must be multiple of %d", AESBlockBytes)
	}

	plaintext := make([]byte, len(ciphertext))
	rk := make([]uint32, RKLength)
	nrounds := rijndaelSetupDecrypt(rk, d.key)

	for offset := 0; offset < len(ciphertext); offset += AESBlockBytes {
		rijndaelDecrypt(rk, nrounds, ciphertext[offset:offset+AESBlockBytes], plaintext[offset:offset+AESBlockBytes])
	}

	return plaintext, nil
}

// CalculateEncryptedBytesForMarvelRivals calculates how many bytes of a file are encrypted
// using Blake3 hash of the asset path
func CalculateEncryptedBytesForMarvelRivals(assetPath string) int {
	hasher := blake3.New()

	// Initial seed: 0x44332211
	seed := make([]byte, 4)
	binary.LittleEndian.PutUint32(seed, 0x44332211)
	hasher.Write(seed)

	// Hash lowercase asset path
	hasher.Write([]byte(assetPath))

	// Get first 8 bytes as uint64
	hash := hasher.Sum(nil)
	firstU64 := binary.LittleEndian.Uint64(hash[:8])

	// Formula: (63 * (firstU64 % 0x3D) + 319) & 0xFFFFFFFFFFFFFFC0
	final := (63*(firstU64%0x3D) + 319) & 0xFFFFFFFFFFFFFFC0

	return int(final)
}

// Rijndael decryption implementation
func rijndaelSetupEncrypt(rk []uint32, key []byte) int {
	rk[0] = getu32(key[0:])
	rk[1] = getu32(key[4:])
	rk[2] = getu32(key[8:])
	rk[3] = getu32(key[12:])

	rk[4] = getu32(key[16:])
	rk[5] = getu32(key[20:])
	rk[6] = getu32(key[24:])
	rk[7] = getu32(key[28:])

	i := 0
	for {
		temp := rk[7]
		rk[8] = rk[0] ^
			(te4[(temp>>16)&0xff]&0xff000000) ^
			(te4[(temp>>8)&0xff]&0x00ff0000) ^
			(te4[(temp>>0)&0xff]&0x0000ff00) ^
			(te4[(temp>>24)&0xff]&0x000000ff) ^
			rcon[i]
		rk[9] = rk[1] ^ rk[8]
		rk[10] = rk[2] ^ rk[9]
		rk[11] = rk[3] ^ rk[10]
		if i++; i == 7 {
			return 14
		}
		temp = rk[11]
		rk[12] = rk[4] ^
			(te4[(temp>>24)&0xff]&0xff000000) ^
			(te4[(temp>>16)&0xff]&0x00ff0000) ^
			(te4[(temp>>8)&0xff]&0x0000ff00) ^
			(te4[(temp>>0)&0xff]&0x000000ff)
		rk[13] = rk[5] ^ rk[12]
		rk[14] = rk[6] ^ rk[13]
		rk[15] = rk[7] ^ rk[14]
		rk = rk[8:]
	}
}

func rijndaelSetupDecrypt(rk []uint32, key []byte) int {
	nrounds := rijndaelSetupEncrypt(rk, key)

	// Invert the order of round keys
	for i, j := 0, 4*nrounds; i < j; i, j = i+4, j-4 {
		rk[i+0], rk[j+0] = rk[j+0], rk[i+0]
		rk[i+1], rk[j+1] = rk[j+1], rk[i+1]
		rk[i+2], rk[j+2] = rk[j+2], rk[i+2]
		rk[i+3], rk[j+3] = rk[j+3], rk[i+3]
	}

	// Apply inverse MixColumn transform
	for i := 1; i < nrounds; i++ {
		idx := i * 4
		rk[idx+0] =
			td0[te4[(rk[idx+0]>>24)&0xff]&0xff] ^
				td1[te4[(rk[idx+0]>>16)&0xff]&0xff] ^
				td2[te4[(rk[idx+0]>>8)&0xff]&0xff] ^
				td3[te4[(rk[idx+0]>>0)&0xff]&0xff]
		rk[idx+1] =
			td0[te4[(rk[idx+1]>>24)&0xff]&0xff] ^
				td1[te4[(rk[idx+1]>>16)&0xff]&0xff] ^
				td2[te4[(rk[idx+1]>>8)&0xff]&0xff] ^
				td3[te4[(rk[idx+1]>>0)&0xff]&0xff]
		rk[idx+2] =
			td0[te4[(rk[idx+2]>>24)&0xff]&0xff] ^
				td1[te4[(rk[idx+2]>>16)&0xff]&0xff] ^
				td2[te4[(rk[idx+2]>>8)&0xff]&0xff] ^
				td3[te4[(rk[idx+2]>>0)&0xff]&0xff]
		rk[idx+3] =
			td0[te4[(rk[idx+3]>>24)&0xff]&0xff] ^
				td1[te4[(rk[idx+3]>>16)&0xff]&0xff] ^
				td2[te4[(rk[idx+3]>>8)&0xff]&0xff] ^
				td3[te4[(rk[idx+3]>>0)&0xff]&0xff]
	}

	return nrounds
}

func rijndaelDecrypt(rk []uint32, nrounds int, ciphertext, plaintext []byte) {
	s0 := getu32(ciphertext[0:]) ^ rk[0]
	s1 := getu32(ciphertext[4:]) ^ rk[1]
	s2 := getu32(ciphertext[8:]) ^ rk[2]
	s3 := getu32(ciphertext[12:]) ^ rk[3]

	// Round 1
	t0 := td0[s0>>24] ^ td1[(s3>>16)&0xff] ^ td2[(s2>>8)&0xff] ^ td3[s1&0xff] ^ rk[4]
	t1 := td0[s1>>24] ^ td1[(s0>>16)&0xff] ^ td2[(s3>>8)&0xff] ^ td3[s2&0xff] ^ rk[5]
	t2 := td0[s2>>24] ^ td1[(s1>>16)&0xff] ^ td2[(s0>>8)&0xff] ^ td3[s3&0xff] ^ rk[6]
	t3 := td0[s3>>24] ^ td1[(s2>>16)&0xff] ^ td2[(s1>>8)&0xff] ^ td3[s0&0xff] ^ rk[7]

	// Rounds 2-9
	for r := 1; r < 9; r++ {
		idx := r*8 + 0
		s0 = td0[t0>>24] ^ td1[(t3>>16)&0xff] ^ td2[(t2>>8)&0xff] ^ td3[t1&0xff] ^ rk[idx+0]
		s1 = td0[t1>>24] ^ td1[(t0>>16)&0xff] ^ td2[(t3>>8)&0xff] ^ td3[t2&0xff] ^ rk[idx+1]
		s2 = td0[t2>>24] ^ td1[(t1>>16)&0xff] ^ td2[(t0>>8)&0xff] ^ td3[t3&0xff] ^ rk[idx+2]
		s3 = td0[t3>>24] ^ td1[(t2>>16)&0xff] ^ td2[(t1>>8)&0xff] ^ td3[t0&0xff] ^ rk[idx+3]

		t0 = td0[s0>>24] ^ td1[(s3>>16)&0xff] ^ td2[(s2>>8)&0xff] ^ td3[s1&0xff] ^ rk[idx+4]
		t1 = td0[s1>>24] ^ td1[(s0>>16)&0xff] ^ td2[(s3>>8)&0xff] ^ td3[s2&0xff] ^ rk[idx+5]
		t2 = td0[s2>>24] ^ td1[(s1>>16)&0xff] ^ td2[(s0>>8)&0xff] ^ td3[s3&0xff] ^ rk[idx+6]
		t3 = td0[s3>>24] ^ td1[(s2>>16)&0xff] ^ td2[(s1>>8)&0xff] ^ td3[s0&0xff] ^ rk[idx+7]
	}

	// Additional rounds for AES-256
	if nrounds > 10 {
		s0 = td0[t0>>24] ^ td1[(t3>>16)&0xff] ^ td2[(t2>>8)&0xff] ^ td3[t1&0xff] ^ rk[40]
		s1 = td0[t1>>24] ^ td1[(t0>>16)&0xff] ^ td2[(t3>>8)&0xff] ^ td3[t2&0xff] ^ rk[41]
		s2 = td0[t2>>24] ^ td1[(t1>>16)&0xff] ^ td2[(t0>>8)&0xff] ^ td3[t3&0xff] ^ rk[42]
		s3 = td0[t3>>24] ^ td1[(t2>>16)&0xff] ^ td2[(t1>>8)&0xff] ^ td3[t0&0xff] ^ rk[43]

		t0 = td0[s0>>24] ^ td1[(s3>>16)&0xff] ^ td2[(s2>>8)&0xff] ^ td3[s1&0xff] ^ rk[44]
		t1 = td0[s1>>24] ^ td1[(s0>>16)&0xff] ^ td2[(s3>>8)&0xff] ^ td3[s2&0xff] ^ rk[45]
		t2 = td0[s2>>24] ^ td1[(s1>>16)&0xff] ^ td2[(s0>>8)&0xff] ^ td3[s3&0xff] ^ rk[46]
		t3 = td0[s3>>24] ^ td1[(s2>>16)&0xff] ^ td2[(s1>>8)&0xff] ^ td3[s0&0xff] ^ rk[47]

		if nrounds > 12 {
			s0 = td0[t0>>24] ^ td1[(t3>>16)&0xff] ^ td2[(t2>>8)&0xff] ^ td3[t1&0xff] ^ rk[48]
			s1 = td0[t1>>24] ^ td1[(t0>>16)&0xff] ^ td2[(t3>>8)&0xff] ^ td3[t2&0xff] ^ rk[49]
			s2 = td0[t2>>24] ^ td1[(t1>>16)&0xff] ^ td2[(t0>>8)&0xff] ^ td3[t3&0xff] ^ rk[50]
			s3 = td0[t3>>24] ^ td1[(t2>>16)&0xff] ^ td2[(t1>>8)&0xff] ^ td3[t0&0xff] ^ rk[51]

			t0 = td0[s0>>24] ^ td1[(s3>>16)&0xff] ^ td2[(s2>>8)&0xff] ^ td3[s1&0xff] ^ rk[52]
			t1 = td0[s1>>24] ^ td1[(s0>>16)&0xff] ^ td2[(s3>>8)&0xff] ^ td3[s2&0xff] ^ rk[53]
			t2 = td0[s2>>24] ^ td1[(s1>>16)&0xff] ^ td2[(s0>>8)&0xff] ^ td3[s3&0xff] ^ rk[54]
			t3 = td0[s3>>24] ^ td1[(s2>>16)&0xff] ^ td2[(s1>>8)&0xff] ^ td3[s0&0xff] ^ rk[55]
		}
	}

	// Final round
	rkIdx := nrounds * 4
	s0 = (td4[(t0>>24)&0xff]&0xff000000)^
		(td4[(t3>>16)&0xff]&0x00ff0000)^
		(td4[(t2>>8)&0xff]&0x0000ff00)^
		(td4[(t1>>0)&0xff]&0x000000ff)^
		rk[rkIdx+0]
	putu32(plaintext[0:], s0)

	s1 = (td4[(t1>>24)&0xff]&0xff000000)^
		(td4[(t0>>16)&0xff]&0x00ff0000)^
		(td4[(t3>>8)&0xff]&0x0000ff00)^
		(td4[(t2>>0)&0xff]&0x000000ff)^
		rk[rkIdx+1]
	putu32(plaintext[4:], s1)

	s2 = (td4[(t2>>24)&0xff]&0xff000000)^
		(td4[(t1>>16)&0xff]&0x00ff0000)^
		(td4[(t0>>8)&0xff]&0x0000ff00)^
		(td4[(t3>>0)&0xff]&0x000000ff)^
		rk[rkIdx+2]
	putu32(plaintext[8:], s2)

	s3 = (td4[(t3>>24)&0xff]&0xff000000)^
		(td4[(t2>>16)&0xff]&0x00ff0000)^
		(td4[(t1>>8)&0xff]&0x0000ff00)^
		(td4[(t0>>0)&0xff]&0x000000ff)^
		rk[rkIdx+3]
	putu32(plaintext[12:], s3)
}

func getu32(b []byte) uint32 {
	return binary.LittleEndian.Uint32(b)
}

func putu32(b []byte, v uint32) {
	binary.LittleEndian.PutUint32(b, v)
}
