using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab1
{
    public class Header
    {
        public byte[] Signature = { 0xBF, 0x21, 0xE5, 0x6F };
        public int FormatVersion = 2;
        public byte[] CompressionAndProtectionAlgorithmCode = { (byte)EncodingType.None };
        public int FileLength = 0;

        public Header()
        {

        }

        public Header(int fileLength)
        {
            FileLength = fileLength;
        }

        public Header(int formatVersion, byte[] compressionAndProtectionAlgorithmCode)
        {
            FormatVersion = formatVersion;
            CompressionAndProtectionAlgorithmCode = compressionAndProtectionAlgorithmCode;
        }

        public Header(int formatVersion, byte[] compressionAndProtectionAlgorithmCode, int fileLength)
        {
            FormatVersion = formatVersion;
            CompressionAndProtectionAlgorithmCode = compressionAndProtectionAlgorithmCode;
            FileLength = fileLength;
        }

        public static Header GetHeader(string pathRead, out int dataStartPosition)
        {
            dataStartPosition = -1;
            byte[] buffer;
            Header newHeader = new Header();
            using (FileStream fstream = File.OpenRead(pathRead))
            {
                // выделяем массив для считывания данных из файла
                buffer = new byte[fstream.Length];
                // считываем данные

                fstream.Read(buffer);
            }

            //проверка сигнатуры
            int i = 0;
            int edge = newHeader.Signature.Length;
            byte[] signature = new byte[edge];
            for (int j = 0; i < edge && i < buffer.Length; i++, j++)
            {
                signature[j] = buffer[i];
                if (signature[j] != newHeader.Signature[j])
                {
                    Console.WriteLine("Неизвестная сигнатура");
                    return null;
                }
            }

            byte[] formatVersion = new byte[4];
            edge += BitConverter.GetBytes(newHeader.FormatVersion).Length;
            for (int j = 0; i < edge && i < buffer.Length; i++, j++)
            {
                formatVersion[j] = buffer[i];
                if (formatVersion[j] != BitConverter.GetBytes(newHeader.FormatVersion)[j])
                {
                    Console.WriteLine("Неактуальная версия");
                    return null;
                }
            }

            //читаем в хедер код алгоритма
            edge += newHeader.CompressionAndProtectionAlgorithmCode.Length;
            for (int j = 0; i < edge && i < buffer.Length; i++, j++)
            {
                newHeader.CompressionAndProtectionAlgorithmCode[j] = buffer[i];
            }

            byte[] fileLength = new byte[4];
            edge += BitConverter.GetBytes(newHeader.FileLength).Length;
            for (int j = 0; i < edge && i < buffer.Length; i++, j++)
            {
                fileLength[j] = buffer[i];
            }

            //пока что размер данных не нужен
            //int fileLengthInt = BitConverter.ToInt32(fileLength, 0);
            //if (fileLengthInt != buffer.Length - edge)
            //{
            //    Console.WriteLine("Размер файла не совпадает");
            //    return;
            //}
            dataStartPosition = i;
            return newHeader;
        }

        public enum EncodingType : byte
        {
            None = 0x00,
            ShenonFano = 0x01,
            RLE = 0x02
        }
    }

    public class MyDataFile
    {
        public Header Header;
        public byte[] Data;

        public MyDataFile(byte[] data)
        {
            Header = new Header(data.Length);
            Data = new byte[data.Length];
            Array.Copy(data, Data, data.Length);
        }
    }


}
