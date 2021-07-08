using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Serilog;
using DiegoG.TelegramBot;
using DiegoG.Utilities.Settings;

namespace LyricsBot
{
    public static class LyricsBot
    {
        public static BotCommandProcessor Processor { get; private set; }

        public static async Task Init()
        {
            Log.Information("Initializing LyricsBot");
            var bot = new TelegramBotClient(Settings<BotSettings>.Current.LyricsBotAPIKey);
            Log.Information($"Connected to {await bot.GetMeAsync()}");

            Processor = new(bot);

            bot.StartReceiving(new UpdateType[] { UpdateType.Message });

            AppDomain.CurrentDomain.ProcessExit += (s, a) =>
            {
                Log.Information("Stopping LyricsBot");
                bot.StopReceiving();
                Processor.MessageQueue.Stop();
                Log.Information("Stopped LyricsBot");
            };
        }
    }
}
