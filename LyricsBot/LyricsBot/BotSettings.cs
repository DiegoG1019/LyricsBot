using DiegoG.Utilities.Settings;
using System;
using System.ComponentModel;
using Telegram.Bot;

namespace LyricsBot
{
    public class BotSettings : ISettings
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string SettingsType => "LyricsBot.BotSettings";
        public ulong Version => 0;

        public string BotAPIKey { get; init; } = "1882098106:AAFQFYLC4bWYPCgamIIJh8VhpflXNN_DitA";
    }
}
