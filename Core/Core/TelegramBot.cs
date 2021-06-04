using Core.alerts;
using Core.alerts.models;
using Core.wms_pamel_procesor;
using Core.wms_pamel_procesor.models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Core
{
    class TelegramBot
    {
        private TelegramBotClient Bot;
        private Config config;

        private List<long> Chats;

        public TelegramBot(string token)
        {
            Bot = new TelegramBotClient(token);
            Chats = new List<long>();
        }

        public void AddConfig(Config config)
        {
            this.config = config;
        }

        public void Start()
        {
            AddMessageHendlers();
            Bot.StartReceiving();

            // Запуск уведомлений по расписанию для пользователей
            AlertsByTime().Start();

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private void AddMessageHendlers()
        {
            Bot.OnMessage += HandlerBotOnMessage;
        }

        // Отправка сообщений по расписанию в config (точность 5 минут)
        private Task AlertsByTime()
        {
            while(true)
            {
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute;

                if (compareTime(config.TimeAlertsCheck, hour, minute)) {
                    Chats.ForEach(chatId =>
                    {
                        MakeCheckupAndSendMessagesToUser(chatId);
                    });
                }
                Thread.Sleep(300000);
            }
        }

        private bool compareTime(Dictionary<int, int> setTime, int hoursNow, int minNow)
        {
            int hoursTimezone = hoursNow - config.TimezoneOffset;

            if (setTime.ContainsKey(hoursTimezone))
            {
                int maxMinutes = setTime[hoursTimezone] + 5;
                int minMinutes = setTime[hoursTimezone] - 2;

                if (maxMinutes >= minNow &&
                    minMinutes <= minNow)
                {
                    return true;
                }
            }
            return false;
        }

        private void HandlerBotOnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                long chatId = getTelegramChatId(e);
                string message = e.Message.Text;

                if (message == "/all")
                {
                    SenMessageToUserAsync(chatId, "Wait a few seconds...");

                    try
                    {
                        MakeCheckupAndSendMessagesToUser(chatId);
                    }
                    catch (Exception es)
                    {
                        SenMessageToUserAsync(chatId, es.Message);
                    }
                }
                else if (message == "/subscribe")
                {
                    if (!Chats.Contains(chatId))
                    {
                        Chats.Add(chatId);
                        SenMessageToUserAsync(chatId, "Вы подписальсь на плновую проверку!");
                    }
                    else
                    {
                        SenMessageToUserAsync(chatId, "Вы уже подписаны на плановую проверку");
                    }
                }
                else if (message == "/unsubscribe")
                {
                    if (Chats.Remove(chatId))
                    {
                        SenMessageToUserAsync(chatId, "Вы отписались от плановой проверки");
                    }
                }
                else
                {
                    SenMessageToUserAsync(chatId,
                        "Help: \n" +
                        "/all - Вывод всей информиции о потоках\n" +
                        "/subscribe - Уведомления о плановой проверке \n" +
                        "/unsubscribe" +
                        "\n"
                    );
                }
            }
        }

        private long getTelegramChatId(Telegram.Bot.Args.MessageEventArgs e)
        {
            return e.Message.Chat.Id;
        }

        private void MakeCheckupAndSendMessagesToUser(long chatId)
        {
            var analyzer = GetAnalyzerResult();
            Alerts alerts = analyzer.getAlerts();

            var uptimeAlerts = alerts.GetUptimeAlerts();
            var streamAlerts = alerts.GetStreamAlerts();
            var lowBitrateAlerts = alerts.GetLowBitrateAlert();

            SenMessageToUserAsync(chatId,
                MakeUptimeAlertMessage("Uptime потока ниже " + config.UptimeTreshold + "мин. :", uptimeAlerts)
                );
            SenMessageToUserAsync(chatId,
                MakeStreamAlertMessage("Битрейт ниже " + config.BandwidthThreshold + ":", lowBitrateAlerts)
                );
            SenMessageToUserAsync(chatId,
                MakeStreamAlertMessage("Не рабочие потоки:", streamAlerts)
                );
        }

        private Task AllertMessageSender()
        {
            throw new NotImplementedException();
        }

        private Connector CreateAPIConndectr()
        {
            return new Connector(config);
        }

        private void SenMessageToUserAsync(long chatId, string text)
        {
            Bot.SendTextMessageAsync(chatId, text, Telegram.Bot.Types.Enums.ParseMode.Html);
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

        private string MakeUptimeAlertMessage(string initMsg, List<UptimeAlert> alerts)
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
