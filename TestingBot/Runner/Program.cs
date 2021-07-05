using System;
using TestingBot;
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
