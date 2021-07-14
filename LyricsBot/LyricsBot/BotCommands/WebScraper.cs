using DiegoG.TelegramBot;
using DiegoG.TelegramBot.Types;
using DiegoG.Utilities;
using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        private const string GeniusBaseURL = "https://genius.com/amp/{0}-lyrics";
        private static async Task<string> GetGenius(string args)
            => string.Format(GeniusBaseURL, await Task.Run(() => Regex.Replace(args, @"[\s+]?[-][\s+]? | \s?", "-").ToLower().Replace("\"", "")).AwaitWithTimeout(1000));

        private const string MusixmatchBaseUrl = "https://www.musixmatch.com/lyrics/{0}/{1}";
        private static async Task<string> GetMusixmatch(string args)
        {
            var (artist, song) = await Task.Run(() =>
            {
                args = args.ToLower().Replace("\"", "");
                List<int> dashes = new(args.Length / 2);
                int count = 0;
                for (int i = 0; i < args.Length; i++)
                    if (args[i] is '-')
                    {
                        count++;
                        dashes.Add(i);
                    }

                if (count < 1)
                    throw new InvalidBotCommandArgumentsException(args, "Please tell me the name of the song like ArtistName - SongName");

                var song = args[dashes[count / 2]..].Trim();
                var artist = args[..dashes[count / 2]].Trim();

                return (artist.Replace(' ', '-').Replace("\"", ""), song.Replace(' ', '-').Replace("\"", ""));
            }).AwaitWithTimeout(1000);
            return string.Format(MusixmatchBaseUrl, artist, song);
        }

        private static readonly HtmlDocument HtmlDoc = new();

        private readonly TemporaryCache<string, string> LyricsCache = new(TimeSpan.FromMinutes(5));

        private bool FetchCache([NotNullWhen(true)] out string? lyrics, params string[] checks)
            => FetchCache(out lyrics, checks);
        private bool FetchCache([NotNullWhen(true)] out string? lyrics, IEnumerable<string> checks)
        {
            foreach(var s in checks)
                if(LyricsCache.ContainsKey(s))
                {
                    lyrics = LyricsCache[s];
                    return true;
                }
            lyrics = null;
            return false;
        }

        public override async Task<(string, bool)> Action(BotCommandArguments args)
        {
            var t = LyricsCache.CleanAsync();
            try
            {
                Log.Information($"Attempting to find lyrics for user {args.User}");

                List<string> urls = new();
                try
                {
                    {
                        var gt = GetGenius(args.ArgString);
                        urls.Add(await GetMusixmatch(args.ArgString));
                        urls.Add(await gt);
                    }

                    if (FetchCache(out string? lyrics, urls))
                    {
                        Log.Debug($"Retrieving lyrics from cache");
                        return (lyrics, false);
                    }
                    else
                    {
                        WebClient MyWeb = new();
                        foreach (var url in urls)
                        {
#warning This is Untested @Jal
                            try
                            {
                                Log.Debug($"Downloading lyrics page from {url}");
                                string Data = MyWeb.DownloadString(url);
                                HtmlDoc.LoadHtml(Data);
                                MyWeb.Dispose();
                                Log.Verbose($"Searching for \"{args.ArgString}\" ({url})");
                                var HeaderLyrics = HtmlDoc.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                                lyrics = HeaderLyrics.InnerText;
                                LyricsCache.Add(url, lyrics);
                                Log.Information($"Found Lyrics for user {args.User}");

                                MyWeb.Dispose();
                                return (lyrics, false);
                            }
                            catch (Exception)
                            {
                                Log.Verbose($"Could not find Lyrics for User {args.User} in {url}");
                            }
                        }

                        Log.Information($"Could not find Lyrics for User {args.User}");
                        
                        MyWeb.Dispose();
                        return ("Could not find the requested song/artist", false);
                    }
                }
                catch (TimeoutException e)
                {
                    Log.Error(e, "A TimeoutException ocurred while parsing the request");
                    return ("There was a problem parsing the request, please try again. Regex timed out.", false);
                }
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
