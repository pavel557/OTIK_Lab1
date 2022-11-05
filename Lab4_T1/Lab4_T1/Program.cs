using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab4_T1
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"H:\LearnMIET\otik\Lab4_T1\Lab4_T1\bin\Debug\netcoreapp3.1\good.txt";
            byte[] bytes = File.ReadAllBytes(path);

            int fileLen = bytes.Length;
            Console.WriteLine("Длина файла = " + fileLen + "\n");

            Dictionary<(byte, byte), int> countMet_ij = new Dictionary<(byte, byte), int>();

            for (int i = 0; i < fileLen - 1; i++)
            {
                if (!countMet_ij.ContainsKey((bytes[i + 1], bytes[i])))
                {
                    countMet_ij.Add((bytes[i + 1], bytes[i]), 0);
                }
                    
                countMet_ij[(bytes[i + 1], bytes[i])]++;
            }

            foreach (var item in countMet_ij)
            {
                Console.WriteLine("Подстрока " + item.Key.Item2 + " " + item.Key.Item1 + " -> " + item.Value);
            }

            Console.WriteLine("");

            Dictionary<byte, int> countMet_j = new Dictionary<byte, int>();

            for (int i = 0; i < fileLen - 1; i++)
            {
                if (!countMet_j.ContainsKey(bytes[i]))
                {
                    countMet_j.Add(bytes[i], 0);
                }
                    
                countMet_j[bytes[i]]++;
            }

            foreach (var item in countMet_j)
            {
                Console.WriteLine("Подстрока " + item.Key + " * -> " + item.Value);
            }

            Dictionary<byte, int> countMet_i = new Dictionary<byte, int>();

            for (int i = 0; i < fileLen; i++)
            {
                if (!countMet_i.ContainsKey(bytes[i]))
                {
                    countMet_i.Add(bytes[i], 0);
                }

                countMet_i[bytes[i]]++;
            }

            Console.WriteLine("\nБезусловные вероятности: ");

            Dictionary<byte, double> unconditionalProbability = new Dictionary<byte, double>();

            foreach (byte b in countMet_i.Keys)
            {
                unconditionalProbability.Add(b, (double)countMet_i[b] / fileLen);
                Console.WriteLine(b + " -> " + (double)countMet_i[b] / fileLen);
            }

            Console.WriteLine("\nУсловные вероятности: ");
            Dictionary<(byte, byte), double> conditionalProbability = new Dictionary<(byte, byte), double>();

            foreach (var item in countMet_ij)
            {
                double probability = (double)item.Value / countMet_j[item.Key.Item2];
                conditionalProbability.Add((item.Key.Item1, item.Key.Item2), probability);
                Console.WriteLine(item.Key.Item1 + " | " + item.Key.Item2 + " -> " + probability);
            }

            double countInformation = -Math.Log(unconditionalProbability.Values.ToArray()[0], 2);

            for (int i = 1; i < bytes.Length; i++)
            {
                byte x_cur = bytes[i];
                byte x_prev = bytes[i - 1];
                double prob = conditionalProbability.ContainsKey((x_cur, x_prev)) ? conditionalProbability[(x_cur, x_prev)] : 0;
                countInformation += -Math.Log(prob, 2);
            }

            Console.WriteLine("\nОбщий объем информации = " + countInformation);
        }
    }
}
