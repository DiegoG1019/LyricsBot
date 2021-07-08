using System;
using TestingBot;
using GeniusScrapper;
namespace Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            OutOfChronologicalOrderBot Init = new();
            Init.Main();
        }
    }
}
