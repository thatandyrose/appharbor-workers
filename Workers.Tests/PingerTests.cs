using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Workers.Host.Console.Jobs;
using Workers.Models;

namespace Workers.Tests
{
    [TestFixture]
    public class PingerTests
    {
        [Test]
        public void CanPublish()
        {
            var pinger = new Pinger();

            pinger.Publish(new PingerModel()
                               {
                                   Duration = TimeSpan.FromSeconds(12),
                                   Time = DateTime.Now,
                                   Url = "tests.now"
                               });

        }
    }
}
