using System;
using System.Collections.Generic;
using System.Text;

namespace Core.alerts.models
{
    class Alert
    {
        public Alert(string application, string streamName, string serverName, string serverID)
        {
            Application = application;
            StreamName = streamName;
            ServerName = serverName;
            ServerID = serverID;
        }

        public string Application { get; set; }
        public string StreamName { get; set; }
        public string ServerName { get; set; }
        public string ServerID { get; set; }
    }
}
