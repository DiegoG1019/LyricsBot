using DiegoG.TelegramBot;
using DiegoG.TelegramBot.Types;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace LyricsBot
{
    [BotCommand]
    public class WebScraper : Default
    {
        private static readonly string BaseURL = "https://genius.com/amp/";
        public static string Scrape(string artist, string song)
        {

            var NewUrl = Format(artist, song);
            try
            {
                using (WebClient MyWeb = new())
                {
                    string Data = MyWeb.DownloadString(NewUrl);
                    HtmlDocument doc = new();
                    doc.LoadHtml(Data);

                    Log.Information("Searching for" + song + " by " + artist);
                    var HeaderLyrics = doc.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                    string Lyrics = HeaderLyrics.InnerText;
                    Log.Verbose(Lyrics);
                    return Lyrics;
                }
            }
            catch (Exception)
            {
                return "Could not find the requested song/artist";
            }
        }

        private static string Format(string artist, string song)
        {
            artist = artist.ToLower();
            artist = artist.Replace(" ", "-");
            song = song.ToLower();
            song = song.Replace(" ", "-");
            string NewUrl = BaseURL + artist + "-" + song + "-lyrics";

            return NewUrl;
        }

        public override async Task<(string Result, bool Hold)> Action(BotCommandArguments args)
        {
            return Task.FromResult(Scrape());
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
