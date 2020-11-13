using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ConsoleApp1.shared;
using ConsoleApp1.Service;

namespace ConsoleApp1
{
    public class ItViecService
    {
        public readonly string email = @"nguyentuoc123789a@gmail.com";
        public readonly string passWord = @"abc123456";


        private IWebDriver _driver;
        public readonly string itViet_url = @"https://itviec.com";
        public readonly string itWorkPath = @"../../../cUrl/itwork.curl";
        private CUrl itWorkCurl;

        public readonly string outputDataPath = @"../../../OutPut/";
        public List<String> pageLinkList;
        public WorkList WorkList;


        public ItViecService(IWebDriver webDriver)
        {
            WorkList = new WorkList();
            this._driver = new ChromeDriver();

            pageLinkList = new List<string>();
            itWorkCurl = new CUrl();
            itWorkCurl.ReadFile(itWorkPath);
            _driver.Navigate().GoToUrl("https://itviec.com/it-jobs");
            Process();

        }


        public void Process()
        {
            Login();
            //CrawTask();
        }

        private void ExtractContent() 
        {
            
        }
        private void WriteFile()
        {

        }



        private int counter = 0;
        public void LoadToEnd()
        {
            var login_btn = _driver.FindElement(By.XPath("//a[@class='pageMenu__link']"));

            login_btn.Click();

            WebDriverWait wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 5));
            //    wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//input[@name='user[email]' and contains(@class,'form-control')]")));
            IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            string script = @"document.getElementsByName('user[email]')[0].setAttribute('value', 'abc@gmail.com')";
            js.ExecuteScript(script);

            Console.WriteLine("Task 1");
            
            while (IsElementPresent(By.Id("show_more")))
            {
                try
                {
                    var btnNextPage = _driver.FindElement(By.Id("show_more"));
                    if (btnNextPage.Enabled)
                    {
                        btnNextPage.Click();
                        
                        var a= new WebDriverWait(_driver, TimeSpan.FromSeconds(120)).Until(
                        d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                 

                    }

                    var itemLinks = Regex.Matches(_driver.PageSource, @"(?<=<h2 class=""title"")(.*?)(?=</h2>)", RegexOptions.Singleline);
                    for (int i = counter; i < itemLinks.Count; i++)
                    {
                        pageLinkList.Add(itemLinks[i].Value);
                        counter++;

                    }


                }
                catch (Exception)
                {

                    
                    //    Thread.Sleep(50);
                }


            }


        }

