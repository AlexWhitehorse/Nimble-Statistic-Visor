
using Core.wms_pamel_procesor;
using Core.wms_pamel_procesor.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Telegram.Bot;

namespace Core
{
    class Program
    {
        static string botToken = "1822189604:AAFN_IacONeRdsayO5X9JnQkVvNzxpXGZdA";
        static TelegramBotClient bot = new TelegramBotClient(botToken);

        static Config config = new Config("94089d98-b2a0-40e1-8732-094fbefe685c", "7227020af0181ba045ee7a528ff14637");
        static Connector connector = new Connector(config);

        static void Main(string[] args)
        {
            bot.StartReceiving();
            bot.OnMessage += BotOnMessage;
            Console.ReadLine();
            bot.StopReceiving();

        }

        private static void BotOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if(e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (e.Message.Text == "/g")
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Begining...");

                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    List<ServerStreams> serverStreams = connector.GetAllServerStreamsFromApi();
                    
                    

                    Analyzer anal = new Analyzer(serverStreams);
                    anal.Analyze();
                    List<Alert> alerts = anal.getAlerts();

                    string result = "Не рабочие потоки:\n";
                    for (int i = 0; i < alerts.Count; i++)
                    {
                        //bot.SendTextMessageAsync(e.Message.Chat.Id, alerts[i].sence);
                        result += alerts[i].sence;
                    }
                    bot.SendTextMessageAsync(e.Message.Chat.Id, result);
                }
                else
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Help: \"/g\"");
                }
            }
        }
    }
}
