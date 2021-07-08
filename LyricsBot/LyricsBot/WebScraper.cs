using DiegoG.TelegramBot;
using DiegoG.TelegramBot.Types;
using DiegoG.Utilities;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace LyricsBot
{
    [BotCommand]
    public class WebScraper : Default
    {
        private const string BaseURL = "https://genius.com/amp/{0}";
        private static async Task<string> GetURL(string args)
            => string.Format(BaseURL, await Task.Run(() => Regex.Replace(args, @"[\s+]?[-][\s+]?", "-").ToLower()).AwaitWithTimeout(1000));

        public override async Task<(string, bool)> Action(BotCommandArguments args)
        {
            try
            {
                using WebClient MyWeb = new();

                string url;
                try
                {
                    url = await GetURL(args.ArgString);
                }
                catch (TimeoutException)
                {
                    return ("There was a problem parsing the request, please try again. Regex timed out.", false);
                }

                string Data = MyWeb.DownloadString(url);
                HtmlDocument doc = new();
                doc.LoadHtml(Data);

                Log.Information($"Searching for \"{args.ArgString}\" ({url})");
                var HeaderLyrics = doc.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                string Lyrics = HeaderLyrics.InnerText;
                Log.Verbose(Lyrics);

                return (Lyrics, false);
            }
            catch (Exception)
            {
                return ("Could not find the requested song/artist", false);
            }
        }

        public override Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args)
        {
            throw new NotImplementedException();
        }

        public override void Cancel(User user)
        {
            throw new NotImplementedException();
        }
    }
}
