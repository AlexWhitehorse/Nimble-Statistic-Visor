using Core.alerts;
using Core.wms_pamel_procesor.models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.wms_pamel_procesor
{
    class Analyzer
    {
        private List<ServerStreams> serverStreams;

        private Config config;
        private Alerts alerts; 


        public Analyzer (Config conf, List<ServerStreams> Lss)
        {
            config = conf;
            serverStreams = Lss;

            alerts = new Alerts();
        }

        public void Analyze()
        {
            serverStreams.ForEach(x =>
            {
                if (CheckServer(x))
                {
                    x.Streams.ForEach(s =>
                    {
                        CheckStream(s, x.Name, x.ID);
                    });
                }
            });
        }

        public Alerts getAlerts()
        {
            return alerts;
        }

        private bool CheckServer(Server server)
        {
            if (!IsServerOnline(server))
                return false;
            return true;
        }

        private void CheckStream(Stream stream, string serName, string serId)
        {
            if(IsStreamOnline(stream, serName, serId))
            {
                BandwodthCheck(stream, serName, serId);
                UptimeCheck(stream, serName, serId);
            }

        }

        private void BandwodthCheck(Stream stream, string serverName, string serverID)
        {
            if(stream.Bandwidth < config.BandwidthThreshold)
            {
                alerts.AddLowBitrateAlert(
                    stream.Application,
                    stream.stream,
                    serverName,
                    serverID
                    );
            }
        }

        private void UptimeCheck(Stream stream, string serverName, string serverID)
        {
            DateTime thisDate = DateTime.Now;
            DateTime StreamDate = UnixTimeStampToDatetime(stream.Publish_time);
            var comparison = (thisDate - StreamDate).TotalMinutes;

            if(comparison < config.UptimeTreshold)
            {
                alerts.AddUptimeAlert(
                    stream.Application, 
                    stream.stream, 
                    serverName, 
                    serverID, 
                    comparison.ToString().Substring(0, 2).Trim(','));
            }
        }

        private DateTime UnixTimeStampToDatetime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private bool IsStreamOnline(Stream stream, string serverName, string serverID)
        {
            if(stream.Status != "online")
            {
                alerts.AddStreamAlert(
                    stream.Application,
                    stream.stream,
                    serverName,
                    serverID
                    );


                return false;
            }
            return true;
        }

        private bool IsServerOnline(Server server)
        {
            if (server.Status != "online")
            {
                //AddAlert("Server OFFLINE:" + server.Name);
                return false;
            }
            return true;
        }
    }
}
