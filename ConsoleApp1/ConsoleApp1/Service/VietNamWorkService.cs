using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1.Service
{
   public class VietNamWorkService
    {
        public  readonly string cUrlPath = @"../../../cUrl/vietnamwork_it_job.curl";

        public  readonly string outputDataPath = @"../../../OutPut/";

        private CUrl cUrl;

        public VietNameWorkModle vietNameWorkModle { set; get; }


        

        public VietNamWorkService()
        {
            cUrl = new CUrl();

        }

        public void CrawlData()
        {
            cUrl.ReadFile(cUrlPath);
            var client = new RestClient(cUrl.BaseURL);
            var request = new RestRequest(Method.GET);

            foreach (var item in cUrl.Header)
            {
                request.AddHeader(item.Key, item.Value);
            }
            // request.AddParameter("application/x-www-form-urlencoded", test.FormContent, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);


            using (StreamWriter file = File.CreateText(outputDataPath + "vietnamwork_it_job.data.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, JsonConvert.DeserializeObject(response.Content));
            }



            // ép kiểu result về model
             this.vietNameWorkModle = JsonConvert.DeserializeObject<VietNameWorkModle>(response.Content);
        }


        public void a(int a)
        {

        }

        public int b()
        {

            return 1;
        }
        public void c()
        {
            
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
            // request.AddParameter("application/x-www-form-urlencoded", test.FormContent, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            result = response.Content.Replace("\n", "");
            
            




            //using (StreamWriter file = File.CreateText(outputDataPath + "vietnamwork_it_job.data.json"))
            //{
            //    JsonSerializer serializer = new JsonSerializer();
            //    //serialize object directly into file stream
            //    serializer.Serialize(file, JsonConvert.DeserializeObject(response.Content));
            //}


            return result;
            // ép kiểu result về model
            // VietNameWorkModle myDeserializedClass = JsonConvert.DeserializeObject<VietNameWorkModle>(response.Content);



        }


       
    }
}
