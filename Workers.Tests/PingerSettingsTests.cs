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
    public class PingerSettingsTests
    {
        [Test]
        public void CanCreateNullSettingsFromNull()
        {
            Assert.IsNull(SettingsFactory.Create<PingerSettings>(null));
        }

        [Test]
        public void CanCreateNullSettingsFromEmpty()
        {
            Assert.IsNull(SettingsFactory.Create<PingerSettings>(""));
        }

        [Test]
        public void CanCreateNullSettingsFromEmptyObject()
        {
            Assert.IsNull(SettingsFactory.Create<PingerSettings>("{}"));
        }

        [Test]
        public void CanCreateSettingsFromJson()
        {
            var s = SettingsFactory.Create<PingerSettings>("{pings:[{urls:['url1.com','url2.com'],interval:1.5}]}");

            Assert.NotNull(s);
            Assert.NotNull(s.Pings);
            Assert.AreEqual(1,s.Pings.Count);

            var p1 = s.Pings.First();
            Assert.AreEqual(2,p1.Urls.Count);
            Assert.AreEqual(1.5,p1.Interval);
        }
    }
}
