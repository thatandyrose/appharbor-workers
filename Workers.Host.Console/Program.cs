using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using StatsMix;
using Workers.Host.Console.Jobs;

namespace Workers.Host.Console
{
    class Program
    {
        private static readonly TimeSpan Interval;
        static Program()
        {
            Interval = TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["interval"]));
        }
        static void Main(string[] args)
        {
            var settings = JsonConvert.DeserializeObject<PingerSettings>(ConfigurationManager.AppSettings["settings"]);

            // construct a scheduler
            var scheduler = new StdSchedulerFactory().GetScheduler();
            scheduler.Start();
            
            settings.Pings.ForEach(p=>
            {
                var job = JobBuilder.Create<Pinger>()
                    .UsingJobData("urls",string.Join(",",p.Urls))
                    .Build();
                
                var trigger = TriggerBuilder.Create()
                                .WithSimpleSchedule(x =>
                                x.WithInterval(TimeSpan.FromMinutes(p.Interval)).RepeatForever())
                                .Build();

                scheduler.ScheduleJob(job, trigger);                            
            });
        }
    }
}
