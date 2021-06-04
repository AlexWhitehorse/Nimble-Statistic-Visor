
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
        static Config config = new Config("", "");

        static void Main(string[] args)
        {
            config.BandwidthThreshold = 20000;
            config.UptimeTreshold = 15;
            config.TimezoneOffset = -2;

            config.TimeAlertsCheck.Add(8,30);
            config.TimeAlertsCheck.Add(12,0);
            config.TimeAlertsCheck.Add(15,30);
            config.TimeAlertsCheck.Add(19,0);
            config.TimeAlertsCheck.Add(22,30);

            Console.WriteLine("" +
                "Starting an app..\n" +
                "Release version 1.3.1" +
                "");

            TelegramBot Bot = new TelegramBot("");

            Bot.AddConfig(config);
            Bot.Start();
        }
    }
}
