using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Quartz;
using Quartz.Impl;
using StatsMix;
using Workers.Host.Console.Jobs;

namespace Workers.Host.Console
{
    class Program
    {
        private static readonly TimeSpan _interval;
        static Program()
        {
            _interval = TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["interval"]));
        }
        static void Main(string[] args)
        {
            // construct a scheduler
            var scheduler = new StdSchedulerFactory().GetScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<Pinger>().Build();

            var trigger = TriggerBuilder.Create()
                            .WithSimpleSchedule(x =>
                            x.WithInterval(_interval).RepeatForever())
                            .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
