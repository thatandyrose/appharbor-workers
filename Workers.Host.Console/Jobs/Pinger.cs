using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Quartz;
using Rappers.DaData;
using Rappers.DaData.Implementations.Mongo;
using StatsMix;
using Workers.Models;

namespace Workers.Host.Console.Jobs
{
    public class Pinger: IJob
    {
        private readonly IRepository<PingerModel> _repository;
        public Pinger()
        {
            _repository = new MongoRepository<PingerModel>(ConfigurationManager.AppSettings["MONGOLAB_URI"]);
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var urls = ConfigurationManager.AppSettings["pinger.urls"].Split(',');
                urls.ToList().ForEach(url =>
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var s = new Stopwatch();
                    s.Start();
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    {
                        string ret = string.Empty;
                        if (stream != null)
                        {
                            string readToEnd = new StreamReader(stream).ReadToEnd();
                            ret = "cancelled";
                        }
                        s.Stop();
                        Publish(new PingerModel()
                        {
                            Time = DateTime.Now,
                            Url = url,
                            response = ret,
                            Status = response.StatusCode.ToString(),
                            StatusDescription = response.StatusDescription,
                            Duration = s.Elapsed
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                throw new JobExecutionException(ex);
            }
        }

        public void Publish(PingerModel model)
        {
            _repository.Save(model);

            var smClient = new Client("444b5639fd1e744c2150");
            //to add metadata, create a Dictionairy object containing key-value paisrs
            var meta = new Dictionary<string,string>
            {
                {"url", model.Url}
            };

            var props = new Dictionary<string, string>
            {
                {"value", model.Duration.ToString()}
            };

            smClient.track("ResponseTime", props, meta);	
        }
    }
}
