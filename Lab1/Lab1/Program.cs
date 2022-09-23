using System;

namespace Lab1
{
    class Program
    {
        static int ChooseComand()
        {
            Console.WriteLine("Choose operation:\n");
            Console.WriteLine("0-exit from program\n1-encode file\n2-decode file\n");
            Console.WriteLine("---------------------------------------------------");
            return Convert.ToInt32(Console.ReadLine());
        }

        static (string pathRead, string pathWrite) GetFileNames()
        {//При шифровании pathRead - путь до папки, а при дешифровке это название файла и наоборот
            Console.WriteLine("Enter the path name to read: ");
            string pathRead = Console.ReadLine();
            Console.WriteLine("Enter the path name to write: ");
            string pathWrite = Console.ReadLine();

            return (pathRead, pathWrite);
        }

        static void Main(string[] args)
        {
            while (true)
            {
                switch (ChooseComand())
                {
                    case 0:
                        Environment.Exit(0);
                        break;
                    case 1:
                        Console.WriteLine("Encoding...");
                        var (pathRead, pathWrite) = GetFileNames();
                        Encoder encoder = new Encoder();
                        encoder.Encode(pathRead, pathWrite);
                        break;
                    case 2:
                        Console.WriteLine("Decoding...");
                        (pathRead, pathWrite) = GetFileNames();
                        Decoder decoder = new Decoder();
                        decoder.Decode(pathRead, pathWrite);
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
