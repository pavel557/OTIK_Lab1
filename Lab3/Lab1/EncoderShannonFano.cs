using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab1
{
    class EncoderShannonFano
    {
        public void Encode(string folderPathRead, string pathWrite)
        {
            //Берем все файлы из папки
            List<string> files = Directory.EnumerateFiles(folderPathRead).ToList();
            //Создаем для каждого файла свой буфер
            List<List<byte>> filesBuffers = new List<List<byte>>(files.Count);
            //Заплнаяем буферы
            for (int i = 0; i < files.Count; i++)
            {
                using (FileStream fstream = File.OpenRead(files[i]))
                {
                    byte[] fileBuffersАrray = new byte[fstream.Length];
                    // выделяем массив для считывания данных из файла
                    // считываем данные
                    fstream.Read(fileBuffersАrray);
                    filesBuffers.Add(fileBuffersАrray.ToList<byte>());
                }
            }

            List<List<byte>> newFilesBuffers = new List<List<byte>>();
            List<List<int>> frequency = new List<List<int>>();

            foreach (List<byte> fileBuffers in filesBuffers)
            {
                List<byte> newfileBuffers = new List<byte>();
                List<int> newFrequency = new List<int>();

                foreach (byte byteValue in fileBuffers)
                {
                    if (newfileBuffers.Contains(byteValue))
                    {
                        int index = newfileBuffers.IndexOf(byteValue);
                        newFrequency[index]++;
                    }
                    else
                    {
                        newfileBuffers.Add(byteValue);
                        newFrequency.Add(1);
                    }
                }

                newFilesBuffers.Add(newfileBuffers);
                frequency.Add(newFrequency);
            }

            for (int i=0; i<newFilesBuffers.Count; i++)
            {
                for (int j=0; j<newFilesBuffers[i].Count; j++)
                {
                    for (int k=j+1; k<newFilesBuffers[i].Count; k++)
                    {
                        if (frequency[i][j] < frequency[i][k])
                        {
                            byte buff = newFilesBuffers[i][j];
                            newFilesBuffers[i][j] = newFilesBuffers[i][k];
                            newFilesBuffers[i][k] = buff;

                            int freq = frequency[i][j];
                            frequency[i][j] = frequency[i][k];
                            frequency[i][k] = freq;
                        }
                    }
                }
                for (int j = 0; j < newFilesBuffers[i].Count; j++)
                {
                    for (int k = j + 1; k < newFilesBuffers[i].Count; k++)
                    {
                        if (frequency[i][j] < frequency[i][k])
                        {
                            byte buff = newFilesBuffers[i][j];
                            newFilesBuffers[i][j] = newFilesBuffers[i][k];
                            newFilesBuffers[i][k] = buff;

                            int freq = frequency[i][j];
                            frequency[i][j] = frequency[i][k];
                            frequency[i][k] = freq;
                        }
                    }
                }
            }

            //for (int i=0; i<newFilesBuffers.Count; i++)
            //{
            //    for (int j=0; j<newFilesBuffers[i].Count; j++)
            //    {
            //        Console.WriteLine(newFilesBuffers[i][j] + " " + frequency[i][j] + " " 
            //            + CalculateСharacterСode(new List<byte>(newFilesBuffers[i]), new List<int>(frequency[i]), newFilesBuffers[i][j], ""));
            //    }
            //    Console.WriteLine("_________________");
            //}

            List<List<byte>> AllFileBytes = new List<List<byte>>();
            for (int i = 0; i < newFilesBuffers.Count; i++)
            {
                string bytesFileString = "";
                foreach (byte byteValue in filesBuffers[i])
                {
                    bytesFileString = bytesFileString + CalculateСharacterСode(new List<byte>(newFilesBuffers[i]), new List<int>(frequency[i]), byteValue, "");
                }

                AllFileBytes.Add(new List<byte>());

                byte[] countTableElements = new byte[4];
                countTableElements = BitConverter.GetBytes(newFilesBuffers[i].Count);

                foreach (byte Cbyte in countTableElements)
                {
                    AllFileBytes[i].Add(Cbyte);
                }

                List<byte[]> tableElements = new List<byte[]>();

                for (int j=0; j<newFilesBuffers[i].Count; j++)
                {
                    byte[] tableElement = new byte[4];
                    
                    tableElement = BitConverter.GetBytes(frequency[i][j]);
                    Array.Reverse(tableElement);

                    tableElement[0] = newFilesBuffers[i][j];

                    tableElements.Add(tableElement);
                }

                foreach (byte[] Tbytes in tableElements)
                {
                    foreach (byte Tbyte in Tbytes)
                    {
                        AllFileBytes[i].Add(Tbyte);
                    }
                }


                byte[] countСharacter = new byte[4];
                countСharacter = BitConverter.GetBytes(bytesFileString.Length);

                foreach (byte Cbyte in countСharacter)
                {
                    AllFileBytes[i].Add(Cbyte);
                }

                while (bytesFileString.Length % 8 != 0)
                {
                    bytesFileString = "0" + bytesFileString;
                }

                List<byte> listBytesFile = new List<byte>();

                for (int j=0; j<bytesFileString.Length; j+=8)
                {
                    char[] mas = new char[8];
                    bytesFileString.CopyTo(j, mas, 0, 8);
                    listBytesFile.Add(Convert.ToByte(new string(mas), 2));
                }

                foreach (byte Lbyte in listBytesFile)
                {
                    AllFileBytes[i].Add(Lbyte);
                }
            }

            //Console.WriteLine(AllFileBytes.Sum(b => b.Count));
            for (int i=0; i< AllFileBytes.Count; i++)
            {
                foreach (byte b in AllFileBytes[i])
                {
                    //Console.WriteLine(Convert.ToString(b, 2));
                }    
            }

            //Записываем буферы файлов поочередно в архив
            using (FileStream fstream = new FileStream(pathWrite, FileMode.OpenOrCreate))
            {
                int totalLength = AllFileBytes.Sum(b => b.Count);

                Header header = new Header(2, new byte[1] { (byte)Header.EncodingType.ShenonFano }, totalLength);
                fstream.Write(header.Signature);
                fstream.Write(BitConverter.GetBytes(header.FormatVersion));
                fstream.Write(header.CompressionAndProtectionAlgorithmCode);
                fstream.Write(BitConverter.GetBytes(header.FileLength));

                for (int i = 0; i < AllFileBytes.Count; i++)
                {//! 
                    string currentFileName = new FileInfo(files[i]).Name;
                    //Получили полный путь файла в байтах 
                    byte[] fileNameInbytes = Encoding.UTF8.GetBytes(currentFileName);
                    //Записываем длину названия файла в байтах
                    fstream.Write(BitConverter.GetBytes(fileNameInbytes.Length));
                    //Записываем это назване перед файлом
                    fstream.Write(fileNameInbytes);

                    //если не нужно кодировать
                    if(AllFileBytes[i].Count >= filesBuffers[i].Count)
                    {
                        fstream.Write(new byte[1] { 0x0 });
                        fstream.Write(BitConverter.GetBytes(filesBuffers[i].Count));
                        fstream.Write(filesBuffers[i].ToArray());
                        continue;
                    }
                    //говорим, что нужно декодировать
                    fstream.Write(new byte[1] { 0x1 });
                    byte[] buffer = AllFileBytes[i].ToArray();
                    //записываем дилну данных
                    fstream.Write(BitConverter.GetBytes(buffer.Length));
                    //Записываем буфер файла
                    fstream.Write(buffer);
                }

                Console.WriteLine("Закодировано");
            }
        }

        public string CalculateСharacterСode(List<byte> characters, List<int> frequency, byte characterValue, string code)
        {
            if (characters.Count == 1)
            {
                return code;
            }

            int rightSumFrequency = 0;
            foreach (int frequencyElement in frequency)
            {
                rightSumFrequency += frequencyElement;
            }

            List<byte> LeftCharacters = new List<byte>();
            List<int> LeftFrequency = new List<int>();

            int leftSumFrequency = 0;
            while (leftSumFrequency + frequency[0] < rightSumFrequency)
            {
                leftSumFrequency += frequency[0];
                rightSumFrequency -= frequency[0];
                LeftCharacters.Add(characters[0]);
                LeftFrequency.Add(frequency[0]);
                characters.RemoveAt(0);
                frequency.RemoveAt(0);

            }

            if (LeftCharacters.Contains(characterValue))
            {
                return code + CalculateСharacterСode(LeftCharacters, LeftFrequency, characterValue, "0");
            }
            else
            {
                return code + CalculateСharacterСode(characters, frequency, characterValue, "1");
            }
        }
    }
}
