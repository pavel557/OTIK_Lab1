using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Lab1
{
    class DecoderRLE : IDecoder
    {
        public void Decode(string pathRead, string folderPathWrite, int dataStartPosition)
        {
            byte[] buffer = File.ReadAllBytes(pathRead);

            int i = dataStartPosition;

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

                List<byte> newFile = new List<byte>();

                for (int b = 0; b < fileBuffer.Length; b++)
                {
                    byte bt = fileBuffer[b];
                    string s = Convert.ToString(bt, 2);
                    while (s.Length < 8)
                    {
                        s = "0" + s;
                    }
                    if (s[0] == '1')
                    {
                        char[] charMas = new char[7];
                        s.CopyTo(1, charMas, 0, 7);
                        s = "0" + new string(charMas);
                        int count = Convert.ToByte(s, 2);
                        b++;
                        
                        while (count > 0)
                        {
                            byte cr = fileBuffer[b];
                            newFile.Add(cr);
                            count--;
                            b++;
                        }
                    }
                    else
                    {
                        int count = Convert.ToByte(s, 2);
                        b++;
                        while (count > 0)
                        {
                            byte cr = fileBuffer[b];
                            newFile.Add(cr);
                            count--;
                        }
                    }
                }


                //"C:\Users\123m\source\repos\OTIK_Lab1\Lab1\Lab1\bin\Debug\netcoreapp3.1\files\
                //MinecraftEdu1.pdf
                Directory.CreateDirectory(folderPathWrite);
                string fullFilePath = folderPathWrite + "\\" + fName;
                //C:\Users\123m\source\repos\OTIK_Lab1\Lab1\Lab1\bin\Debug\netcoreapp3.1\files\MinecraftEdu1.pdf

                File.WriteAllBytes(fullFilePath, newFile.ToArray());
            }

            Console.WriteLine("Раскодировано");
        }
    }
}
