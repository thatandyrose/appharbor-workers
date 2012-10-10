using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workers.Host.Console.Jobs
{
    public class PingerSettings
    {
        public List<PingSetting> Pings { get; set; }
    }

    public class PingSetting
    {
        public List<string> Urls { get; set; }
        public int Interval { get; set; }
    }
}
