using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Quartz;
using Rappers.DaData;
using Rappers.DaData.Implementations.Mongo;
using Workers.Models;

namespace Workers.Jobs
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
                var urls = context.Get("urls").ToString().Split(',');

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
                            ret = string.Format("{0}...", readToEnd.Substring(0, 500));
                        }
                        s.Stop();
                        
                        Publish(new PingerModel()
                        {
                            Time = DateTime.Now,
                            Url = url,
                            response = ret,
                            Status = response.StatusCode.ToString(),
                            StatusDescription = response.StatusDescription,
                            Duration = s.Elapsed,
                            ContentLength = response.ContentLength,
                            ContentType = response.ContentType
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
        }
    }
}
