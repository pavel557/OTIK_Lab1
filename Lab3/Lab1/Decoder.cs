using System;
using System.IO;
using System.Text;

namespace Lab1
{
    class Decoder : IDecoder
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
