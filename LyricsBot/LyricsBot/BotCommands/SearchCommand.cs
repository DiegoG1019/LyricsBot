using DiegoG.TelegramBot;
using DiegoG.TelegramBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace LyricsBot.BotCommands
{
#error Es aqui la verga @Jal

    //[BotCommand]
    class SearchCommand : IBotCommand
    {
        public BotCommandProcessor Processor { get; set; }

//You can delete these
#error Write The things

        public string HelpExplanation => "Write something here";


        public string HelpUsage => "Write something here";

        public IEnumerable<(string Option, string Explanation)>? HelpOptions => new (string, string)[]
        {
            ("OptionName", "Explain like I'm 5"),
            //Add more here, if necessary
        };

        public string Trigger => "/search";

        public string? Alias => null; //You can also set something here, usually a shorter version of /search, like /s? Purely optional. Beware of trigger collisions

        //the "Hold" portion of the tuple is to tell the processor to hold this command for a conversation to that specific user. Usually, you'd keep your own User Dictionary to maintain a state inside this class. The Processor will tell you which user it is, but nothing else. But you probably won't need the Hold, so set it to false each time you return

        public Task<(string Result, bool Hold)> Action(BotCommandArguments args)
        {
            throw new NotImplementedException();
        }

        public Task<(string Result, bool Hold)> ActionReply(BotCommandArguments args)
        {
            throw new NotImplementedException(); //If you're not gonna need to hold a conversation, you can leave this one as is. If it throws this exception, you did something wrong somewhere else. 
        }

        //This is for clearing any state info you may have in here, and it's called manually. Usually for when you Hold a conversation

        public void Cancel(User user)
        {
            throw new NotImplementedException();
        }
    }
}
