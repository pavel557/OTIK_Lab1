using System;
using System.IO;
using System.Text;

namespace Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Encoder encoder = new Encoder();
            //encoder.Encode("test.txt", "test2.txt");

            Decoder decoder = new Decoder();
            decoder.Decode("test2.txt", "test.txt");
        }
    }
}