        private void Login()

        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(2)); // throw timeout after 2 minute
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""pageMenuToggle""]/ul[2]/li[1]/a")));
            var loginBtn = _driver.FindElement(By.XPath(@"//*[@id=""pageMenuToggle""]/ul[2]/li[1]/a"));
            loginBtn.Click();

          
            wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(@"//*[@id=""sign-in-modal""]")));


            var emailInput = _driver.FindElement(By.XPath(@"//*[@id=""signin-tab""]/form/div[2]/input"));
            var passInput = _driver.FindElement(By.XPath(@"//*[@id=""signin-tab""]/form/div[3]/input"));

            emailInput.SendKeys(email);
            passInput.SendKeys(passWord);

            var submit = _driver.FindElement(By.XPath(@"//*[@id=""signin-tab""]/form/div[5]/input[3]"));
            submit.Click();
            //_driver.FindElement(By.XPath("//a[@class='pageMenu__headerLink' and @data-toggle='modal']")).Click();

            //WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));
            //var passwordElement = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=""signin-tab""]/form/div[2]/input")));
            //passwordElement.Click();
            //passwordElement.Clear();
            //_driver.FindElement(By.Name("user[email]")).SendKeys("abc");


        }


        private bool isComplete = false;
        private int processCounter = 0;
        public async void CrawItViec()
        {

            //_driver.FindElement(By.Name("user[password]")).Clear();
            //_driver.FindElement(By.Name("user[password]")).Click(); // Keep this click statement even if you are using click before clear.
            //_driver.FindElement(By.Name("user[password]")).SendKeys("manish");





        



            using (StreamWriter file = File.CreateText(outputDataPath + "itviec.data.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                var json = JsonConvert.SerializeObject(this.WorkList.WorkModels);
                // json= Regex.Replace(json, @"\r\n?|\n", "");


                serializer.Serialize(file, json);

            }


            //    var itemLink = Regex.Matches(source, @"(?<=<h2 class=""title"")(.*?)(?=</h2>)", RegexOptions.Singleline);


            //_driver.FindElement(By.Name("user[email]")).Click();

        }





        private void CrawTask()
        {

            //  Console.WriteLine("Task 2 process");


            /// Read curl
            /// 


            if (processCounter == pageLinkList.Count)
            {
                if (isComplete)
                {
                    return;
                }

            }
            else
            {
                List<string> ChildPageList = new List<string>();
                for (int i = processCounter; i < pageLinkList.Count; i++)
                {

                    var link = Regex.Match(pageLinkList[i].ToString(), @"(?<=href="")(.*?)(?="">)", RegexOptions.Singleline);
                    Console.WriteLine("counter:   " + processCounter + "/t" + link);


                    string url = itViet_url + link;
                    itWorkCurl.BaseURL = itViet_url + link;
                    var item = GetPageSource(itWorkCurl);
                    processCounter++;

                    ItWorkModel companyJob = new ItWorkModel();
                    string jobName = Regex.Match(item, @"(?<=<h1 class='job_title'>)(.*?)(?=</h1>)", RegexOptions.Singleline).Value;
                    var divSkillTag = Regex.Match(item, @"(?<=<div class='tag-list'>).*?(?=</div>)", RegexOptions.Singleline).Value;

                    var divCompanyName = Regex.Match(item, @"(?<=<h3 class='name'>)(.*?)(?=</h3>)", RegexOptions.Singleline).Value;
                    var companyName = Regex.Match(divCompanyName, @"(?<="">).*?(?=</a>)", RegexOptions.Singleline).Value;


                    var skills = Regex.Matches(divSkillTag, @"(?<=<span>).*?(?=</span>)", RegexOptions.Singleline).ToList();
                    var skillsAndExperienceDiv = Regex.Match(item, @"(?=<div class='skills_experience').*?(?=</div>)", RegexOptions.Singleline);

                    var skillAndExperience = Regex.Matches(skillsAndExperienceDiv.Value, @"(?<=<li>).*?(?=</li>)", RegexOptions.Singleline).ToList();


                    companyJob.id = processCounter;
                    companyJob.JobName = jobName;
                    companyJob.CompanyName = companyName;



                    Console.WriteLine("*********************");
                    Console.WriteLine(companyName);
                    Console.WriteLine(jobName);


                    foreach (var item2 in skills)
                    {
                        companyJob.Skills.Add(item2.Value);
                        Console.Write(item2 + "-");
                    }

                    foreach (var item3 in skillAndExperience)
                    {
                        companyJob.SkillsExperience.Add(item3.ToString());
                        Console.WriteLine("- " + item3);
                    }
                    this.WorkList.WorkModels.Add(companyJob);

                    Console.WriteLine("*********************");



                }



            }


            CrawTask();

        }




        private bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                isComplete = true;
                return false;
            }
        }

        public string GetPageSource(CUrl cUrl)
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





            return result;


        }

        /// <summary>
        /// Chia 2 tiến trình 1 crawl 1 xử lý
        /// 
        /// </summary>
        /// 




        public string ReturnSourcePage(string url, CUrl cUrl)
        {
            string result = "NOT FOUND!!!";


            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);

            foreach (var item in cUrl.Header)
            {
                request.AddHeader(item.Key, item.Value);
            }
            // request.AddParameter("application/x-www-form-urlencoded", test.FormContent, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.ContentEncoding);
            return response.ContentEncoding;


        }




    }


}
