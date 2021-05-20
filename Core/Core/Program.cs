
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
        static Config config = new Config("94089d98-b2a0-40e1-8732-094fbefe685c", "7227020af0181ba045ee7a528ff14637");

        static void Main(string[] args)
        {
            config.BandwidthThreshold = 10000;
            config.UptimeTreshold = 30;

            Console.WriteLine("" +
                "Starting an app..\n" +
                "Version 1.2.0" +
                "");

            TelegramBot Bot = new TelegramBot("1822189604:AAFN_IacONeRdsayO5X9JnQkVvNzxpXGZdA");
            Bot.AddConfig(config);
            Bot.Start();
        }
    }
}
