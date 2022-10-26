using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class Header
    {
        public byte[] Signature = { 0xBF, 0x21, 0xE5, 0x6F };
        public int FormatVersion = 1;
        public byte[] CompressionAndProtectionAlgorithmCode = { 0x00 };
        public int FileLength = 0;

        public Header()
        {

        }

        public Header(int fileLength)
        {
            FileLength = fileLength;
        }

        public Header(int formatVersion, byte[] compressionAndProtectionAlgorithmCode)
        {
            FormatVersion = formatVersion;
            CompressionAndProtectionAlgorithmCode = compressionAndProtectionAlgorithmCode;
        }

        public Header(int formatVersion, byte[] compressionAndProtectionAlgorithmCode, int fileLength)
        {
            FormatVersion = formatVersion;
            CompressionAndProtectionAlgorithmCode = compressionAndProtectionAlgorithmCode;
            FileLength = fileLength;
        }
    }

    public class MyDataFile
    {
        public Header Header;
        public byte[] Data;

        public MyDataFile(byte[] data)
        {
            Header = new Header(data.Length);
            Data = new byte[data.Length];
            Array.Copy(data, Data, data.Length);
        }
    }


}
