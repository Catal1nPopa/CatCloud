
using System.IO.Compression;

namespace Helper.FileCompression
{
    public class FileCompression
    {
        public static void CompressFile(string inputFilePath, string outputFilePath)
        {
            using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open))
            using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
            using (GZipStream compressionStream = new GZipStream(outputFileStream, CompressionLevel.Optimal))
            {
                inputFileStream.CopyTo(compressionStream);
            }
        }

        public static void DecompressFile(string inputFilePath, string outputFilePath)
        {
            using (FileStream inputFileStream = new FileStream(inputFilePath, FileMode.Open))
            using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
            using (GZipStream decompressionStream = new GZipStream(inputFileStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(outputFileStream);
            }
        }

    }
}
