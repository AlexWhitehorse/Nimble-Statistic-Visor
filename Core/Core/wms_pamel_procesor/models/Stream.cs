using System;
using System.Collections.Generic;
using System.Text;

namespace Core.wms_pamel_procesor.models
{
    class Stream
    {
        public string ID { get; set; }
        public string Application { get; set; }
        public string stream { get; set; }
        public string Status { get; set; }
        public string Protocol { get; set; }
        public string Video_codec { get; set; }
        public string Audio_codec { get; set; }
        public string Resolution { get; set; }

        public int Bandwidth { get; set; }
        public double Publish_time { get; set; }

    }
}
