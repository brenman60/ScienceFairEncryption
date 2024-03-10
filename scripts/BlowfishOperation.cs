using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace EncryptionDecryptionUsingSymmetricKey
{
    public class BlowfishOperation
    {
        private static readonly string key = "b14ca5898a4e4133";

        public static string Key
        {
            get { return key; }
        }

        public async static Task<string> EncryptString(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            CipherKeyGenerator generator = GeneratorUtilities.GetKeyGenerator("Blowfish");
            generator.Init(new KeyGenerationParameters(new SecureRandom(), 128));

            IBufferedCipher cipher = CipherUtilities.GetCipher("Blowfish/ECB/PKCS7Padding");
            cipher.Init(true, new KeyParameter(keyBytes));

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] outputBytes = cipher.DoFinal(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        public async static Task<string> DecryptString(string cipherText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            CipherKeyGenerator generator = GeneratorUtilities.GetKeyGenerator("Blowfish");
            generator.Init(new KeyGenerationParameters(new SecureRandom(), 128));

            IBufferedCipher cipher = CipherUtilities.GetCipher("Blowfish/ECB/PKCS7Padding");
            cipher.Init(false, new KeyParameter(keyBytes));

            byte[] inputBytes = Convert.FromBase64String(cipherText);
            byte[] outputBytes = cipher.DoFinal(inputBytes);

            return Encoding.UTF8.GetString(outputBytes);
        }
    }
}
