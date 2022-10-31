using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab1
{
    class DoubleEncoder
    {
        public void Encode(string folderPathRead, string pathWrite)
        {
            //закодируем сначала алгоритмом RLE
            EncoderRLE encoderRLE = new EncoderRLE();
            encoderRLE.Encode(folderPathRead, pathWrite);
            Header header = new Header();

            //перекинем этот файл в новую папку , чтобы не переписывать следующий кодер
            FileInfo file = new FileInfo(pathWrite);
            DirectoryInfo dir = file.Directory;
            DirectoryInfo newDir = Directory.CreateDirectory(dir.FullName + "\\" + "a-tmp\\");
            file.CopyTo(newDir.FullName + "\\" + file.Name);
            file.Delete();

            //применим к новому скопированному файлу алгоритм Шеннона-Фано
            //и закинем в заданный изанчально файл
            EncoderShannonFano encoderShannonFano = new EncoderShannonFano();
            encoderShannonFano.Encode(newDir.FullName, pathWrite, header.HeaderStructSize);
            //удалим созданные временные файлы
            newDir.Delete(recursive: true);

            using (FileStream fstream = new FileStream(pathWrite, FileMode.OpenOrCreate))
            {
                //переписываем сигнатуру для идентификации как двойное
                //применение алгоритмов

                //пропускаем сигнатуру
                fstream.Seek(header.Signature.Length, SeekOrigin.Current);
                //пропускаем версию
                fstream.Seek(sizeof(int), SeekOrigin.Current);
                //переписываем байт кодировки с Шеннона-Фано на двойную кодировку
                fstream.Write(new byte[] { (byte)Header.EncodingType.DoubleEncode });
            }
        }
    }
}
