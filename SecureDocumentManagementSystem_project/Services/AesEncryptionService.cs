using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecureDocumentManagementSystem.Services
{
    public class AesEncryptionService
    {
        // مفتاح 256 بت (32 حرف بالضبط)
        private readonly byte[] _key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");

        public byte[] Encrypt(byte[] data)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.GenerateIV();

                using (var ms = new MemoryStream())
                {
                    // نكتب IV أولاً في الملف
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                    }

                    return ms.ToArray(); // الملف المشفر + IV
                }
            }
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;

                using (var ms = new MemoryStream(encryptedData))
                {
                    byte[] iv = new byte[16];
                    ms.Read(iv, 0, 16); // أول 16 بايت هي الـ IV
                    aes.IV = iv;

                    using (var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var output = new MemoryStream())
                    {
                        cryptoStream.CopyTo(output);
                        return output.ToArray();
                    }
                }
            }
        }
    }
}
