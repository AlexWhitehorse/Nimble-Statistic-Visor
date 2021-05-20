using Core.alerts.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.alerts
{
    class Alerts
    {
        private List<StreamAlert> streamAlert;
        private List<UptimeAlert> uptimeAlert;
        private List<LowBitrateAlert> lowBitrateAlert;

        public Alerts()
        {
            streamAlert = new List<StreamAlert>();
            uptimeAlert = new List<UptimeAlert>();
            lowBitrateAlert = new List<LowBitrateAlert>();
        }

        public void AddStreamAlert(string application, string streamName, string serverName, string serverID)
        {
            streamAlert.Add(new StreamAlert(application, streamName, serverName, serverID));
        }

        public void AddUptimeAlert(string application, string streamName, string serverName, string serverID, string uptime)
        {
            uptimeAlert.Add(new UptimeAlert(application, streamName, serverName, serverID, uptime));
        }

        public void AddLowBitrateAlert(string application, string streamName, string serverName, string serverID)
        {
            lowBitrateAlert.Add(new LowBitrateAlert(application, streamName, serverName, serverID));
        }


        public List<UptimeAlert> GetUptimeAlerts()
        {
            return uptimeAlert;
        }

        public List<StreamAlert> GetStreamAlerts()
        {
            return streamAlert;
        }

        public List<LowBitrateAlert> GetLowBitrateAlert()
        {
            return lowBitrateAlert;
        }
    }
}
