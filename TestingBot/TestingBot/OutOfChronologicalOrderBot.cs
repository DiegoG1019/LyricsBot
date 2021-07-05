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

namespace TestingBot
{
    public class OutOfChronologicalOrderBot : IBot
    {
        private static readonly TelegramBotClient MyClient = ClientInitializer.InitClient();
        private static Queue<MessageEventArgs> MyQueue = new();

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
            if (MyQueue.Count == 0)
                MyQueue.Enqueue(ArgEvent);

               var MyBtn = new InlineKeyboardButton();
               MyBtn.Text = "I'm Hungry";
               MyBtn.CallbackData = "eat";

               var MyKeyboard = new InlineKeyboardMarkup(MyBtn);
            try
            {
                var Message = MyQueue.Dequeue().Message;
                if (Message == null || Message.Type != MessageType.Text)
                    return;

                await MyClient.SendTextMessageAsync(Message.Chat.Id,
                                                    $"Succioname el pito {Message.Chat.Username}",
                                                    replyMarkup: MyKeyboard);
            }
            catch (Exception)
            {
                MyQueue.Enqueue(ArgEvent);
                await MyClient.SendTextMessageAsync(ArgEvent.Message.Chat.Id,
                                                    $"Muchas peticiones espere un momento.");
                Thread.Sleep(1000);
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
