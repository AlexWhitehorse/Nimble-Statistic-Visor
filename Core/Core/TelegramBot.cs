using Core.alerts;
using Core.alerts.models;
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
        private TelegramBotClient Bot;
        private Config config;

        public TelegramBot(string token)
        {
            Bot = new TelegramBotClient(token);
        }

        public void AddConfig(Config config)
        {
            this.config = config;
        }

        public void Start()
        {
            AddMessageHendlers();
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private void AddMessageHendlers()
        {
            Bot.OnMessage += HandlerBotOnMessage;
        }


        private void HandlerBotOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (e.Message.Text == "/all")
                {
                    SenMessageToUserAsync(e, "Wait a few seconds...");

                    try
                    {
                        var analyzer = GetAnalyzerResult();
                        Alerts alerts = analyzer.getAlerts();

                        var uptimeAlerts = alerts.GetUptimeAlerts();
                        var streamAlerts = alerts.GetStreamAlerts();
                        var lowBitrateAlerts = alerts.GetLowBitrateAlert();

                        SenMessageToUserAsync(e, 
                            MakeUptimeAlertMessage("Uptime потока ниже " + config.UptimeTreshold + "мин. :", uptimeAlerts)
                            );
                        SenMessageToUserAsync(e, 
                            MakeStreamAlertMessage("Битрейт ниже " + config.BandwidthThreshold + ":", lowBitrateAlerts)
                            );
                        SenMessageToUserAsync(e,
                            MakeStreamAlertMessage("Не рабочие потоки:", streamAlerts)
                            );
                    }
                    catch (Exception es)
                    {
                        SenMessageToUserAsync(e, es.Message);
                    }
                }
                else
                {
                    SenMessageToUserAsync(e,
                        "Help: \n" +
                        "/all - Вывод всей информиции о потоках" +
                        "\n"
                    );
                }
            }
        }

        private Connector CreateAPIConndectr()
        {
            return new Connector(config);
        }

        private void SenMessageToUserAsync(Telegram.Bot.Args.MessageEventArgs e, string text)
        {
            Bot.SendTextMessageAsync(e.Message.Chat.Id, text, Telegram.Bot.Types.Enums.ParseMode.Html);
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

        private string MakeStreamAlertMessage<T>(string initMsg, List<T> alerts) where T : Alert
        {
            if(alerts.Count == 0)
            {
                return "<b>" + initMsg + "</b>\n\nВсе потоки работают";
            }
            string result = "<b>" + initMsg + "</b>\n\n";

            alerts.ForEach(x =>
            {
                result += string.Format(
                    "<a href=\"https://wmspanel.com/nimble_live_streams/outgoing/{0}\">{1}/{2}</a> \n      <i>Сервер: {3}</i>\n",
                    x.ServerID,
                    x.Application,
                    x.StreamName,
                    x.ServerName
                    );
            });

            return result;
        }

        private string MakeUptimeAlertMessage(string initMsg, List<alerts.models.UptimeAlert> alerts)
        {
            if (alerts.Count == 0)
            {
                return "< b >" + initMsg + " </ b >\n\n Всё хорошо!)";
            }
            string result = "<b>" + initMsg + "</b>\n\n";

            alerts.ForEach(x =>
            {
                result += string.Format(
                    "<a href=\"https://wmspanel.com/nimble_live_streams/outgoing/{0}\">{1}/{2}</a> {3} мин;\n      <i>Сервер: {4}</i>\n",
                    x.ServerID,
                    x.Application,
                    x.StreamName,
                    x.uptime,
                    x.ServerName
                    );
            });

            return result;
        }
    }
}
