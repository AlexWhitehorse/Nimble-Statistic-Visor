using System;
using System.Collections.Generic;
using System.Text;

namespace Core.wms_pamel_procesor.models
{
    class ServerStreams : Server
    {
        public List<Stream> Streams { get; set; }

        public ServerStreams()
        {
            Streams = new List<Stream>();
        }
    }
}
