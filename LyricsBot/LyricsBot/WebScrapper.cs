using HtmlAgilityPack;
using System;
using System.Net;
namespace GeniusScrapper
{
    public class WebScrapper
    {
        private static readonly string BaseURL = "https://genius.com/amp/";
        public static string Scrap(string artist, string song)
        {

            var NewUrl = Format(artist, song);
            try
            {
                using (WebClient MyWeb = new())
                {
                    string Data = MyWeb.DownloadString(NewUrl);
                    HtmlDocument doc = new();
                    doc.LoadHtml(Data);

                    Console.WriteLine("Looking for lyrics...");
                    Console.WriteLine("Searching " + song + " by " + artist);
                    var HeaderLyrics = doc.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                    string Lyrics = HeaderLyrics.InnerText;
                    Console.WriteLine(Lyrics);
                    return Lyrics;
                }
            }
            catch (Exception)
            {
                return "Couldn't find the song or the artist";
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
    }
}
