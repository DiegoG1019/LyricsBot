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

        public string? LyricsBotAPIKey { get; init; } = null;
        public long? LogOutputChannel { get; init; } = null;
    }
}
