using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1
{
    class EncoderRLE
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

            List<List<byte>> newFilesBytes = new List<List<byte>>();

            foreach (byte[] fileBytes in filesBuffers)
            {
                List<byte> newFileBytes = new List<byte>();
                List<byte> buff = new List<byte>();
                bool isRepeat = true;
                for (int i=0; i<fileBytes.Length; i++)
                {
                    if (buff.Count == 0)
                    {
                        buff.Add(fileBytes[i]);
                    }
                    else if (fileBytes[i] == buff[buff.Count - 1])
                    {
                        if (isRepeat)
                        {
                            buff.Add(fileBytes[i]);
                        }
                        else
                        {
                            isRepeat = true;
                            int countChar = buff.Count;
                            byte[] countCharInByte = BitConverter.GetBytes(countChar);
                            int newCountChar = (int)countCharInByte[0] + 128;
                            byte[] newcountCharInByte = BitConverter.GetBytes(newCountChar);
                            byte finalValue = newcountCharInByte[0];
                            newFileBytes.Add(finalValue);
                            foreach (byte b in buff)
                            {
                                newFileBytes.Add(b);
                            }
                            buff.Clear();
                            buff.Add(fileBytes[i]);
                        }
                    }
                    else
                    {
                        if (isRepeat && buff.Count >= 3)
                        {
                            int countChar = buff.Count;
                            byte[] countCharInByte = BitConverter.GetBytes(countChar);
                            byte finalValue = countCharInByte[0];
                            newFileBytes.Add(finalValue);
                            newFileBytes.Add(buff[0]);
                            buff.Clear();
                            buff.Add(fileBytes[i]);

                        }
                        else
                        {
                            buff.Add(fileBytes[i]);
                            isRepeat = false;
                        }
                    }

                    if (buff.Count == 127)
                    {
                        if (isRepeat)
                        {
                            int countChar = buff.Count;
                            byte[] countCharInByte = BitConverter.GetBytes(countChar);
                            byte finalValue = countCharInByte[0];
                            newFileBytes.Add(finalValue);
                            newFileBytes.Add(buff[0]);
                            buff.Clear();

                        }
                        else
                        {
                            int countChar = buff.Count;
                            byte[] countCharInByte = BitConverter.GetBytes(countChar);
                            int newCountChar = (int)countCharInByte[0] + 128;
                            byte[] newcountCharInByte = BitConverter.GetBytes(newCountChar);
                            byte finalValue = newcountCharInByte[0];
                            newFileBytes.Add(finalValue);
                            foreach (byte b in buff)
                            {
                                newFileBytes.Add(b);
                            }
                            buff.Clear();
                        }
                    }
                }

                if (buff.Count != 0)
                {
                    if (isRepeat)
                    {
                        int countChar = buff.Count;
                        byte[] countCharInByte = BitConverter.GetBytes(countChar);
                        byte finalValue = countCharInByte[0];
                        newFileBytes.Add(finalValue);
                        newFileBytes.Add(buff[0]);
                        buff.Clear();

                    }
                    else
                    {
                        int countChar = buff.Count;
                        byte[] countCharInByte = BitConverter.GetBytes(countChar);
                        int newCountChar = (int)countCharInByte[0] + 128;
                        byte[] newcountCharInByte = BitConverter.GetBytes(newCountChar);
                        byte finalValue = newcountCharInByte[0];
                        newFileBytes.Add(finalValue);
                        foreach (byte b in buff)
                        {
                            newFileBytes.Add(b);
                        }
                        buff.Clear();
                    }
                }

                newFilesBytes.Add(newFileBytes);
            }

            //Записываем буферы файлов поочередно в архив
            using (FileStream fstream = new FileStream(pathWrite, FileMode.OpenOrCreate))
            {
                int totalLength = newFilesBytes.Sum(b => b.Count);

                Header header = new Header(2, new byte[1] { (byte)Header.EncodingType.RLE }, totalLength);
                fstream.Write(header.Signature);
                fstream.Write(BitConverter.GetBytes(header.FormatVersion));
                fstream.Write(header.CompressionAndProtectionAlgorithmCode);
                fstream.Write(BitConverter.GetBytes(header.FileLength));

                for (int i = 0; i < newFilesBytes.Count; i++)
                {//! 
                    string currentFileName = new FileInfo(files[i]).Name;
                    //Получили полный путь файла в байтах 
                    byte[] fileNameInbytes = Encoding.UTF8.GetBytes(currentFileName);
                    //Записываем длину названия файла в байтах
                    fstream.Write(BitConverter.GetBytes(fileNameInbytes.Length));
                    //Записываем это назване перед файлом
                    fstream.Write(fileNameInbytes);

                    byte[] buffer = newFilesBytes[i].ToArray();
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
