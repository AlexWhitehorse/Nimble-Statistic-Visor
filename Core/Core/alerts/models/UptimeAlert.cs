using System;
using System.Collections.Generic;
using System.Text;

namespace Core.alerts.models
{
    class UptimeAlert : Alert
    {
        public string uptime { get; set; }

        public UptimeAlert(string application, string streamName, string serverName, string serverID, string uptime) : base(application, streamName, serverName, serverID)
        {
            this.uptime = uptime;
        }
    }
}
