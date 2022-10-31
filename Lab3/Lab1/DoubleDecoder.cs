using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab1
{
    class DoubleDecoder : IDecoder
    {
        public void Decode(string pathRead, string folderPathWrite, int dataStartPosition)
        {
            //создадим новую директорию , чтобы туда закинуть
            //внутренний файл , закодированный Шенноном-Фано
            FileInfo file = new FileInfo(pathRead);
            DirectoryInfo dir = file.Directory;
            DirectoryInfo newDir = Directory.CreateDirectory(dir.FullName + "\\" + "a-temp\\");

            //декодируем внутренний файл во временную папку
            DecoderShannonFano decoderShannonFano = new DecoderShannonFano();
            decoderShannonFano.Decode(file.FullName, newDir.FullName, dataStartPosition);

            //декодируем внешний файл из другой папки в заданную пользователем
            DecoderRLE decoderRLE = new DecoderRLE();
            decoderRLE.Decode(newDir.FullName + "\\" + file.Name, folderPathWrite, 0);
            //удаляем временные файлы
            newDir.Delete(recursive: true);
        }
    }
}
