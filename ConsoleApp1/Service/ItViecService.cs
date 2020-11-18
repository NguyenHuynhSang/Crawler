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
    public class ItViecService : BaseCrawlService
    {



        public readonly string itViet_url = @"https://itviec.com";
        public readonly string itWorkPath = @"../../../cUrl/itwork.curl";
        private CUrl itWorkCurl;

        public Queue<Container> pageLinkList;
        public List<ItWorkModel> itWorkModels;




        public ItViecService() : base()
        {
            itWorkModels = new List<ItWorkModel>();
            resultFileName = "itviec.json";

            pageLinkList = new Queue<Container>();
            itWorkCurl = new CUrl();
            itWorkCurl.ReadFile(itWorkPath);
            this.jobListUrl = @"https://itviec.com/it-jobs";
            _driver.Navigate().GoToUrl(jobListUrl);
            _wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(1));

        }







        private int counter = 0;
        private int currentPage = 1;
        private int processCounter = 0;
        private int extractCounter = 1;

        
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

        protected override void WriteToFile(Object obj = null)
        {
            base.WriteToFile(itWorkModels);
        }

        protected override void Login()
        {
            WebDriverWait wait = new WebDriverWait(_driver, TimeSpan.FromMinutes(2)); // throw timeout after 2 minute
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(@"//*[@id=""pageMenuToggle""]/ul[2]/li[1]/a")));
            var loginBtn = _driver.FindElement(By.XPath(@"//*[@id=""pageMenuToggle""]/ul[2]/li[1]/a"));
            loginBtn.Click();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(@"//*[@id=""sign-in-modal""]")));


            var emailInput = _driver.FindElement(By.XPath(@"//*[@id=""signin-tab""]/form/div[2]/input"));
            var passInput = _driver.FindElement(By.XPath(@"//*[@id=""signin-tab""]/form/div[3]/input"));

            emailInput.SendKeys(email);
            passInput.SendKeys(passWord);

            var submit = _driver.FindElement(By.XPath(@"//*[@id=""signin-tab""]/form/div[5]/input[3]"));
            submit.Click();
        }

        protected override void CrawlData()
        {
            do
            {
                try
                {
                    var a = _wait.Until(
                       d => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));

                    var jobLinkLists = _driver.FindElements(By.XPath(@"//*[starts-with(@class, ""job"")]/div/div[2]/div[1]/div/h2/a"));
                    var salary = _driver.FindElements(By.XPath(@"//*[starts-with(@class, ""job"")]/div/div[2]/div[1]/div/div[1]/span[2]"));
                    Console.WriteLine("current page:" + currentPage);
                    for (int i = counter; i < jobLinkLists.Count; i++)
                    {
                        Container c = new Container();
                        var link = jobLinkLists[i].GetAttribute("href");
                        c.link = link;
                        c.salary = salary[i].Text;
                        pageLinkList.Enqueue(c);
                        counter++;

                    }
                    Console.WriteLine("total record:" + counter);
                    Console.WriteLine("actual record:" + jobLinkLists.Count);
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

        protected override void ExtractContent()
        {
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
                        companyJob.Salary = item.salary;

                        Console.WriteLine("-----");
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

            WriteToFile();
        }
    }


}
