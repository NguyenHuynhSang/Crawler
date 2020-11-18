using Crawler.Model;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1.Service
{
    /// <summary>
    /// Web gọi api, respose trả về là json, nên chỉ cần copy thủ công file cUrl (bash) vào thư mục cUrl
    /// 
    /// </summary>
   public class VietNamWorkService:BaseCrawlService
    {
        public  readonly string cUrlPath = @"../../../cUrl/vietnamwork_it_job.curl";

        private CUrl cUrl;

        public VietNameWorkModle vietNameWorkModle { set; get; }

      
        public VietNamWorkService()
        {
            cUrl = new CUrl();

        }

   

      


        private string GetPageSource(CUrl cUrl)
        {
            string result = "";
            string aaa = cUrl.BaseURL.Replace("\\", "");
            var client = new RestClient(cUrl.BaseURL);
            var request = new RestRequest(Method.GET);

            foreach (var item in cUrl.Header)
            {
                request.AddHeader(item.Key, item.Value);
            }
            request.AddParameter("application/x-www-form-urlencoded", cUrl.FormContent, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            result = response.Content.Replace("\n", "");
            return result;
            // ép kiểu result về model
            // VietNameWorkModle myDeserializedClass = JsonConvert.DeserializeObject<VietNameWorkModle>(response.Content);



        }

        protected override void WriteToFile()
        {
            JsonSerializer serializer = new JsonSerializer();
            //serialize object directly into file 
            var json = JsonConvert.SerializeObject(vietNameWorkModle, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(this.outPutPath+"Itviec.json", json);
        }

        public override void Process()
        {
            CrawlData();
            WriteToFile();
        }

        protected override void CrawlData()
        {
            cUrl.ReadFile(cUrlPath);
            var client = new RestClient(cUrl.BaseURL);
            var request = new RestRequest(Method.GET);

            foreach (var item in cUrl.Header)
            {
                request.AddHeader(item.Key, item.Value);
            }
            request.AddParameter("application/x-www-form-urlencoded", cUrl.FormContent, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            // ép kiểu result về model
            this.vietNameWorkModle = JsonConvert.DeserializeObject<VietNameWorkModle>(response.Content);
            JsonSerializer serializer = new JsonSerializer();
            //serialize object directly into file 
            var json = JsonConvert.SerializeObject(vietNameWorkModle, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outPutPath + "vietnamwork_it_job.data.json", json);
            Console.WriteLine("FINISH");
        }
    }
}
