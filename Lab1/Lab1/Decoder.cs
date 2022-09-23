using System;
using System.IO;
using System.Text;

namespace Lab1
{
    class Decoder
    {
        public void Decode(string pathRead, string folderPathWrite)
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

            //int fileLengthInt = BitConverter.ToInt32(fileLength, 0);
            //if (fileLengthInt != buffer.Length - edge)
            //{
            //    Console.WriteLine("Размер файла не совпадает");
            //    return;
            //}


            while (i < buffer.Length)
            {
                //ex: 7asd.txt10aaaaaaaaaa

                byte[] nameLenInBytes = new byte[4];
                for (int b = 0; b < 4; b++, i++)
                    nameLenInBytes[b] = buffer[i];
                int nameLen = BitConverter.ToInt32(nameLenInBytes);//7

                byte[] nameInBytes = new byte[nameLen];
                for (int b = 0; b < nameLen; b++, i++)
                    nameInBytes[b] = buffer[i];
                string fName = Encoding.UTF8.GetString(nameInBytes);//asd.txt

                byte[] fileBufferBytesCount = new byte[4];
                for (int b = 0; b < 4; b++, i++)
                    fileBufferBytesCount[b] = buffer[i];
                int fileBufferLen = BitConverter.ToInt32(fileBufferBytesCount);//10

                byte[] fileBuffer = new byte[fileBufferLen];
                for (int b = 0; b < fileBufferLen; b++, i++)
                    fileBuffer[b] = buffer[i];//aaaaaaaaaa

                //"C:\Users\123m\source\repos\OTIK_Lab1\Lab1\Lab1\bin\Debug\netcoreapp3.1\files\
                //MinecraftEdu1.pdf
                Directory.CreateDirectory(folderPathWrite);
                string fullFilePath = folderPathWrite + "\\" + fName;
                //C:\Users\123m\source\repos\OTIK_Lab1\Lab1\Lab1\bin\Debug\netcoreapp3.1\files\MinecraftEdu1.pdf

                File.WriteAllBytes(fullFilePath, fileBuffer);
            }

            Console.WriteLine("Раскодировано");
        }
    }
}
