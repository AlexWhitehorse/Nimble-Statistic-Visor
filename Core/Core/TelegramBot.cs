using Core.wms_pamel_procesor;
using Core.wms_pamel_procesor.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Telegram.Bot;

namespace Core
{
    class TelegramBot
    {
        private TelegramBotClient bot;
        private Config config;

        public TelegramBot(string token)
        {
            bot = new TelegramBotClient(token);
        }

        public void AddConfig(Config config)
        {
            this.config = config;
        }

        public void Start()
        {
            AddMessageHendlers();
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();
        }

        private void AddMessageHendlers()
        {
            bot.OnMessage += HandlerBotOnMessage;
        }


        private void HandlerBotOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (e.Message.Text == "/g")
                {
                    SenMessageToUserAsync(e, "Wait a few seconds...");

                    try
                    {
                        var analyzer = GetAnalyzerResult();

                        string result = MakeResultStringAlerts("Не рабочие потоки:", analyzer.getAlerts());
                        string bandwodthAlerts = MakeResultStringAlerts("Битрейт ниже " + config.BandwidthThreshold + ":", analyzer.GetBitrateAlerts());
                        string uptimeAlerts = MakeResultStringAlerts("Uptime потока ниже " + config.UptimeTreshold + "мин. :", analyzer.GetUptimeAlerts());


                        SenMessageToUserAsync(e, bandwodthAlerts);
                        SenMessageToUserAsync(e, uptimeAlerts);
                        SenMessageToUserAsync(e, result);
                    }
                    catch (Exception es)
                    {
                        SenMessageToUserAsync(e, es.Message);
                    }
                }
                else
                {
                    SenMessageToUserAsync(e, "Help: \"/g\"");
                }
            }
        }

        private Connector CreateAPIConndectr()
        {
            return new Connector(config);
        }

        private void SenMessageToUserAsync(Telegram.Bot.Args.MessageEventArgs e, string text)
        {
            bot.SendTextMessageAsync(e.Message.Chat.Id, text, Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        private Analyzer GetAnalyzerResult()
        {
            try {
                List<ServerStreams> serverStreams = CreateAPIConndectr()
                                                    .GetAllServerStreamsFromApi();
                Analyzer anal = new Analyzer(config, serverStreams);
                anal.Analyze();

                return anal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private string MakeResultStringAlerts<T>(string initMSG, List<T> alerts) where T : Alert
        {
            if (alerts.Count == 0)
            {
                return "Все потоки работают";
            }

            string result = initMSG + "\n\n";
            for (int i = 0; i < alerts.Count; i++)
            {
                result += alerts[i].sence;
            }
            return result;
        }
    }
}
