using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        //daca fisierul va fi private/ only me atunci se poate de adaugat  id la user in fisierul criptat
        public async Task EncryptFileAsync(IFormFile inputFile, string outputFilePath, Guid userId)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.GenerateIV();

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

            Console.WriteLine("Fișierul a fost criptat și salvat la: " + outputFilePath);
        }

        public async Task<byte[]> DecryptFileAsync(string encryptedFilePath)
        {
            using (FileStream inputFileStream = new FileStream(encryptedFilePath, FileMode.Open))
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = _key;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    // Citim IV-ul din fișierul criptat
                    byte[] iv = new byte[aesAlg.BlockSize / 8];
                    await inputFileStream.ReadAsync(iv, 0, iv.Length);
                    aesAlg.IV = iv;

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
