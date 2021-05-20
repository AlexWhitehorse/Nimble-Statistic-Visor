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

        //Alert lists
        private List<Alert> Alerts; // Aletrs streams OFFLINE
        private List<LowBitrateAlert> lowBitrateAlertlerts;
        private List<UptimeAlert> uptimeAlerts;


        public Analyzer (Config conf, List<ServerStreams> Lss)
        {
            config = conf;
            serverStreams = Lss;

            Alerts = new List<Alert>();
            lowBitrateAlertlerts = new List<LowBitrateAlert>();
            uptimeAlerts = new List<UptimeAlert>();
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

        public List<Alert> getAlerts()
        {
            return Alerts;
        }

        public List<LowBitrateAlert> GetBitrateAlerts()
        {
            return lowBitrateAlertlerts;
        }

        public List<UptimeAlert> GetUptimeAlerts()
        {
            return uptimeAlerts;
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
                AddLowBitrateAllert(
                    FormatToOutString(stream, serverName, serverID)
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
                AddUptimeAlert(
                    FormatToOutString(stream, serverName, serverID, comparison.ToString().Substring(0,2).Trim(',') + " мин")
                    );
            }
        }

        private DateTime UnixTimeStampToDatetime(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private bool IsStreamOnline(Stream stream, string serverName, string severID)
        {
            if(stream.Status != "online")
            {
                AddAlert(
                    FormatToOutString(stream, serverName, severID)
                    );
                return false;
            }
            return true;
        }

        private string FormatToOutString(Stream stream, string serverName, string severID, string addData = "")
        {
            return string.Format("<a href=\"https://wmspanel.com/nimble_live_streams/outgoing/{3}\">{0}/{1}</a> {4}\n      <i>Сервер: {2}</i>\n",
                    stream.Application, stream.stream, serverName, severID, addData);
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

        private void AddUptimeAlert(string s)
        {
            uptimeAlerts.Add(new UptimeAlert(s));
        }

        private void AddLowBitrateAllert(string s)
        {
            lowBitrateAlertlerts.Add(new LowBitrateAlert(s));
        }

        private void AddAlert(string s)
        {
            Alerts.Add( new Alert(s) );
        }
    }
}
