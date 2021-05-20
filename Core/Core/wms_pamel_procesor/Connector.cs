using Core.wms_pamel_procesor.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.wms_pamel_procesor
{
    class Connector
    {
        private Config _config;

        public Connector(Config conf)
        {
            _config = conf;
        }

        /// <summary>
        /// Gets information of servers by API
        /// Gets information about streams and returns All servers with its streams
        /// </summary>
        /// <returns></returns>
        public List<ServerStreams> GetAllServerStreamsFromApi()
        {
            List<Server> Servers = GetServersListFromApi().Result;
            List<ServerStreams> serverStreams = ToListServerStreams(Servers);

            List<Task<List<Stream>>> tasks = new List<Task<List<Stream>>>();

            for (int i = 0; i < serverStreams.Count; i++)
            {
                var ServerID = serverStreams[i].ID;
                var task = CreateTaskGetingLiveStreams(ServerID);
                tasks.Add(task);
            }

            var arrTasks = tasks.ToArray();
            Task.WaitAll(arrTasks);

            for (int i = 0; i < serverStreams.Count; i++)
            {
                var task = arrTasks[i];
                var result = GetResultTask(task);
                serverStreams[i].Streams = result;
            }
            return serverStreams;
        }

        private List<Stream> GetResultTask(Task<List<Stream>> task)
        {
            return task.Result;
        }

        private async Task<List<Stream>> CreateTaskGetingLiveStreams(string ServerID)
        {
            return await Task.Run(()=> GetLiveStreamsOnServerFromApi(ServerID));
        }

        private List<Stream> GetLiveStreamsOnServerFromApi(string serverId)
        {
            string url = AddAuthDataToApiLink("https://api.wmspanel.com/v1/server/" + serverId + "/live/streams");
            string responseBody = SenRequestToWMSApi(url).Result;

            var kvpJson = JsonConvert.DeserializeObject<Dictionary<string, Object>>(responseBody);
            var streamsJson = kvpJson["streams"].ToString();

            return JsonConvert.DeserializeObject<List<Stream>>(streamsJson);

        }

        /// <summary>
        /// Send a GET request to WMSPanel.
        /// Geting list of servers of WMSPanel.
        /// </summary>
        /// <returns></returns>
        private async Task<List<Server>> GetServersListFromApi()
        {
            string url = AddAuthDataToApiLink("https://api.wmspanel.com/v1/server");

            string responseBody = await SenRequestToWMSApi(url);

            var kvpJson = JsonConvert.DeserializeObject<Dictionary<string, Object>>(responseBody);
            var serversJson = kvpJson["servers"].ToString();
            return JsonConvert.DeserializeObject<List<Server>>(serversJson);
        }

        private List<ServerStreams> ToListServerStreams(List<Server> servers)
        {
            List<ServerStreams> ServerStreams = new List<ServerStreams>();
            servers.ForEach(x =>
            {
                var ss = new ServerStreams();

                ss.ID = x.ID;
                ss.Name = x.Name;
                ss.Kind = x.Kind;
                ss.Status = x.Status;
                ss.IP = x.IP;
                ss.Custom_ips = x.Custom_ips;

                ServerStreams.Add(ss);
            });

            return ServerStreams;
        }

        /// <summary>
        /// Throws exceptoin if WMS pannwel is not configured
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<string> SenRequestToWMSApi(string url)
        {
            try
            {
                HttpClient client = new HttpClient();
                string responseBody = await client.GetStringAsync(url);
                return responseBody;
            }
            catch (HttpRequestException)
            {
                throw new Exception("HttpRequestException: Can not get access to WMS pannel API(Check allowed hosts in pannel)");
            }
        }

        private string AddAuthDataToApiLink(string ConnString)
        {
            if (ConnString.Contains("?"))
                return string.Format("{0}client_id={1}&api_key={2}", ConnString, _config.ClientId, _config.ApiKey);
            else
                return string.Format("{0}?client_id={1}&api_key={2}", ConnString, _config.ClientId, _config.ApiKey);
        }
    }


}
