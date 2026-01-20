using System.IO;
using System.Runtime.CompilerServices;
using CUE4Parse.Compression;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Readers;
using CUE4Parse.UE4.Versions;

namespace CUE4Parse.FileProvider.Objects
{
    public class OsGameFile : VersionedGameFile
    {
        public readonly FileInfo ActualFile;

        public OsGameFile(DirectoryInfo baseDir, FileInfo info, string mountPoint, VersionContainer versions)
            : base(System.IO.Path.GetRelativePath(baseDir.FullName, info.FullName).Replace('\\', '/'), info.Length, versions)
        {
            ActualFile = info;
        }

        public override bool IsEncrypted => false;
        public override CompressionMethod CompressionMethod => CompressionMethod.None;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte[] Read(FByteBulkDataHeader? header = null) => File.ReadAllBytes(ActualFile.FullName);

        // Override CreateReader to use streaming instead of loading entire file into memory
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override FArchive CreateReader(FByteBulkDataHeader? header = null)
        {
            // Use buffered FileStream for better performance on sequential reads
            var stream = new BufferedStream(
                ActualFile.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                81920 // 80KB buffer
            );
            return new FStreamArchive(Path, stream, Versions);
        }
    }
}
