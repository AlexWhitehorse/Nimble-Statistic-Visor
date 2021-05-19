using System;
using System.Collections.Generic;
using System.Text;

namespace Core.wms_pamel_procesor.models
{
    class Server
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public string Status { get; set; }
        public List<string> IP;
        public List<string> Custom_ips { get; set; }
    }
}
