using System;
using System.Collections.Generic;
using System.Text;

namespace Core.wms_pamel_procesor.models
{
    class StreamAlert : Stream
    {
        public string description { get; set; }

        public StreamAlert(Stream s)
        {
            ID = s.ID;
            Application = s.Application;
            stream = s.stream;
            Status = s.Status;
            Protocol = s.Protocol;
            Video_codec = s.Video_codec;
            Audio_codec = s.Audio_codec;

            Resolution = s.Resolution;
            Bandwidth = s.Bandwidth;
            Publish_time = s.Publish_time;
        }

        public StreamAlert(Stream s, string desc) : this(s)
        {
            description = desc;
        }
    }
}
