using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApp1.Service
{
    /// <summary>
    /// 1 task crawl pagesource về ( bỏ vào hết 1 list)
    ///  1 task lấy dữ liệu ra
    /// 
    /// 
    /// </summary>
    public class CareerBuilderService
    {

        private static string base_url = @"https://careerbuilder.vn/viec-lam/cntt-phan-mem-c1-vi.html";
        private static CUrl base_curl;
        private static string curl_path = @"../../../cUrl/Careerbuilder.curl";
        private readonly IWebDriver _driver;
        public List<String> pageSourceList;
        public List<CareerBuilderModel> careerBuilderModels;

        private Thread thread_CrawData;
        private Thread Thread_ExtractData;


        public CareerBuilderService()
        {
            careerBuilderModels = new List<CareerBuilderModel>();
            base_curl = new CUrl();
            base_curl.ReadFile(curl_path);

            _driver = new ChromeDriver();

            _driver.Navigate().GoToUrl(base_url);
            pageSourceList = new List<string>();

            Process();

        }


        public void Process()
        {
            thread_CrawData = new Thread(CrawlData);
            Thread_ExtractData = new Thread(Extractor);
            thread_CrawData.Start();
            Thread_ExtractData.Start();
        }


        public void CrawlData()
        {
            NextPage();
        }



        private static int pageIndex = 1;
        private void NextPage()
        {
            try
            {
                IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(30.00));
                wait.Until(driver1 => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));
                Console.WriteLine("********** current page**********" + pageIndex);
                Console.WriteLine("========URL:" + _driver.Url);
                ExtractPageSource();
                var btnNextPage = _driver.FindElement(By.ClassName("next-page"));
                if (btnNextPage.Displayed)
                {

                    btnNextPage.Click();
                    pageIndex++;
                }

                NextPage();

            }
            catch (Exception)
            {
                Console.WriteLine("Exception");
                return;
            }


        }

        private static int counter = 1;
        private void ExtractPageSource()
        {
            var pageSource = _driver.PageSource;
            var job_list_div = Regex.Match(pageSource, "", RegexOptions.Singleline);

            var jobItemsLinkdiv = Regex.Matches(pageSource, @"(?=<a class=""job_link"")(.*?)(?=</a>)", RegexOptions.Singleline);
            Console.WriteLine("per page jobitem count:", jobItemsLinkdiv.Count);
            foreach (var item in jobItemsLinkdiv)
            {
                var link = Regex.Match(item.ToString(), @"(?<=href="")(.*?)(?="" target)", RegexOptions.Singleline).Value;
                ExtractContent(link);
                Console.WriteLine(counter);
                counter++;
            }

            var a = 2;
        }




        private void ExtractContent(string url)
        {
            base_curl.BaseURL = url;
            var pageSource = GetPageSource(base_curl);
            pageSourceList.Add(pageSource);

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

        public void Extractor()
        {

            while (true)
            {
                while (pageSourceList.Count != 0)
                {
                    var result = Regex.Match(pageSourceList[0], @"(?<='dispatch', 'p_detail_page',).*?(?<=})", RegexOptions.Singleline).Value;
                    var job = JsonConvert.DeserializeObject<CareerBuilderModel>(result);
                    careerBuilderModels.Add(job);
                    pageSourceList.RemoveAt(0);
                    Console.WriteLine("Extractor");

                }

                if (!thread_CrawData.IsAlive)
                {
                    Console.WriteLine("FINISH");
                    break;
                }
            }
        }

    }
}
