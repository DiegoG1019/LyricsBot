using DiegoG.Utilities.Settings;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LyricsBot
{
    public class Program
    {
        public readonly static string ConfigDir = Environment.GetEnvironmentVariable("ConfigDir") ?? (
            OperatingSystem.IsLinux() ? "~/.config/LyricsBot" :
            OperatingSystem.IsWindows() ? "configurations/Lyrics" :
            throw new InvalidOperationException("No \"ConfigDir\" Environment Variable provided, and the current platform is not supported for a default location")
            );


        static async Task Main(string[] args)
        {
            Settings<BotSettings>.Initialize(ConfigDir, "BotSettings.cfg");
            if (Settings<BotSettings>.Current.LyricsBotAPIKey is null)
                throw new InvalidDataException
                    ($"Could not find a valid API Key for LyricsBot in the BotSettings file, fill it out in {Path.GetFullPath(Path.Combine(ConfigDir, "BotSettings.cfg.json"))}");

            await LyricsBot.Init();

            while (true)
                await Task.Delay(200); //It really doesn't have much else to do other than to wait for input, and everything else is done on background threads
        }
    }
}
