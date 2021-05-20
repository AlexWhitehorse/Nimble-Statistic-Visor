using System;
using System.Collections.Generic;
using System.Text;

namespace Core.alerts.models
{
    class StreamAlert : Alert
    {
        public StreamAlert(string application, string streamName, string serverName, string serverID) : base(application, streamName, serverName, serverID)
        {
        }
    }
}
