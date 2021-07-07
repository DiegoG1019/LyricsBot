using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using GeniusScrapper;

namespace TestingBot
{
    public class OutOfChronologicalOrderBot
    {
        private static readonly TelegramBotClient MyClient = ClientInitializer.InitClient();
        private static Queue<MessageEventArgs> MyQueue = new();
        private static bool Loop = true;

        [Obsolete]
        public void Main()
        {
            var MyBot = MyClient.GetMeAsync().Result;
            Console.WriteLine($"Connected to {MyBot.Username}");

            MyClient.OnMessage += ReadMessagesEvent;
            MyClient.OnReceiveError += ErrorReadEvent;

            MyClient.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine("Listening Messages...");
            Console.ReadKey();
            MyClient.StopReceiving();
        }

        [Obsolete]
        static async void ReadMessagesEvent(object sender, MessageEventArgs ArgEvent)
        {

                MyQueue.Enqueue(ArgEvent);

                try
                {
                    var Message = MyQueue.Dequeue().Message;

                    if (Message == null || Message.Type != MessageType.Text)
                        return;

                     if(Message.Text.Split(' ').First() == "/help")
                     {
                            await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                    $"Send me a message with this format:");
                            await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                    $" ArtistName - SongName");
                    await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                   $"If the song have two artist use (and) between their names, if it has more than two" +
                                                   $" type every artist between spaces and put (and) to name the last one. " +
                                                   $"Remember to type them in order.");
                    return;
                     }

                    if(Message.Text.Contains('-'))
                    {
                        await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                        $"Looking for lyrics...");

                        var _Message = Message.Text.Split(" - ");
                        var Response = WebScrapper.Scrap(_Message[0], _Message[1]);

                        if (Response == "Couldn't find the song or the artist")
                        {
                            await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                                "Remember to type a backspace between the names and the dash, " +
                                                                "and don't use especial characters.");
                        }


                        await MyClient.SendTextMessageAsync(Message.Chat.Id, Response);
                        return;
                    }

                else
                {
                    await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                                $"Use /help for more information");
                    return;
                }

                }
                catch (Exception)
                {
                    MyQueue.Enqueue(ArgEvent);
                }
        }

        [Obsolete]
        static void ErrorReadEvent(object sender, ReceiveErrorEventArgs ArgEvent)
        {
            Console.WriteLine("Error Recibido: {0} - {1}", 
                              ArgEvent.ApiRequestException.ErrorCode, 
                              ArgEvent.ApiRequestException.Message);
        }
    }
}
