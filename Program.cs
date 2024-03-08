using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EncryptionDecryptionUsingSymmetricKey;

namespace ScienceFairEncryption
{
    internal class Program
    {
        static Dictionary<string, long> files = new Dictionary<string, long>()
        {
            ["encryptionAES1.txt"] = 1000, // one kb
            ["encryptionAES2.txt"] = 1000000, // one mb
            ["encryptionAES3.txt"] = 1000000000, // one gb 
        };

        static void print(object text) => Console.WriteLine(text);

        static TimeSpan getStopwatchTime(Stopwatch stopWatch)
        {
            stopWatch.Stop();
            TimeSpan time = stopWatch.Elapsed;
            stopWatch.Reset();
            stopWatch.Start();

            return time;
        }

        static string getFilePath(string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("files", fileName));
        }

        private static void createTextFile(string fileName, long length)
        {
            const int blockSize = 1024 * 8;
            const int blocksPerMb = (1024 * 1024) / blockSize;
            byte[] data = new byte[blockSize];
            Random rng = new Random();
            using (FileStream stream = File.OpenWrite(getFilePath(fileName)))
            {
                for (int i = 0; i < (length / 1000000) * blocksPerMb; i++)
                {
                    rng.NextBytes(data);
                    stream.Write(data, 0, data.Length);
                }
            }
        }
        
        static async void encryptFileContent(string fileName)
        {
            using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                using (StreamReader reader = new StreamReader(fileStream))
                    AesOperation.EncryptString(await reader.ReadToEndAsync());
        }

        static void Main(string[] args)
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "files")))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "files"));

            foreach (KeyValuePair<string, long> file in files)
                createTextFile(file.Key, file.Value);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            print("AES: Encryption");

            encryptFileContent("encryptionAES1.txt");
            print("File 1: One KB: " + getStopwatchTime(stopWatch).ToString("ss':'fff"));

            encryptFileContent("encryptionAES2.txt");
            print("File 2: One MB: " + getStopwatchTime(stopWatch).ToString("ss':'fff"));

            encryptFileContent("encryptionAES3.txt");
            print("File 3: One GB: " + getStopwatchTime(stopWatch).ToString("ss':'fff"));

            print("Press any key to exit...");
            Console.ReadLine();
        }
    }
}
