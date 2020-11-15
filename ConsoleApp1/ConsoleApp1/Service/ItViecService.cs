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
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp1
{
    public class Container
    {
        public string link { set; get; } // link child page
        public string salary { set; get; } // lấy ra salary 
    }
    public class ItViecService
    {

        public readonly string email = @"nguyentuoc123789a@gmail.com";
        public readonly string passWord = @"abc123456";

        private IWebDriver _driver;
        public readonly string itViet_url = @"https://itviec.com";
        public readonly string itWorkPath = @"../../../cUrl/itwork.curl";
        private CUrl itWorkCurl;

        public readonly string outputDataPath = @"../../../OutPut/";
        public Queue<Container> pageLinkList;
        public List<ItWorkModel> itWorkModels;

        private Thread crawlThread;
        private Thread extractThread;
        WebDriverWait _wait;



        public ItViecService()
        {
            itWorkModels = new List<ItWorkModel>();
            this._driver = new ChromeDriver();

            pageLinkList = new Queue<Container>();
            itWorkCurl = new CUrl();
            itWorkCurl.ReadFile(itWorkPath);
            _driver.Navigate().GoToUrl("https://itviec.com/it-jobs");
            _wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(1));

        }


        public void Process()
        {
            Login();
            crawlThread = new Thread(ExtractLink);
            extractThread = new Thread(ExtractPage);
            crawlThread.Start();
             extractThread.Start();

        }





        private int counter = 0;
        private int currentPage = 1;
        public void ExtractLink()
        {
            ////    wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//input[@name='user[email]' and contains(@class,'form-control')]")));
            //IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
            //string script = @"document.getElementsByName('user[email]')[0].setAttribute('value', 'abc@gmail.com')";
            //js.ExecuteScript(script);

            //Console.WriteLine("Task 1");
            do
            {
                try
                {
                    var a = _wait.Until(
                       d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                  
                    var item = _driver.FindElements(By.XPath(@"//*[starts-with(@class, ""job"")]/div/div[2]/div[1]/div/h2/a"));
                    var salary= _driver.FindElements(By.XPath(@"//*[starts-with(@class, ""job"")]/div/div[2]/div[1]/div/div[1]/span[2]"));
                    Console.WriteLine("current page:" + currentPage);
                    //foreach (var item in itemLinksDiv)
                    //{
                    //    Console.WriteLine(item.GetAttribute("href"));
                    //}

                    //   var itemLinks = Regex.Matches(_driver.PageSource, @"(?<=<h2 class=""title"")(.*?)(?=</h2>)", RegexOptions.Singleline);
                    for (int i = counter; i < item.Count; i++)
                    {
                        Container c = new Container();
                        
                        var link = item[i].GetAttribute("href");
                        c.link = link;
                        c.salary = salary[i].Text;
                        pageLinkList.Enqueue(c);
                        counter++;

                    }
                    Console.WriteLine("total recond:" + counter);
                    Console.WriteLine("actual recond:" + item.Count);
                    var btnNextPage = _driver.FindElement(By.XPath(@"//*[@id=""show_more""]/a"));
                    btnNextPage.Click();
                    currentPage++;

                    Console.WriteLine("-----------");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception but can ignore");
                    continue;
                    //    Thread.Sleep(50);
                }
            } while (IsElementPresent(By.Id("show_more")));


            Console.WriteLine("FINISH");


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


        }


        private bool isComplete = false;
        private int processCounter = 0;
        public void WriteFile()
        {

            JsonSerializer serializer = new JsonSerializer();
            //serialize object directly into file 
            var json = JsonConvert.SerializeObject(itWorkModels, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"../../../Output/Itviec.json", json);

        }





        private int extractCounter = 1;
        private void ExtractPage()
        {

            //  Console.WriteLine("Task 2 process");


            /// Read curl
            /// 
            do
            {
                while (pageLinkList.Count != 0)
                {
                    try
                    {
                        Console.WriteLine("Extractor :" + extractCounter);
                        extractCounter++;
                        var item = pageLinkList.Dequeue();
                        //   Console.WriteLine("counter:   " + processCounter + "/t" + link);
                        itWorkCurl.BaseURL = item.link;
                        var pageSource = GetPageSource(itWorkCurl);
                        processCounter++;

                        ItWorkModel companyJob = new ItWorkModel();
                        string jobName = Regex.Match(pageSource, @"(?<=<h1 class='job_title'>)(.*?)(?=</h1>)", RegexOptions.Singleline).Value;
                        var divSkillTag = Regex.Match(pageSource, @"(?<=<div class='tag-list'>).*?(?=</div>)", RegexOptions.Singleline).Value;
                        var divCompanyName = Regex.Match(pageSource, @"(?<=<h3 class='name'>)(.*?)(?=</h3>)", RegexOptions.Singleline).Value;
                        var companyName = Regex.Match(divCompanyName, @"(?<="">).*?(?=</a>)", RegexOptions.Singleline).Value;


                        var skills = Regex.Matches(divSkillTag, @"(?<=<span>).*?(?=</span>)", RegexOptions.Singleline).ToList();
                        var skillsAndExperienceDiv = Regex.Match(pageSource, @"(?=<div class='skills_experience').*?(?=</div>)", RegexOptions.Singleline);

                        var skillAndExperience = Regex.Matches(skillsAndExperienceDiv.Value, @"(?<=<li>).*?(?=</li>)", RegexOptions.Singleline).ToList();

                    
                        companyJob.id = processCounter;
                        companyJob.JobName = jobName;
                        companyJob.CompanyName = companyName;
                        companyJob.Salary =item.salary;

                        Console.WriteLine("-----" );
                        Console.WriteLine(companyName);
                        Console.WriteLine(jobName);
                        Console.WriteLine(item.salary);
                        this.itWorkModels.Add(companyJob);
                        Console.WriteLine("=====");


                    }
                    catch (Exception)
                    {

                        Console.WriteLine("Extract exception");
                    }

                }


            } while (crawlThread.IsAlive);

            Console.WriteLine("All finish");

            WriteFile();
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
                Console.WriteLine("Element not present");
                isComplete = true;
                return false;
            }
        }

        public string GetPageSource(CUrl cUrl)
        {
            string result = "";
            var client = new RestClient(cUrl.BaseURL);
            var request = new RestRequest(Method.GET);
           
            foreach (var item in cUrl.Header)
            {
                request.AddHeader(item.Key, item.Value);
            }
            foreach (var item in _driver.Manage().Cookies.AllCookies)
            {
                request.AddCookie(item.Name, item.Value); 
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









    }


}
