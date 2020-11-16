using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Support.UI;

namespace ConsoleApp1.Service
{
   public  abstract class BaseCrawlService
    {
        protected  readonly string email = @"nguyentuoc123789a@gmail.com";
        protected readonly string passWord = @"abc123456";

        protected readonly string outPutPath = @"../../../Output/";

        protected string loginUrl;
        protected string jobListUrl;
        


        protected IWebDriver _driver;
        protected WebDriverWait _wait;
         public BaseCrawlService()
        {
            _driver = new ChromeDriver();
        }


       
        protected Thread crawlThread;
        protected Thread extractThread;
        public void Process() 
        {
            Login();
            CrawlData();


        }

        protected abstract void Login();
        protected abstract void CrawlData();
        protected abstract void ExtractContent();
        protected abstract void WriteToFile();


    }
}
