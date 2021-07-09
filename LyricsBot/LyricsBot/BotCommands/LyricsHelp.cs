using DiegoG.TelegramBot;
using DiegoG.TelegramBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LyricsBot.BotCommands
{
    [BotCommand]
    public class LyricsHelp : Help
    {
        private static readonly string IntroMessage = "In order to request a song's lyrics, write: \"ArtistName - SongName\"\n\nAvailable Commands:\n";
        public override async Task<(string Result, bool Hold)> Action(BotCommandArguments args)
        {
            var (r, _) = await base.Action(args);
            return (IntroMessage + r, false);
        }
    }
}
