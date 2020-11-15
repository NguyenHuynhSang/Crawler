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
    /// <summary>
    /// 1 task crawl pagesource về ( bỏ vào hết 1 list)
    ///  1 task lấy dữ liệu ra
    /// 
    /// 
    /// </summary>
    /// 



    public class CareerBuilderService
    {

        private static string base_url = @"https://careerbuilder.vn/viec-lam/cntt-phan-mem-c1-vi.html";
        private static CUrl base_curl;
        private static string curl_path = @"../../../cUrl/Careerbuilder.curl";
        private readonly IWebDriver _driver;
        public Queue<String> jobLinkList;
        public List<CareerBuilderModel> careerBuilderModels;

        // lấy ra tất cả cái link jobdetail 
        private Thread thread_CrawData;

        // lấy ra dữ liệu jobdetail dựa vào từng links
        private Thread Thread_ExtractData;


        public CareerBuilderService()
        {
            careerBuilderModels = new List<CareerBuilderModel>();
            base_curl = new CUrl();
            base_curl.ReadFile(curl_path);

            _driver = new ChromeDriver();

            _driver.Navigate().GoToUrl(base_url);
            jobLinkList = new Queue<string>();
        }





        public void Process()
        {
            thread_CrawData = new Thread(CrawlData);
            Thread_ExtractData = new Thread(ExtractContent);
            thread_CrawData.Start();
            Thread_ExtractData.Start();



        }

        private void WriteFile()
        {

            JsonSerializer serializer = new JsonSerializer();
            //serialize object directly into file 
            var json = JsonConvert.SerializeObject(careerBuilderModels, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"../../../Output/CareerBuilder.json", json);

        }



        public void CrawlData()
        {
            NextPage();
        }



        private static int pageIndex = 1;


        /// <summary>
        /// To-do: nhấn next page liên tục do đến hết
        /// web paging dạng cuộn 
        /// </summary>
        private void NextPage()
        {
            try
            {
                IWait<IWebDriver> wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(30.00));
                wait.Until(driver1 => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));
                Console.WriteLine("********** current page**********" + pageIndex);
                Console.WriteLine("========URL:" + _driver.Url);
                ExtractLink();
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




        private int extractCouter = 1;
        bool moreCondition = true;
        private void ExtractContent()
        {


            do
            {
                while (jobLinkList.Count != 0)
                {

                
                    var item = jobLinkList.Dequeue();
                    base_curl.BaseURL = item;
                    var pageSource = GetPageSource(base_curl);
                    var result = Regex.Match(pageSource, @"(?<='dispatch', 'p_detail_page',).*?(?=\);)", RegexOptions.Singleline).Value;

                    string replacement = Regex.Replace(result, @"\t|\n|\r", "");
                    //var replace2= Regex.Replace(replacement, "\\\"", "\"");
                    CareerBuilderModel cc = new CareerBuilderModel();
                    //đùa nhau à
                    result = result.Replace("First Alliances' client", "First Alliances client");
                    result = result.Replace("First Alliances' Client", "First Alliances client");
                    result = result.Replace("First Alliances's Client", "First Alliances client");
                    result = result.Replace("First Alliances's client", "First Alliances client");
                    result = result.Replace("  L'amour Bakery", "  Lamour Bakery");
                    try
                    {
                        var job = JsonConvert.DeserializeObject<CareerBuilderModel>(result);
                        careerBuilderModels.Add(job);
                    }
                    catch (Exception)
                    {
                        extractCouter++;
                        continue;
                    }
             

               
                  
                    Console.WriteLine("Extractor" + extractCouter);
                    extractCouter++;


                }

            } while (thread_CrawData.IsAlive);
     
            WriteFile();

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






    }
}
