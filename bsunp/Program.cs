/* Author: daemon1 (zenhax)
 * Re-compiled by xyx0826
 * 
 * This branch is designed for quickly extracting sound banks in game assets.
 */

using System;
using System.IO;
using zlib;

namespace bitsquid_unp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Bitsquid Toolchain - bsunp (bitsquid_unp) by daemon1");
            Console.WriteLine("Modified by xyx0826 || !Timpani-only Branch!");
            Console.WriteLine("Usage: bsunp.exe [package-name]");
            if (args.Length == 0) // If no parameters are given
            {
                Console.WriteLine("ERROR: package file not specified.");
                Console.ReadKey();
                return;
            }

            // Read package file
            FileStream packageFile = new FileStream(args[0], FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(packageFile);
            MemoryStream memoryStream = new MemoryStream();
            // Check if stream file exists with package file
            FileStream streamFile = null;
            if (File.Exists(args[0] + ".stream"))
            {
                streamFile = new FileStream(args[0] + ".stream", FileMode.Open);
            }
            else Console.WriteLine("INFO: stream file not found.");
            packageFile.Seek(12L, SeekOrigin.Begin);
            byte[] buffer = new byte[66000];
            while (packageFile.Position < packageFile.Length)
            {
                int num = binaryReader.ReadInt32();
                packageFile.Read(buffer, 0, num);
                if (num == 65536)
                {
                    memoryStream.Write(buffer, 0, num);
                }
                else
                {
                    ZOutputStream zOutputStream = new ZOutputStream(memoryStream);
                    zOutputStream.Write(buffer, 0, num);
                    zOutputStream.Flush();
                }
            }
            binaryReader = new BinaryReader(memoryStream);
            memoryStream.Seek(4L, SeekOrigin.Begin);
            
            // Write data.bin
            byte[] buffer2 = new byte[288];
            binaryReader.Read(buffer2, 0, 288);
            FileStream dataFile = new FileStream("data.bin", FileMode.Create);
            dataFile.Write(buffer2, 0, 288);
            dataFile.Close();
            memoryStream.Seek(0L, SeekOrigin.Begin);

            String path = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (!File.Exists(path + "\\hashdict.txt"))
            {
                Console.WriteLine("ERROR: hashdict.txt not found.");
                Console.ReadKey();
                return;
            }

            // Read hash dictionary
            StreamReader streamReader = new StreamReader("hashdict.txt");
            int hashCount = Convert.ToInt32(streamReader.ReadLine());
            ulong[] hashes = new ulong[hashCount];
            string[] types = new string[hashCount];
            for (int i = 0; i < hashCount; i++)
            {
                string hashEntry = streamReader.ReadLine();
                int tabPos = hashEntry.IndexOf('\t', 0);
                hashes[i] = Convert.ToUInt64(hashEntry.Substring(0, tabPos), 16);
                types[i] = hashEntry.Substring(tabPos + 1);
            }
            int num4 = binaryReader.ReadInt32();
            memoryStream.Seek(256L, SeekOrigin.Current);

            // Identify file hashes
            for (int j = 0; j < num4; j++)
            {
                ulong currentHash = binaryReader.ReadUInt64();
                binaryReader.ReadUInt64();
                bool known = false;
                for (int i = 0; i < hashCount; i++)
                {
                    if (currentHash == hashes[i])
                    {
                        Console.WriteLine("INFO: extracting file " + types[i]);
                        known = true;
                        break;
                    }
                }
                if (!known)
                {
                    Console.WriteLine("INFO: file hash unknown " + currentHash);
                }
            }
            for (int j = 0; j < num4; j++)
            {
                ulong num6 = binaryReader.ReadUInt64();
                ulong num7 = binaryReader.ReadUInt64();
                string outputType = num6.ToString("X16");
                for (int i = 0; i < hashCount; i++)
                {
                    if (num6 == hashes[i])
                    {
                        outputType = types[i];
                        break;
                    }
                }
                string outputHash = num7.ToString("X16");
                int num8 = binaryReader.ReadInt32();
                binaryReader.ReadInt32();
                int[] array3 = new int[num8];
                int[] array4 = new int[num8];
                int[] array5 = new int[num8];
                for (int i = 0; i < num8; i++)
                {
                    array4[i] = binaryReader.ReadInt32();
                    array3[i] = binaryReader.ReadInt32();
                    array5[i] = binaryReader.ReadInt32();
                }
                // Write output file
                for (int i = 0; i < num8; i++)
                {
                    string outputFileName;
                    if (i == 0)
                    {
                        outputFileName = outputType.Substring(0, 3) + "_" + outputHash + "." + outputType;
                    }
                    else
                    {
                        outputFileName = string.Concat(new string[]
                        {
                            outputType.Substring(0, 3),
                            "_",
                            outputHash,
                            "_",
                            array4[i].ToString("X"),
                            ".",
                            outputType
                        });
                    }
                    byte[] array6 = new byte[array3[i]];
                    memoryStream.Read(array6, 0, array3[i]);
                    // This is the timpany-only branch
                    if (outputType.Contains("timpani"))
                        File.WriteAllBytes(outputFileName, array6);
                    else Console.WriteLine("Skipping file " + outputFileName + ": not a timpani");

                    if (streamFile != null && array5[i] > 0)
                    {
                        byte[] array7 = new byte[array5[i]];
                        streamFile.Read(array7, 0, array5[i]);
                        // This is the timpany-only branch
                        if (outputType.Contains("timpani"))
                            File.WriteAllBytes(outputFileName + ".stream", array7);
                        else Console.WriteLine("Skipping file " + outputFileName + ": not a timpani");
                    }
                }
            }
            Console.WriteLine("Extraction complete.");
            // Console.ReadKey();
        }
    }
}
