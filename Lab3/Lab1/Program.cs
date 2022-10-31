using System;

namespace Lab1
{
    class Program
    {
        static int ChooseComand()
        {
            Console.WriteLine("Choose operation:\n");
            Console.WriteLine(
                "0-exit from program\n" +
                "1-encode file simpe\n" +
                "2-encode file shenon fano\n" +
                "3-encode file RLE\n" +
                "4-encode file double encoding\n" +
                "5-decode file\n");
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
                        Console.WriteLine("Simple encoding...");
                        var (pathRead, pathWrite) = GetFileNames();
                        Encoder encoder = new Encoder();
                        encoder.Encode(pathRead, pathWrite);
                        break;
                    case 2:
                        Console.WriteLine("ShenonFano encoding...");
                        (pathRead, pathWrite) = GetFileNames();
                        EncoderShannonFano SFencoder = new EncoderShannonFano();
                        SFencoder.Encode(pathRead, pathWrite);
                        break;
                    case 3:
                        Console.WriteLine("RLE encoding...");
                        (pathRead, pathWrite) = GetFileNames();
                        EncoderRLE RLEencoder = new EncoderRLE();
                        RLEencoder.Encode(pathRead, pathWrite);
                        break;
                    case 4:
                        Console.WriteLine("Double encoding...");
                        (pathRead, pathWrite) = GetFileNames();
                        DoubleEncoder doubleEncoder = new DoubleEncoder();
                        doubleEncoder.Encode(pathRead, pathWrite);
                        break;
                    case 5:
                        Console.WriteLine("Decoding...");
                        (pathRead, pathWrite) = GetFileNames();

                        Header header = Header.GetHeader(pathRead, out int dataStartPosition);
                        if(header is null)
                        {
                            Console.WriteLine("Ошибка чтения хедера.");
                            break;
                        }
                        if(header.CompressionAndProtectionAlgorithmCode.Length < 1)
                        {
                            Console.WriteLine("Неверный формат кода алгоритма в хедере файла!");
                            break;
                        }

                        IDecoder decoder = null;
                        switch ((Header.EncodingType)header.CompressionAndProtectionAlgorithmCode[0])
                        {
                            case Header.EncodingType.None:
                                decoder = new Decoder();
                                break;
                            case Header.EncodingType.ShenonFano:
                                decoder = new DecoderShannonFano();
                                break;
                            case Header.EncodingType.RLE:
                                decoder = new DecoderRLE();
                                break;
                            case Header.EncodingType.DoubleEncode:
                                decoder = new DoubleDecoder();
                                break;
                        }
                        decoder.Decode(pathRead, pathWrite, dataStartPosition);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
