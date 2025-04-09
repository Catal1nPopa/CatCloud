using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Helper.Cryptography
{
    public class FileEncryptionService
    {
        private readonly byte[] _key;

        public FileEncryptionService(IConfiguration configuration)
        {
            var encryptionKeyBase64 = configuration["EncryptionSettings:EncryptionKey"];
            _key = Convert.FromBase64String(encryptionKeyBase64);
        }

        private byte[] GenerateIvFromDate(DateTime uploadedAt)
        {
            string ivString = uploadedAt.ToString("yyyyMMddHHmmss");
            return Encoding.UTF8.GetBytes(ivString.PadRight(16, '0'));
        }

        public async Task<long> EncryptFileAsync(IFormFile inputFile, string outputFilePath, Guid userId, DateTime uploadedAt)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.IV = GenerateIvFromDate(uploadedAt);

                using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    await outputFileStream.WriteAsync(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(outputFileStream, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
                    using (Stream inputStream = inputFile.OpenReadStream())
                    {
                        await inputStream.CopyToAsync(cryptoStream);
                    }
                }
            }

            Console.WriteLine($"Fișierul utilizatorului: {userId} a fost criptat și salvat la: " + outputFilePath);
            long fileSizeInBytes = new FileInfo(outputFilePath).Length;
            return fileSizeInBytes;
        }

        public async Task<byte[]> DecryptFileAsync(string encryptedFilePath, DateTime uploadedAt)
        {
            using (FileStream inputFileStream = new FileStream(encryptedFilePath, FileMode.Open))
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _key;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    byte[] iv = new byte[aesAlg.BlockSize / 8];
                    await inputFileStream.ReadAsync(iv, 0, iv.Length);
                    aesAlg.IV = iv;

                    byte[] expectedIv = GenerateIvFromDate(uploadedAt);
                    if (!iv.SequenceEqual(expectedIv))
                    {
                        throw new InvalidOperationException("IV-ul din fișier nu corespunde cu data încărcării.");
                    }

                    using (CryptoStream cryptoStream = new CryptoStream(inputFileStream, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await cryptoStream.CopyToAsync(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
        }
    }
}
