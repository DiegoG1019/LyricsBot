using DiegoG.TelegramBot;
using DiegoG.TelegramBot.Types;
using DiegoG.Utilities;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace LyricsBot.BotCommands
{
    [BotCommand]
    public class WebScraper : Default
    {
        private const string BaseURL = "https://genius.com/amp/{0}-lyrics";
        private static async Task<string> GetURL(string args)
            => string.Format(BaseURL, await Task.Run(() => Regex.Replace(args, @"[\s+]?[-][\s+]? | \s?", "-").ToLower()).AwaitWithTimeout(1000));
        private static readonly HtmlDocument HtmlDoc = new();

        private readonly TemporaryCache<string, string> LyricsCache = new(TimeSpan.FromMinutes(5));

        public override async Task<(string, bool)> Action(BotCommandArguments args)
        {
            var t = LyricsCache.CleanAsync();
            try
            {
                Log.Information($"Attempting to find lyrics for user {args.User}");

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

                string lyrics;
                if (LyricsCache.ContainsKey(url))
                {
                    Log.Debug($"Retrieving {url} lyrics from cache");
                    lyrics = LyricsCache[url];
                }
                else
                {
                    Log.Debug($"Downloading lyrics page from {url}");
                    WebClient MyWeb = new();
                    string Data = MyWeb.DownloadString(url);
                    HtmlDoc.LoadHtml(Data);
                    MyWeb.Dispose();
                    Log.Verbose($"Searching for \"{args.ArgString}\" ({url})");
                    var HeaderLyrics = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                    lyrics = HeaderLyrics.InnerText;
                    LyricsCache.Add(url, lyrics);
                }

                Log.Information($"Found Lyrics for user {args.User}");

                return (lyrics, false);
            }
            catch (Exception)
            {
                Log.Information($"Could not find Lyrics for User {args.User}");
                return ("Could not find the requested song/artist", false);
            }
            finally
            {
                await t;
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
