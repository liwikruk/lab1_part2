using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp17
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;//і

            string path = "";//файл txt
            string pathToCmprssed = "";//архів
            string pathToCrypt = "";//txt Base64, треба, щоб знати розміри вихідного файлу
            char[] myBase64;//для закодування в Base64
            int totalSymbols = 0;
            double entropy, infoQuant, sizeFile;
            Dictionary<char, double> symbols = new Dictionary<char, double>();
            string text, encodedFileVS, encryptRow;
            byte[] data;

            ChoosePath(ref path, ref pathToCmprssed, ref pathToCrypt);//обираємо який файл та архів досліджуємо

            Console.WriteLine("===================================================================");
            Console.WriteLine(path +"\n");
            text = ReadText(path);//зчитуємо текст
            Console.WriteLine("Закодуємо до Base64 за допомогою функції в Visual Studio\n");
            encodedFileVS = Convert.ToBase64String(Encoding.Default.GetBytes(text));//кодуємо до Base64 txt файл за допомогою функції в Visual Studio
            Console.WriteLine(encodedFileVS + "\n");

            Console.WriteLine("===================================================================");
            Console.WriteLine("Закодуємо до Base64 за допомогою самостійно реалізованого алгоритму\n");
            Console.WriteLine("[txt(base64)]\n");
            Console.WriteLine();
            text = ReadText(path);//зчитуємо текст
            sizeFile = CountSymbols(path, text, symbols, out totalSymbols);//рахуємо символи та скільки кожен символ з'являється
            CountFrequency(symbols, totalSymbols);//рахуємо частоту 
            entropy = CountEntropy(symbols);
            infoQuant = InformationAmount(entropy, totalSymbols);
            data = System.Text.Encoding.Default.GetBytes(text);
            myBase64 = Base64Encode(data);//кодуємо до Base64 за самостійно реалізованим алгоритмом
            encryptRow = new string(myBase64);
            Console.WriteLine(encryptRow);
            Console.WriteLine();
            WriteBase64File(pathToCrypt, encryptRow);
            FileInfo fileSize = new FileInfo(pathToCrypt);//розмір вихідного файлу
            PrintInformation(infoQuant, fileSize.Length);

            Console.WriteLine("===================================================================");
            Console.WriteLine("[base64 + bz2]\n");
            text = ReadText(pathToCmprssed);//зчитуємо текст           
            data = System.Text.Encoding.Default.GetBytes(text);
            myBase64 = Base64Encode(data);//кодуємо до Base64 за самостійно реалізованим алгоритмом
            encryptRow = new string(myBase64);
            Console.WriteLine(encryptRow);
            sizeFile = CountSymbols(pathToCmprssed, text, symbols, out totalSymbols);
            CountFrequency(symbols, totalSymbols);
            entropy = CountEntropy(symbols);
            infoQuant = InformationAmount(entropy, totalSymbols);
            Console.WriteLine();
            PrintInformation(infoQuant, sizeFile);
            Console.WriteLine("===================================================================");
        }

        static void ChoosePath(ref string path, ref string pathToCmprssed, ref string pathToCrypt)//обираємо, які файли досліджуємо
        {
            string path1 = @"F:\Bodnar KI 3_2\кс\lab1\stus.txt";
            string pathToCmprssed1 = @"F:\Bodnar KI 3_2\кс\lab1\archive\stus.bz2";
            string pathToCrypt1 = @"F:\Bodnar KI 3_2\кс\lab1\crypt\stus.txt";
            string path2 = @"F:\Bodnar KI 3_2\кс\lab1\kolobok.txt";
            string pathToCmprssed2 = @"F:\Bodnar KI 3_2\кс\lab1\archive\kolobok.bz2";
            string pathToCrypt2 = @"F:\Bodnar KI 3_2\кс\lab1\crypt\kolobok.txt";
            string path3 = @"F:\Bodnar KI 3_2\кс\lab1\oop.txt";
            string pathToCmprssed3 = @"F:\Bodnar KI 3_2\кс\lab1\archive\oop.bz2";
            string pathToCrypt3 = @"F:\Bodnar KI 3_2\кс\lab1\crypt\oop.txt";
            Console.WriteLine("1,2,3 - Оберіть досліджувальний файл");
            int selection = int.Parse(Console.ReadLine());
            switch (selection)
            {
                case 1:
                    path = path1;
                    pathToCmprssed = pathToCmprssed1;
                    pathToCrypt = pathToCrypt1;
                    break;
                case 2:
                    path = path2;
                    pathToCmprssed = pathToCmprssed2;
                    pathToCrypt = pathToCrypt2;
                    break;
                case 3:
                    path = path3;
                    pathToCmprssed = pathToCmprssed3;
                    pathToCrypt = pathToCrypt3;
                    break;
                default:
                    Console.WriteLine("Помилка");
                    break;
            }
        }

        //Частота
        public static void CountFrequency(Dictionary<char, double> symbols, int totalSymbols)
        {
            char[] keys = new char[symbols.Keys.Count];
            symbols.Keys.CopyTo(keys, 0);
            for (int counter = 0; counter < symbols.Keys.Count; counter++)
            {
                symbols[keys[counter]] = symbols[keys[counter]] / totalSymbols;
            }
        }

        //Ентропія
        public static double CountEntropy(Dictionary<char, double> symbols)
        {
            double frequency = 0, entropy = 0;
            char[] keys = new char[symbols.Keys.Count];
            symbols.Keys.CopyTo(keys, 0);
            
            for (int i = 0; i < symbols.Keys.Count; i++)
            {
                frequency = symbols[keys[i]];//частота
                entropy -= frequency * Math.Log(frequency, 2);
            }
            return entropy;
        }

        //К-сть інформації
        public static double InformationAmount(double entropy, int totalSymbols)
        {
            return entropy * totalSymbols;
        }

        //Інформація
        public static void PrintInformation(double infoQuant, double sizeFile)
        {
            Console.WriteLine("Розмір файлу = {0} bytes", sizeFile);
            Console.WriteLine("Кількість інформації = {0} bytes", infoQuant / 8);
            Console.WriteLine("Кількість інформації = {0} bits\n", infoQuant);
        }

        //Кількість символів, розмір файлу
        public static double CountSymbols(string path, string text, Dictionary<char, double> symbols,out int totalSymbols)
        {       
            int counter = 0;
            double symbolIndex;
            totalSymbols = 0;           
            while (counter < text.Length)
            {
                symbolIndex = 1;
                if (!symbols.ContainsKey(text[counter]))
                {
                    symbols.Add(text[counter], symbolIndex);
                }
                else
                    if (symbols.ContainsKey(text[counter]))
                {
                    symbols[text[counter]]++;
                }
                counter++;
                totalSymbols++;
            }
            FileInfo fileSize = new FileInfo(path);
            return fileSize.Length;//отримуємо розмір файлу
        }

        //Base64
        public static char[] Base64Encode(byte[] data)
        {
            int length, lengthText;
            int blockCount;
            int paddingCount;
            length = data.Length;
            if ((length % 3) == 0)
            {
                paddingCount = 0;
                blockCount = length / 3;
            }
            else
            {
                paddingCount = 3 - (length % 3);
                blockCount = (length + paddingCount) / 3;
            }
            lengthText = length + paddingCount;
            byte[] source;
            source = new byte[lengthText];
            for (int x = 0; x < lengthText; x++)
            {
                if (x < length)
                {
                    source[x] = data[x];
                }
                else
                {
                    source[x] = 0;
                }
            }
            byte b1, b2, b3;
            byte temp1, temp2, temp3, temp4, temp5;
            byte[] buffer = new byte[blockCount * 4];
            char[] result = new char[blockCount * 4];
            for (int x = 0; x < blockCount; x++)
            {
                b1 = source[x * 3];
                b2 = source[x * 3 + 1];
                b3 = source[x * 3 + 2];

                temp2 = (byte)((b1 & 252) >> 2);
                temp1 = (byte)((b1 & 3) << 4);
                temp3 = (byte)((b2 & 240) >> 4);
                temp3 += temp1;
                temp1 = (byte)((b2 & 15) << 2);
                temp4 = (byte)((b3 & 192) >> 6);
                temp4 += temp1;
                temp5 = (byte)(b3 & 63);

                buffer[x * 4] = temp2;
                buffer[x * 4 + 1] = temp3;
                buffer[x * 4 + 2] = temp4;
                buffer[x * 4 + 3] = temp5;

            }

            for (int x = 0; x < blockCount * 4; x++)
            {
                result[x] = Symbols(buffer[x]);
            }

            switch (paddingCount)
            {
                case 0:
                    break;
                case 1:
                    result[blockCount * 4 - 1] = '=';
                    break;
                case 2:
                    result[blockCount * 4 - 1] = '=';
                    result[blockCount * 4 - 2] = '=';
                    break;
                default:
                    break;
            }
            return result;
        }

        public static char Symbols(byte n)
        {
            char[] table = new char[64] {
    'A','B','C','D','E','F','G','H','I','J','K','L','M',
    'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
    'a','b','c','d','e','f','g','h','i','j','k','l','m',
    'n','o','p','q','r','s','t','u','v','w','x','y','z',
    '0','1','2','3','4','5','6','7','8','9','+','/'
  };
            if ((n >= 0) && (n <= 63))
            {
                return table[n];
            }
            else
            {
                return ' ';
            }
        }
        static string ReadText(string file)
        {
            string text = "";
            string line;
            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        text += line + "\n"; //записуємо весь текст з файлу
                    }
                }
            }
            else
                throw new Exception("Файла не існує");
            return text;
        }

        public static void WriteBase64File(string pathFile, string text)//записуємо закодований файл, щоб потім дізнатися розміри вихідного файлу
        {
            using (StreamWriter sw = new StreamWriter(pathFile, false, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(text);
            }
        }

    }
}
