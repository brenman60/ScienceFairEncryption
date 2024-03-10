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

            ["encryptionDES1.txt"] = 1000, // one KB
            ["encryptionDES2.txt"] = 1000000, // one MB
            ["encryptionDES3.txt"] = 1000000 * 10, // ten MB
            ["encryptionDES4.txt"] = 1000000 * 25, // twenty five MB
            ["encryptionDES5.txt"] = 1000000 * 50, // fifty MB

            ["encryption3DES1.txt"] = 1000, // one KB
            ["encryption3DES2.txt"] = 1000000, // one MB
            ["encryption3DES3.txt"] = 1000000 * 10, // ten MB
            ["encryption3DES4.txt"] = 1000000 * 25, // twenty five MB
            ["encryption3DES5.txt"] = 1000000 * 50, // fifty MB

            ["encryptionBlowfish1.txt"] = 1000, // one KB
            ["encryptionBlowfish2.txt"] = 1000000, // one MB
            ["encryptionBlowfish3.txt"] = 1000000 * 10, // ten MB
            ["encryptionBlowfish4.txt"] = 1000000 * 25, // twenty five MB
            ["encryptionBlowfish5.txt"] = 1000000 * 50, // fifty MB
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
                case EncryptionMode.TripleDES:
                    using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                    using (StreamReader reader = new StreamReader(fileStream))
                        return await TripleDesOperation.EncryptString(await reader.ReadToEndAsync());
                case EncryptionMode.Blowfish:
                    using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                        using (StreamReader reader = new StreamReader(fileStream))
                            return await BlowfishOperation.EncryptString(await reader.ReadToEndAsync());
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
                case EncryptionMode.TripleDES:
                    using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                    using (StreamReader reader = new StreamReader(fileStream))
                        return await TripleDesOperation.DecryptString(await reader.ReadToEndAsync());
                case EncryptionMode.Blowfish:
                    using (FileStream fileStream = new FileStream(getFilePath(fileName), FileMode.Open))
                        using (StreamReader reader = new StreamReader(fileStream))
                            return await BlowfishOperation.DecryptString(await reader.ReadToEndAsync());
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
                "encryptionDES1.txt",
                "encryptionDES2.txt",
                "encryptionDES3.txt",
                "encryptionDES4.txt",
                "encryptionDES5.txt",
            };

            List<string> encryptionSet3 = new List<string>()
            {
                "encryption3DES1.txt",
                "encryption3DES2.txt",
                "encryption3DES3.txt",
                "encryption3DES4.txt",
                "encryption3DES5.txt",
            };

            List<string> encryptionSet4 = new List<string>()
            {
                "encryptionBlowfish1.txt",
                "encryptionBlowfish2.txt",
                "encryptionBlowfish3.txt",
                "encryptionBlowfish4.txt",
                "encryptionBlowfish5.txt",
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
            getStopwatchTime(stopWatch);

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
            getStopwatchTime(stopWatch);

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
            getStopwatchTime(stopWatch);

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

            await Task.Delay(10000);
            getStopwatchTime(stopWatch);

            print("Triple DES: Encryption");
            foreach (string item in encryptionSet3)
            {
                string encryptedText = await encryptFileContent(item, EncryptionMode.Blowfish);
                using (FileStream stream = new FileStream(getFilePath(item), FileMode.OpenOrCreate))
                using (StreamWriter writer = new StreamWriter(stream))
                    await writer.WriteAsync(encryptedText);

                double CPUusage = cpu.NextValue();
                double RAMusage = ram.NextValue();
                print("File encryption: " + item + " - " + getStopwatchTime(stopWatch).ToString("ss':'fff") + " - " + CPUusage + " - " + RAMusage);
            }
            print("\n");

            await Task.Delay(10000);
            getStopwatchTime(stopWatch);

            print("Triple DES: Decryption");
            foreach (string item in encryptionSet3)
            {
                string decrypted = await decryptFileContent(item, EncryptionMode.Blowfish);
                using (FileStream stream = new FileStream(getFilePath(item), FileMode.OpenOrCreate))
                using (StreamWriter writer = new StreamWriter(stream))
                    await writer.WriteAsync(decrypted);

                double CPUusage = cpu.NextValue();
                double RAMusage = ram.NextValue();
                print("File decryption: " + item + " - " + getStopwatchTime(stopWatch).ToString("ss':'fff") + " - " + CPUusage + " - " + RAMusage);
            }
            print("\n");

            await Task.Delay(10000);
            getStopwatchTime(stopWatch);

            print("Blowfish: Encryption");
            foreach (string item in encryptionSet4)
            {
                string encryptedText = await encryptFileContent(item, EncryptionMode.Blowfish);
                using (FileStream stream = new FileStream(getFilePath(item), FileMode.OpenOrCreate))
                using (StreamWriter writer = new StreamWriter(stream))
                    await writer.WriteAsync(encryptedText);

                double CPUusage = cpu.NextValue();
                double RAMusage = ram.NextValue();
                print("File encryption: " + item + " - " + getStopwatchTime(stopWatch).ToString("ss':'fff") + " - " + CPUusage + " - " + RAMusage);
            }
            print("\n");

            await Task.Delay(10000);
            getStopwatchTime(stopWatch);

            print("Blowfish: Decryption");
            foreach (string item in encryptionSet4)
            {
                string decrypted = await decryptFileContent(item, EncryptionMode.Blowfish);
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
        TripleDES,
        Blowfish,
    }
}