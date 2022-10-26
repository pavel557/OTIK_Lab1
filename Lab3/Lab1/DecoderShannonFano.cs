using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace Lab1
{
    class DecoderShannonFano
    {
        public void Decode(string pathRead, string folderPathWrite)
        {
            byte[] buffer;
            Header newHeader = new Header(2, new byte[1]{ 0x01 });
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

                //количество записей в таблице
                byte[] countTableElements = new byte[4];
                for (int b = 0; b < 4; b++, i++)
                    countTableElements[b] = buffer[i];
                int countTableElementsInt = BitConverter.ToInt32(countTableElements);//10

                byte[] TableElements = new byte[countTableElementsInt];
                int[] frequency = new int[countTableElementsInt];

                for (int b = 0; b < countTableElementsInt; b++)
                {
                    byte[] tableElement = new byte[4];
                    for (int c = 0; c < 4; c++, i++)
                    {
                        tableElement[c] = buffer[i];
                    }
                    TableElements[b] = tableElement[0];

                    //вычислить частоту
                    byte[] frequencyByte = new byte[4];
                    frequencyByte[0] = tableElement[3];
                    frequencyByte[1] = tableElement[2];
                    frequencyByte[2] = tableElement[1];
                    frequencyByte[3] = 0;
                    frequency[b] = BitConverter.ToInt32(frequencyByte);
                }

                //дальше посчитать количество бит, вычислить словарь код->байт, и считать сообщение, отбросить биты незначащие и проверять

                byte[] countBits = new byte[4];
                for (int b = 0; b < 4; b++, i++)
                    countBits[b] = buffer[i];
                countBits.Reverse();
                int countBitsInt = BitConverter.ToInt32(countBits);

                int countByte = countBitsInt;
                while (countByte % 8 != 0)
                {
                    countByte++;
                }
                countByte = countByte / 8;

                byte[] message = new byte[countByte];
                for (int b = 0; b < countByte; b++, i++)
                    message[b] = buffer[i];


                Dictionary<string, byte> codeByte = new Dictionary<string, byte>();

                for(int b=0; b<TableElements.Length; b++)
                {
                    codeByte.Add(CalculateСharacterСode(new List<byte>(TableElements), new List<int>(frequency), TableElements[b], ""), TableElements[b]);
                }

                string messageString = "";
                foreach(byte b in message)
                {
                    string s = Convert.ToString(b, 2);
                    while (s.Length < 8)
                    {
                        s = "0" + s;
                    }
                    messageString += s;
                }
                string bufString = "";
                List<byte> originMessage = new List<byte>();

                for (int index = countByte * 8 - countBitsInt; index < messageString.Length; index++)
                {
                    bufString += messageString[index];
                    if (codeByte.ContainsKey(bufString))
                    {
                        originMessage.Add(codeByte[bufString]);
                        bufString = "";
                    }
                }


                //"C:\Users\123m\source\repos\OTIK_Lab1\Lab1\Lab1\bin\Debug\netcoreapp3.1\files\
                //MinecraftEdu1.pdf
                Directory.CreateDirectory(folderPathWrite);
                string fullFilePath = folderPathWrite + "\\" + fName;
                //C:\Users\123m\source\repos\OTIK_Lab1\Lab1\Lab1\bin\Debug\netcoreapp3.1\files\MinecraftEdu1.pdf

                File.WriteAllBytes(fullFilePath, originMessage.ToArray());
            }

            Console.WriteLine("Раскодировано");
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
