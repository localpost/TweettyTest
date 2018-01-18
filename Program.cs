using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using TweetSharp;
using System.Net;

namespace TwittyTest
{
    class Program
    {
        //Получаем зашифрованные ключи от API твиттера из файла конфигурации
        static string Key = ConfigurationSettings.AppSettings.GetValues("ConsumerKey")[0];
        static string Secret = ConfigurationSettings.AppSettings.GetValues("ConsumerSecret")[0];
        static string TokenKey = ConfigurationSettings.AppSettings.GetValues("TokenKey")[0];
        static string TokenSecret = ConfigurationSettings.AppSettings.GetValues("TokenSecret")[0];      

        static void Main(string[] args)
        {
            TwitService Service = new TwitService(Encrypt(Key, 2), Encrypt(Secret, 2), Encrypt(TokenKey, 2), Encrypt(TokenSecret, 2));
            bool exit = false;
            string userName = "";
            char[] abc = MakeABC('a', 'z');

            while (!exit)
            {
                
                Console.Write("Введите имя (для выхода введите 'Q'): ");
                userName = Console.ReadLine();
                if (userName.ToLower() != "q")
                {
                    try
                    {
                        List<string> tweets = Service.GetTweets(userName, 5);
                        Dictionary<char, double> stats = Service.GetStatystics(abc, tweets);
                        string json = Service.DictionaryToJSON(stats);
                        Service.SendTweet(userName, json);
                        PrintStats(stats);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    exit = true;
                }
            }
        }

        //Простенький Шифратор/Дешифратор
        static string Encrypt(string _plaintext, int _key)
        {
            string result = "";
            for (int i = 0; i < _plaintext.Length; i++)
            {
                result += (char)((int)_plaintext[i] ^ _key);
            }
            return result;
        }

        //Генератор массива символов
        static char[] MakeABC(char _start, char _end)
        {
            _start = Char.ToUpper(_start);
            _end = Char.ToUpper(_end);

            string temp = "";
            if (_start < _end)
            {
                for (char letter = _start; letter <= _end; letter++)
                {
                    temp += letter;
                }
            }
            else
            {
                for (char letter = _start; letter >= _end; letter--)
                {
                    temp += letter;
                }
            }
            return temp.ToCharArray();
        }

        //Вывод JSON объекта на консоль
        static void PrintJSON(string _data)
        {
            Console.WriteLine("Статистика по данному пользователю:");
            foreach (char _c in _data.Skip(1))
            {
                if (_c != ',' && _c != '}')
                {
                    Console.Write(_c);
                }
                else
                {
                    Console.Write('\n');
                }
            }
        }

        //Вывод статистики на консоль
        static void PrintStats(Dictionary<char, double> stats)
        {
            Console.WriteLine("Статистика по данному пользователю:");
            foreach (var item in stats)
            {
                Console.WriteLine(item.Key.ToString() + ":'" + item.Value.ToString().Replace(",", ".") + "'\n");
            }
        }
    }
}