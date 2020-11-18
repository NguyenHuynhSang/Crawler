using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp1.Service
{

    public class CareerBuilderService : BaseCrawlService
    {

        private static CUrl base_curl;
        private static string curl_path = @"../../../cUrl/Careerbuilder.curl";
        public Queue<String> jobLinkList;// lấy ra ds job từ thread crawl bỏ vào hàng đợi cho thread kia xử lý 
        public List<CareerBuilderModel> careerBuilderModels;




        public CareerBuilderService() : base()
        {
            this.jobListUrl = @"https://careerbuilder.vn/viec-lam/cntt-phan-mem-c1-vi.html";
            careerBuilderModels = new List<CareerBuilderModel>();
            resultFileName = "careerbuilder.json";
            base_curl = new CUrl();
            base_curl.ReadFile(curl_path);
            _driver.Navigate().GoToUrl(jobListUrl);
            jobLinkList = new Queue<string>();

        }



        /// <summary>
        ///  so sánh số record thực tế so với record lấy ra
        /// </summary>
        private void Check()
        {
            var dataJson = File.ReadAllText("../../../Output/CareerBuilder.json");
            var data = JsonConvert.DeserializeObject<List<CareerBuilderModel>>(dataJson).Count();
            Console.WriteLine("Actual record:" + data);
        }


        private static int pageIndex = 1;


        /// <summary>
        /// To-do: nhấn next page liên tục do đến hết
        /// web paging dạng cuộn 
        /// </summary>


        private static int counter = 1;
        private void ExtractLink()
        {
            var pageSource = _driver.PageSource;
            var job_list_div = Regex.Match(pageSource, "", RegexOptions.Singleline);

            var jobItemsLinkdiv = Regex.Matches(pageSource, @"(?=<a class=""job_link"")(.*?)(?=</a>)", RegexOptions.Singleline);
            Console.WriteLine("per page jobitem count:", jobItemsLinkdiv.Count);
            foreach (var item in jobItemsLinkdiv)
            {
                var link = Regex.Match(item.ToString(), @"(?<=href="")(.*?)(?="" target)", RegexOptions.Singleline).Value;
                jobLinkList.Enqueue(link);
                Console.WriteLine(counter);
                counter++;
            }

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
            //request.AddParameter("application/x-www-form-urlencoded", test.FormContent, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Xml;
            IRestResponse response = client.Execute(request);
            result = response.Content.Replace("\n", "");
            return result;
        }

        protected override void CrawlData()
        {
            try
            {
                _wait.Until(driver1 => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));
                Console.WriteLine("********** current page**********" + pageIndex);
                Console.WriteLine("========URL:" + _driver.Url);
                ExtractLink();
                var btnNextPage = _driver.FindElement(By.ClassName("next-page"));
                if (btnNextPage.Displayed)
                {
                    btnNextPage.Click();
                    pageIndex++;
                }

                CrawlData();

            }
            catch (Exception)
            {
                Console.WriteLine("Exception");
                return;
            }

        }

        private int extractCouter = 1;
        bool morecordition = true;

        /// <summary>
        /// do trang con có 1 script cần truyền param vào là chuỗi json thông tin của job nên chỉ cần lấy param đó ra
        /// </summary>
        protected override void ExtractContent()
        {
            do
            {
                while (jobLinkList.Count != 0)
                {


                    var item = jobLinkList.Dequeue();
                    base_curl.BaseURL = item;
                    var pageSource = GetPageSource(base_curl);
                    // lấy ra param thứ 3 của script
                    var result = Regex.Match(pageSource, @"(?<='dispatch', 'p_detail_page',).*?(?=\);)", RegexOptions.Singleline).Value;

                    string replacement = Regex.Replace(result, @"\t|\n|\r", "");
                    //var replace2= Regex.Replace(replacement, "\\\"", "\"");
                    CareerBuilderModel cc = new CareerBuilderModel();


                    // lỗi gây ra nếu json attribute value có chứa ' nên k  dùng thư viện có sẵn ép sang đc
                    try
                    {
                        var job = JsonConvert.DeserializeObject<CareerBuilderModel>(result);
                        careerBuilderModels.Add(job);
                    }
                    catch (Exception)
                    {
                        // bỏ qua những record bị lỗi
                        Console.WriteLine("bỏ qua record do lỗi ép kiểu");
                        extractCouter++;
                        continue;
                    }
                    Console.WriteLine("Extractor" + extractCouter);
                    extractCouter++;


                }

            } while (crawlThread.IsAlive);

            WriteToFile();
        }

         protected override void WriteToFile(Object obj = null)
        {
            base.WriteToFile(careerBuilderModels);
        }
    }
}
