using System;
using System.Collections.Generic;
using System.Text;

namespace Core.wms_pamel_procesor
{
    public class Config
    {
        public string ClientId { get; set; }
        public string ApiKey { get; set; }
        public int BandwidthThreshold { get; set; }
        public double UptimeTreshold { get; set; }
        public Dictionary<int, int> TimeAlertsCheck { get; set; }
        public int TimezoneOffset { get; set; }


        public Config(string clientId, string apiKey)
        {
            ClientId = clientId;
            ApiKey = apiKey;
            TimeAlertsCheck = new Dictionary<int, int>();
        }


    }
}
