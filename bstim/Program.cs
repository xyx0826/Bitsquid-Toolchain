/* Author: daemon1 (zenhax)
 * Re-compiled by xyx0826
 */

using System;
using System.IO;

namespace bitsquid_timpani
{
    internal class Program
    {
        private static void WriteWavHeader(BinaryWriter bw, int channels, int bits, int samplerate, int bytes)
        {
            bw.Write("RIFF");
            bw.Write(bytes + 36);
            bw.Write("WAVEfmt ");
            bw.Write(16);
            bw.Write(1);
            bw.Write((short)channels);
            bw.Write(samplerate);
            bw.Write(samplerate * channels * bits / 8);
            bw.Write((short)(channels * bits / 8));
            bw.Write((short)bits);
            bw.Write("data");
            bw.Write(bytes);
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Bitsquid Toolchain - bstim (bitsquid_timpany) by daemon1");
            Console.WriteLine("Modified by xyx0826");
            Console.WriteLine("Usage: bstim.exe [bank-name]");
            if (args.Length == 0) // If no parameters are given
            {
                Console.WriteLine("ERROR: bank file not specified.");
                Console.ReadKey();
                return;
            }
            bool isStreamFound = false;
            try
            {
                if (File.Exists(args[0] + ".stream")) isStreamFound = true;
            }
            catch
            {
                Console.WriteLine("INFO: stream file not found.");
            }
            FileStream bankFile = new FileStream(args[0], FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(bankFile);
            FileStream streamFile = null;
            int fileCounts = (int)binaryReader.ReadInt64();
            int[] array = new int[fileCounts];
            int[] array2 = new int[fileCounts];
            ulong[] outFileName = new ulong[fileCounts];
            int[] array4 = new int[fileCounts];
            int[] array5 = new int[fileCounts];
            if (isStreamFound) streamFile = new FileStream(args[0] + ".stream", FileMode.Open);
            for (long num2 = 0L; num2 < fileCounts; num2 += 1L)
            {
                checked
                {
                    outFileName[(int)((IntPtr)num2)] = binaryReader.ReadUInt64();
                    array[(int)((IntPtr)num2)] = binaryReader.ReadInt32();
                    array2[(int)((IntPtr)num2)] = binaryReader.ReadInt32();
                    if (isStreamFound)
                    {
                        array4[(int)((IntPtr)num2)] = binaryReader.ReadInt32();
                        array5[(int)((IntPtr)num2)] = binaryReader.ReadInt32();
                    }
                }
            }
            long num3 = 0L;

            String path = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().Location);
            Directory.CreateDirectory(path + "\\bsunp_extracted");
            while (num3 < fileCounts)
            {
                bankFile.Seek(array[(int)(checked(num3))], SeekOrigin.Begin);
                int num4 = binaryReader.ReadInt32();
                if (num4 != 68)
                {
                    Console.WriteLine("INFO: unknown sound format.");
                }
                int bytes = binaryReader.ReadInt32();
                bankFile.Seek(40L, SeekOrigin.Current);
                int num5 = binaryReader.ReadInt16();
                FileStream fileStream3;
                if (num5 == 1)
                {
                    int channels = binaryReader.ReadInt16();
                    int samplerate = binaryReader.ReadInt32();
                    binaryReader.ReadInt32();
                    binaryReader.ReadInt16();
                    int bits = binaryReader.ReadInt16();
                    binaryReader.ReadInt32();
                    fileStream3 = new FileStream(path + "\\bsunp_extracted\\" + outFileName[(int)(checked(num3))].ToString("X16") + ".wav", FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fileStream3);
                    WriteWavHeader(bw, channels, bits, samplerate, bytes);
                    goto IL_1D1;
                }
                if (num5 == 22127)
                {
                    fileStream3 = new FileStream(path + "\\bsunp_extracted\\" + outFileName[(int)(checked(num3))].ToString("X16") + ".ogg", FileMode.Create);
                    bankFile.Seek((array[(int)(checked(num3))] + 68), SeekOrigin.Begin);
                    goto IL_1D1;
                }
                Console.WriteLine("INFO: unknown sound format.");
                IL_259:
                num3 += 1L;
                continue;
                IL_1D1:
                byte[] buffer = new byte[array2[(int)(checked(num3))] - 68];
                bankFile.Read(buffer, 0, array2[(int)(checked(num3))] - 68);
                fileStream3.Write(buffer, 0, array2[(int)(checked(num3))] - 68);
                checked
                {
                    if (isStreamFound && streamFile != null && array5[(int)((IntPtr)num3)] > 0)
                    {
                        streamFile.Seek(unchecked(array4[(int)(checked(num3))]), SeekOrigin.Begin);
                        buffer = new byte[array5[(int)((IntPtr)num3)]];
                        streamFile.Read(buffer, 0, array5[(int)((IntPtr)num3)]);
                        fileStream3.Write(buffer, 0, array5[(int)((IntPtr)num3)]);
                    }
                    fileStream3.Close();
                    goto IL_259;
                }
            }
            Console.WriteLine("Extraction complete.");
            Console.ReadKey();
        }
    }
}
