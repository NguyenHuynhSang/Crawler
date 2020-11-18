using ConsoleApp1;
using ConsoleApp1.Service;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ConsoleApp1.shared;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;

namespace Crawler.Model
{
    public class TopcvService : BaseCrawlService
    {
        List<TopCvModel> topCvModels;
        public TopcvService():base()
        {
            this.loginUrl = @"https://www.topcv.vn/login";
            this.jobListUrl = @"https://www.topcv.vn/tim-viec-lam-it-phan-mem-c10026";
            topCvModels = new List<TopCvModel>();
         
        }
        // use main thread only
        public override void Process()
        {
            Login();
            CrawlData();
            ExtractContent();
        }

        private int counter = 1;
        private int currentPage = 1;
        protected override void CrawlData()
        {
            var next_page=_driver.FindElements(By.XPath(@"//*[@id=""box-job-result""]/div[2]/nav/ul/li")).Last();

            while (next_page.GetAttribute("class")!= "disabled")
            {
                Console.WriteLine("Page" + currentPage);
                _wait.Until(driver1 => ((IJavaScriptExecutor)_driver).ExecuteScript("return document.readyState").Equals("complete"));
                var listlistDiv= _driver.FindElements(By.XPath(@"//*[@id=""box-job-result""]/div[1]/div"));
                for (int i = 1; i <= listlistDiv.Count; i++)
                {

                    var jobName= _driver.FindElement(By.XPath(@"//*[@id=""box-job-result""]/div[1]/div["+i+"]/div/div[2]/h4/a/span")).Text;
                    var companyName= _driver.FindElement(By.XPath(@"//*[@id=""box-job-result""]/div[1]/div["+i+ "]/div/div[2]/div[1]/a")).Text;
                    var salary = _driver.FindElement(By.XPath(@"//*[@id=""box-job-result""]/div[1]/div[" + i + "]/div/div[2]/div[2]/div[1]/span")).Text;
                    var id = counter;

                    TopCvModel job = new TopCvModel();
                    job.JobName = jobName;
                    job.CompanyName = companyName;
                    job.Salary = salary;
                    job.ID = id;
                    topCvModels.Add(job);

                    Console.WriteLine("EXtractor" + counter);
                    counter++;
                   

                }

             
                next_page.Click();
                next_page = _driver.FindElements(By.XPath(@"//*[@id=""box-job-result""]/div[2]/nav/ul/li")).Last();
                currentPage++;
            }
            Console.WriteLine("Process end");
            WriteToFile();
          
        }

        protected override void ExtractContent()
        {
            throw new NotImplementedException();
        }

        protected override void Login()
        {
            try
            {
                _driver.Navigate().GoToUrl(loginUrl);
                var username = _driver.FindElement(By.XPath(@"//*[@id=""form-login""]/p[1]/input"));
                var password = _driver.FindElement(By.XPath(@"//*[@id=""form-login""]/p[2]/input"));
                var btnLogin = _driver.FindElement(By.XPath(@"//*[@id=""form-login""]/p[3]/input"));
                username.SendKeys(this.email);
                password.SendKeys(this.passWord);
                btnLogin.Click();
                _driver.Navigate().GoToUrl(jobListUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception during login");
                Console.WriteLine(ex.ToString());
                throw;
            }


         
        }

        protected override void WriteToFile()
        {
            Newtonsoft.Json.JsonSerializer serializer = new JsonSerializer();
            //serialize object directly into file 
            var json = JsonConvert.SerializeObject(topCvModels, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(outPutPath+"Topcv.json", json);
            Console.WriteLine("WRITE END");
        }

       
    }
}
