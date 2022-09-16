using System;
using System.IO;
using System.Text;

namespace Lab1
{
    class Encoder
    {
        public void Encode(string pathRead, string pathWrite)
        {
            byte[] buffer;
            using (FileStream fstream = File.OpenRead(pathRead))
            {
                // выделяем массив для считывания данных из файла
                buffer = new byte[fstream.Length];
                // считываем данные

                fstream.Read(buffer);
            }
            
            using (FileStream fstream = new FileStream(pathWrite, FileMode.OpenOrCreate))
            {
                
                // запись массива байтов в файл
                Header header = new Header(buffer.Length);

                fstream.Write(header.Signature);
                fstream.Write(BitConverter.GetBytes(header.FormatVersion));
                fstream.Write(header.CompressionAndProtectionAlgorithmCode);
                fstream.Write(BitConverter.GetBytes(header.FileLength));
                fstream.Write(buffer);
                
                Console.WriteLine("Закодировано");
            }
        }
    }
}
