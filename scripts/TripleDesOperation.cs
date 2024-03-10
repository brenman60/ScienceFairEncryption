using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionDecryptionUsingSymmetricKey
{
    public class TripleDesOperation
    {
        private static readonly string key = "b14ca5898a4e4133b14ca589"; // 24-byte key for Triple DES

        public static string Key
        {
            get { return key; }
        }

        public async static Task<string> EncryptString(string plainText)
        {
            byte[] iv = new byte[8];
            byte[] array;

            using (TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider())
            {
                tripleDes.Key = Encoding.UTF8.GetBytes(key);
                tripleDes.IV = iv;

                ICryptoTransform encryptor = tripleDes.CreateEncryptor(tripleDes.Key, tripleDes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            await streamWriter.WriteAsync(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static async Task<string> DecryptString(string cipherText)
        {
            byte[] iv = new byte[8];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (TripleDESCryptoServiceProvider tripleDes = new TripleDESCryptoServiceProvider())
            {
                tripleDes.Key = Encoding.UTF8.GetBytes(key);
                tripleDes.IV = iv;

                ICryptoTransform decryptor = tripleDes.CreateDecryptor(tripleDes.Key, tripleDes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return await streamReader.ReadToEndAsync();
                        }
                    }
                }
            }
        }
    }
}
