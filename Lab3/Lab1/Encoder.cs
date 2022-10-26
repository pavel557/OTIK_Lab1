using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1
{
    class Encoder
    {
        public void Encode(string folderPathRead, string pathWrite)
        {
            //Берем все файлы из папки
            List<string> files = Directory.EnumerateFiles(folderPathRead).ToList();
            //Создаем для каждого файла свой буфер
            List<byte[]> filesBuffers = new List<byte[]>(files.Count);
            //Заплнаяем буферы
            for (int i = 0; i < files.Count; i++)
            {
                using (FileStream fstream = File.OpenRead(files[i]))
                {
                    // выделяем массив для считывания данных из файла
                    filesBuffers.Add(new byte[fstream.Length]);
                    // считываем данные
                    fstream.Read(filesBuffers[i]);
                }
            }

            //Записываем буферы файлов поочередно в архив
            using (FileStream fstream = new FileStream(pathWrite, FileMode.OpenOrCreate))
            {
                int totalLength = filesBuffers.Sum(b => b.Length);

                Header header = new Header(totalLength);
                fstream.Write(header.Signature);
                fstream.Write(BitConverter.GetBytes(header.FormatVersion));
                fstream.Write(header.CompressionAndProtectionAlgorithmCode);
                fstream.Write(BitConverter.GetBytes(header.FileLength));

                for (int i = 0; i < filesBuffers.Count; i++)
                {//! 
                    string currentFileName = new FileInfo(files[i]).Name;
                    //Получили полный путь файла в байтах 
                    byte[] fileNameInbytes = Encoding.UTF8.GetBytes(currentFileName);
                    //Записываем длину названия файла в байтах
                    fstream.Write(BitConverter.GetBytes(fileNameInbytes.Length));
                    //Записываем это назване перед файлом
                    fstream.Write(fileNameInbytes);

                    byte[] buffer = filesBuffers[i];
                    //Записали длину файла
                    fstream.Write(BitConverter.GetBytes(buffer.Length));
                    //Записываем буфер файла
                    fstream.Write(buffer);
                }

                Console.WriteLine("Закодировано");
            }
        }
    }
}
