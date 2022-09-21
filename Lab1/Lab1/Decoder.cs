using System;
using System.IO;
using System.Text;

namespace Lab1
{
    class Decoder
    {
        public void Decode(string pathRead, string pathWrite)
        {
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
                    return;
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
                    return;
                }
            }

            byte[] сompressionAndProtectionAlgorithmCode = new byte[1];
            edge += newHeader.CompressionAndProtectionAlgorithmCode.Length;
            for (int j = 0; i < edge && i < buffer.Length; i++, j++)
            {
                сompressionAndProtectionAlgorithmCode[j] = buffer[i];
                if (сompressionAndProtectionAlgorithmCode[j] != newHeader.CompressionAndProtectionAlgorithmCode[j])
                {
                    Console.WriteLine("Код алгоритма не совпадает");
                    return;
                }
            }

            byte[] fileLength = new byte[4];
            edge += BitConverter.GetBytes(newHeader.FileLength).Length;
            for (int j = 0; i < edge && i < buffer.Length; i++, j++)
            {
                fileLength[j] = buffer[i];
            }

            int fileLengthInt = BitConverter.ToInt32(fileLength, 0);
            byte[] data = new byte[fileLengthInt];
            if (fileLengthInt != buffer.Length - edge)
            {
                Console.WriteLine("Размер файла не совпадает");
                return;
            }
            for (int j = 0; i < buffer.Length; i++, j++)
            {
                data[j] = buffer[i]; //преобразование Y -> ~X
            }

            using (FileStream fstream = new FileStream(pathWrite, FileMode.OpenOrCreate))
            {
                // запись массива байтов в файл
                fstream.Write(data);

                Console.WriteLine("Раскодировано");
            }
        }
    }
}
