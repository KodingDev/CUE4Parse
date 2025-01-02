using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CUE4Parse_Conversion.Materials;

namespace CUE4Parse_Conversion.Meshes
{
    public class Mesh : ExporterBase
    {
        public readonly string FileName;
        public readonly byte[] FileData;
        public readonly List<MaterialExporter2> Materials;

        public Mesh(string fileName, byte[] fileData, List<MaterialExporter2> materials)
        {
            FileName = fileName;
            FileData = fileData;
            Materials = materials;
        }

        private readonly object _material = new ();
        public override bool TryWriteToDir(DirectoryInfo baseDirectory, out string label, out string savedFilePath)
        {
            label = string.Empty;
            savedFilePath = string.Empty;
            if (!baseDirectory.Exists || FileData.Length <= 0) return false;

            Parallel.ForEach(Materials, material =>
            {
                try
                {
                    lock (_material) material.TryWriteToDir(baseDirectory, out _, out _);
                }
                catch (IOException e)
                {
                    var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
                    if (errorCode == 32 || errorCode == 33)
                    {
                        Console.WriteLine($"Material is already in use, ignoring write to dir: {baseDirectory}");
                    }
                    else
                    {
                        throw;
                    }
                }
            });

            savedFilePath = FixAndCreatePath(baseDirectory, FileName);
            File.WriteAllBytes(savedFilePath, FileData);
            label = Path.GetFileName(savedFilePath);
            return File.Exists(savedFilePath);
        }

        public override bool TryWriteToZip(out byte[] zipFile)
        {
            throw new NotImplementedException();
        }

        public override void AppendToZip()
        {
            throw new NotImplementedException();
        }
    }
}
