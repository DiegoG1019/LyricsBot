using DiegoG.Utilities.Settings;
using DiegoG.TelegramBot;
using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

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
            Settings<BotSettings>.Initialize(ConfigDir, "BotSettings.cfg", false);
            if (Settings<BotSettings>.Current.LyricsBotAPIKey is null)
                throw new InvalidDataException
                    ($"Could not find a valid API Key for LyricsBot in the BotSettings file, fill it out in {Path.GetFullPath(Path.Combine(ConfigDir, "BotSettings.cfg.json"))}");

            {
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console()
                    .WriteTo.File(Path.Combine(ConfigDir, "logs", ".log"), rollingInterval: RollingInterval.Hour);

                var loc = Settings<BotSettings>.Current.LogOutputChannel;

                Log.Logger = logger.CreateLogger();
            }

            await LyricsBot.Init();

            Log.Information("==========================");
            Log.Information("Started LyricsBot");
            while (true)
                await Task.Delay(200); //It really doesn't have much else to do other than to wait for input, and everything else is done on background threads
        }
    }
}
