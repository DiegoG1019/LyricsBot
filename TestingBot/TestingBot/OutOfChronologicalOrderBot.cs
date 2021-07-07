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
            MyClient.OnCallbackQuery += CallBackQueryRead;

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
                                                    $" ArtistName-SongName");
                        return;
                     }

                    if(Message.Text.Contains('-'))
                    {
                        await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                        $"Looking for lyrics...");

                        var _Message = Message.Text.Split('-');
                        var Response = WebScrapper.Scrap(_Message[0], _Message[1]);

                        await MyClient.SendTextMessageAsync(Message.Chat.Id, Response);

                        if (Response == "Couldn't find the song or the artist")
                        {
                            await MyClient.SendTextMessageAsync(Message.Chat.Id, 
                                                                "Remember to not type a backspace between the names and the dash");
                        return;
                        }
                    return;
                    }
                else
                {
                    await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                                "Remember to not type a backspace between the names and the dash");
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

        [Obsolete]
        static async void CallBackQueryRead(object sender, CallbackQueryEventArgs CBEvent)
        {
            if (CBEvent.CallbackQuery.Data.Equals("eat"))
            {
                await MyClient.SendTextMessageAsync(CBEvent.CallbackQuery.Message.Chat.Id,
                                                $"Come algo pana {CBEvent.CallbackQuery.Message.Chat.Username}");
            }
        }
    }
}
