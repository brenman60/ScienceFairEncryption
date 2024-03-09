using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EncryptionDecryptionUsingSymmetricKey;

namespace ScienceFairEncryption
{
    internal class Program
    {
        static Dictionary<string, long> files = new Dictionary<string, long>()
        {
            ["encryptionAES1.txt"] = 1000, // one KB
            ["encryptionAES2.txt"] = 1000000, // one MB
            ["encryptionAES3.txt"] = 1000000 * 10, // ten MB
            ["encryptionAES4.txt"] = 1000000 * 25, // twenty five MB
            ["encryptionAES5.txt"] = 1000000 * 50, // fifty MB

            ["encryptionAES6.txt"] = 1000, // one KB
            ["encryptionAES7.txt"] = 1000000, // one MB
            ["encryptionAES8.txt"] = 1000000 * 10, // ten MB
            ["encryptionAES9.txt"] = 1000000 * 25, // twenty five MB
            ["encryptionAES10.txt"] = 1000000 * 50, // fifty MB
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
            char[] data = new char[blockSize];
            Random rng = new Random();

            if (File.Exists(getFilePath(fileName)))
                File.Delete(getFilePath(fileName));

            using (FileStream stream = File.OpenWrite(getFilePath(fileName)))
            {
                for (int i = 0; i < (length / 1000000) * blocksPerMb; i++)
                {
                    for (int j = 0; j < blockSize; j++)
                    {
                        data[j] = (char)('A' + rng.Next(26));
                    }

                    string blockContent = new string(data);
                    byte[] bytes = Encoding.UTF8.GetBytes(blockContent);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }
        
        static async Task<string> encryptFileContent(string fileName, EncryptionMode mode)
        {
            switch (mode) 
            {
                case EncryptionMode.AES:
                    using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                        using (StreamReader reader = new StreamReader(fileStream))
                            return await AesOperation.EncryptString(await reader.ReadToEndAsync());
                case EncryptionMode.DES:
                    using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                        using (StreamReader reader = new StreamReader(fileStream))
                            return await DesOperation.EncryptString(await reader.ReadToEndAsync());
                default:
                    return null;
            }
        }

        static async Task<string> decryptFileContent(string fileName, EncryptionMode mode)
        {
            switch (mode)
            {
                case EncryptionMode.AES:
                    using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                    using (StreamReader reader = new StreamReader(fileStream))
                        return await AesOperation.DecryptString(await reader.ReadToEndAsync());
                case EncryptionMode.DES:
                    using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                    using (StreamReader reader = new StreamReader(fileStream))
                        return await DesOperation.DecryptString(await reader.ReadToEndAsync());
                default:
                    return null;
            }
        }

        static async Task Main(string[] args)
        {
            Process process = Process.GetCurrentProcess();
            PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            PerformanceCounter ram = new PerformanceCounter("Memory", "Committed Bytes", null);

            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "files")))
                Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "files"));

            foreach (KeyValuePair<string, long> file in files)
                createTextFile(file.Key, file.Value);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            /*
             
            * make async (will probably yield more accurate results)
             
             */


            /////////////////////// AES Encryption ////////////////////////////////////////////////////////////////
            List<string> encryptionSet1 = new List<string>()
            {
                "encryptionAES1.txt",
                "encryptionAES2.txt",
                "encryptionAES3.txt",
                "encryptionAES4.txt",
                "encryptionAES5.txt",
            };

            List<string> encryptionSet2 = new List<string>()
            {
                "encryptionAES6.txt",
                "encryptionAES7.txt",
                "encryptionAES8.txt",
                "encryptionAES9.txt",
                "encryptionAES10.txt",
            };

            print("AES: Encryption");
            foreach (string item in encryptionSet1)
            {
                string encryptedText = await encryptFileContent(item, EncryptionMode.AES);
                using (FileStream stream = new FileStream(getFilePath(item), FileMode.OpenOrCreate))
                    using (StreamWriter writer = new StreamWriter(stream))
                        await writer.WriteAsync(encryptedText);

                double CPUusage = cpu.NextValue();
                double RAMusage = ram.NextValue();
                print("File encryption: " + item + " - " + getStopwatchTime(stopWatch).ToString("ss':'fff") + " - " + CPUusage + " - " + RAMusage);
            }
            print("\n");

            await Task.Delay(10000);

            print("AES: Decryption");
            foreach (string item in encryptionSet1)
            {
                string decrypted = await decryptFileContent(item, EncryptionMode.AES);
                using (FileStream stream = new FileStream(getFilePath(item), FileMode.OpenOrCreate))
                    using (StreamWriter writer = new StreamWriter(stream))
                        await writer.WriteAsync(decrypted);

                double CPUusage = cpu.NextValue();
                double RAMusage = ram.NextValue();
                print("File decryption: " + item + " - " + getStopwatchTime(stopWatch).ToString("ss':'fff") + " - " + CPUusage + " - " + RAMusage);
            }
            print("\n");

            await Task.Delay(10000);

            print("DES: Encryption");
            foreach (string item in encryptionSet2)
            {
                string encryptedText = await encryptFileContent(item, EncryptionMode.DES);
                using (FileStream stream = new FileStream(getFilePath(item), FileMode.OpenOrCreate))
                using (StreamWriter writer = new StreamWriter(stream))
                    await writer.WriteAsync(encryptedText);

                double CPUusage = cpu.NextValue();
                double RAMusage = ram.NextValue();
                print("File encryption: " + item + " - " + getStopwatchTime(stopWatch).ToString("ss':'fff") + " - " + CPUusage + " - " + RAMusage);
            }
            print("\n");

            await Task.Delay(10000);

            print("DES: Decryption");
            foreach (string item in encryptionSet2)
            {
                string decrypted = await decryptFileContent(item, EncryptionMode.DES);
                using (FileStream stream = new FileStream(getFilePath(item), FileMode.OpenOrCreate))
                using (StreamWriter writer = new StreamWriter(stream))
                    await writer.WriteAsync(decrypted);

                double CPUusage = cpu.NextValue();
                double RAMusage = ram.NextValue();
                print("File decryption: " + item + " - " + getStopwatchTime(stopWatch).ToString("ss':'fff") + " - " + CPUusage + " - " + RAMusage);
            }
            print("\n");

            print("Press enter to exit...");
            Console.ReadLine();
        }
    }

    enum EncryptionMode
    {
        AES,
        DES,
    }
}