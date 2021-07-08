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
        private const string BaseURL = "https://genius.com/amp/{0}-lyrics";
        private static async Task<string> GetURL(string args)
            => string.Format(BaseURL, await Task.Run(() => Regex.Replace(args, @"[\s+]?[-][\s+]? | \s?", "-").ToLower()).AwaitWithTimeout(1000));

        public override async Task<(string, bool)> Action(BotCommandArguments args)
        {
            try
            {
                Log.Information($"Attempting to find lyrics for user {args.User.FirstName}");
                using WebClient MyWeb = new();

                string url;
                try
                {
                    url = await GetURL(args.ArgString);
                }
                catch (TimeoutException e)
                {
                    Log.Error(e, "A TimeoutException ocurred while parsing the request");
                    return ("There was a problem parsing the request, please try again. Regex timed out.", false);
                }

                Log.Debug($"Downloading lyrics page from {url}");
                string Data = MyWeb.DownloadString(url);
                HtmlDocument doc = new();
                doc.LoadHtml(Data);

                Log.Debug($"Searching for \"{args.ArgString}\" ({url})");
                var HeaderLyrics = doc.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                string Lyrics = HeaderLyrics.InnerText;
                Log.Verbose($"Found:\n{Lyrics}");

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
