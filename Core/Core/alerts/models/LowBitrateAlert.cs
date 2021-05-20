using System;
using System.Collections.Generic;
using System.Text;

namespace Core.alerts.models
{
    class LowBitrateAlert : Alert
    {
        public LowBitrateAlert(string application, string streamName, string serverName, string serverID) : base(application, streamName, serverName, serverID)
        {
        }
    }
}
