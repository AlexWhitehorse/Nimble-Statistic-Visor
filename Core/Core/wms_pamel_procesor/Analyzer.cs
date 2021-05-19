using Core.wms_pamel_procesor.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.wms_pamel_procesor
{
    class Analyzer
    {
        private List<ServerStreams> serverStreams;
        //private List<ServerAlert> ServerAlerts;
        //private List<StreamAlert> StreamAlerts;
        private List<Alert> Alerts;

        public Analyzer (List<ServerStreams> Lss)
        {
            serverStreams = Lss;
            Alerts = new List<Alert>();
        }

        public void Analyze()
        {
            serverStreams.ForEach(x =>
            {
                if (CheckServer(x))
                {
                    x.Streams.ForEach(s =>
                    {
                        CheckStream(s, x.Name);
                    });
                }
            });
        }

        public List<Alert> getAlerts()
        {
            return Alerts;
        }

        private bool CheckServer(Server server)
        {
            if (!IsServerOnline(server))
                return false;
            return true;
        }

        private void CheckStream(Stream stream, string serName)
        {
            if(IsStreamOnline(stream, serName))
            {
                bandwodthCheck();
                uptimeCheck();
            }

        }

        private void bandwodthCheck()
        {
            return;
        }

        private void uptimeCheck()
        {
            return;
        }

        private bool IsStreamOnline(Stream stream, string serverName)
        {
            if(stream.Status != "online")
            {
                AddAlert(string.Format("\n{0}/{1}\nСервер: {2}\n", stream.Application, stream.stream, serverName));
                return false;
            }
            return true;
        }

        private bool IsServerOnline(Server server)
        {
            if (server.Status != "online")
            {
                AddAlert("Server OFFLINE:" + server.Name);
                return false;
            }
            return true;
        }

        private void AddAlert(string s)
        {
            Alerts.Add( new Alert(s) );
        }
    }
}
