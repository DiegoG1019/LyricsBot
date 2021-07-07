using System;
using TestingBot;
using GeniusScrapper;
namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(WebScrapper.Scrap("trevor daniel", "xd"));
            OutOfChronologicalOrderBot Init = new();
            Init.Main();
        }
    }
}
