using System.Collections.Generic;
using Newtonsoft.Json;

namespace Workers.Jobs
{
    public class PingerSettings
    {
        public List<PingSetting> Pings { get; set; }
    }

    public class PingSetting
    {
        public List<string> Urls { get; set; }
        public double Interval { get; set; }
    }

    public static class SettingsFactory
    {
        public static T Create<T>(string json)
        {
            if(json == null || json == "{}")
            {
                json = string.Empty;
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
