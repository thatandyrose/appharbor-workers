using System;
using Rappers.DaData;

namespace Workers.Models
{
    public class PingerModel : BaseEntity
    {
        public string Url { get; set; }
        public DateTime Time { get; set; }
        public TimeSpan Duration { get; set; }
        public string response { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
    }
}
