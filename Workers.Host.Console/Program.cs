using System;
using System.Configuration;
using Quartz;
using Quartz.Impl;
using Workers.Jobs;

namespace Workers.Host.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = SettingsFactory.Create<PingerSettings>(ConfigurationManager.AppSettings["settings"]);

            if(settings != null && settings.Pings != null && settings.Pings.Count > 0)
            {
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
}
