using System;
using System.Threading.Tasks;

namespace LyricsBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await LyricsBot.Init();

#error You ain't loading the configurations file

            while (true)
                await Task.Delay(200); //It really doesn't have much else to do other than to wait for input, and everything else is done on background threads
        }
    }
}
