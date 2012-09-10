using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rappers.DaData;
using Rappers.DaData.Implementations.Mongo;
using Rappers.HipHop.Models;
using Rappers.HipHop.Services;
using Rappers.HipHop.Services.Implementations.S3;
using Workers.Fontend.Web.Models;
using Workers.Models;

namespace Workers.Fontend.Web.Controllers
{
    public class DataController : Controller
    {
        private readonly IRepository<PingerModel> _repository;
        private readonly IRepository<ParsedLog> _parsedLogRepo;
        private readonly IParseLogs _s3Parser;
        public DataController()
        {
            _repository = new MongoRepository<PingerModel>(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
            _parsedLogRepo = new MongoRepository<ParsedLog>(ConfigurationManager.AppSettings["localMongo"]);
            _s3Parser = new S3LogParser();
        }

        public ActionResult PingCsv()
        {
            return View(_repository.Search(p=>p.Url == "http://pro-moshcam-s3.edgesuite.net/videos-moshcam/test/SONY_15_DUAL_SCREEN.MP4" && p.Status == "OK").ToList());
        }

        public ActionResult PutS3LogsInMongo()
        {
            _parsedLogRepo.DeleteAll();
            _s3Parser.ParseAction(new DirectoryInfo(@"c:\temp\s3logs"), "access",(l)=> _parsedLogRepo.Save(l));
            return Json(new {status = "done"}, JsonRequestBehavior.AllowGet);
        }

        public ActionResult S3Hits()
        {
            var model = new CsvModel(){Filename = "s3Hits.csv"};
            var date = DateTime.Parse("2012-08-08T00:58:05.000Z").ToUniversalTime();
            var go = true;
            int i = 0;
            do
            {
                DateTime today = date.AddDays(i);
                DateTime tomorrow = today.AddDays(1);
                DateTime yesterday = today.AddDays(-1);
                var dayLogs = _parsedLogRepo.Search(l => l.Time < tomorrow && l.Time > yesterday).ToList();
                i++;
                if(dayLogs.Any())
                {
                    var cols = new[]
                    {
                    new CsvColumn("Date",today.Date.ToString("yyyy-MM-dd")),
                    new CsvColumn("200Hits",dayLogs.Count(l => l.HttpResponseCode == 200).ToString()),
                    new CsvColumn("PartialHits",dayLogs.Count(l => l.HttpResponseCode == 206).ToString()),
                    new CsvColumn("OtherHits",dayLogs.Count(l => l.HttpResponseCode != 206 && l.HttpResponseCode != 200).ToString()),
                    new CsvColumn("TotalHits",dayLogs.Count().ToString()),
                    new CsvColumn("TotalData",dayLogs.Sum(l=>l.BytesSent).ToString())
                    };
                    model.Rows.Add(new CsvRow()
                    {
                        Columns = cols.ToList()
                    });
                }
                else
                {
                    go = false;
                }
                
            } while (go);

            return View("csv",model);
        }

    }
}
