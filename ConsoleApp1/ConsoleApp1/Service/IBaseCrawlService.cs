using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConsoleApp1.Service
{
   public  abstract class BaseCrawlService
    {
        protected  readonly string email = @"nguyentuoc123789a@gmail.com";
        protected readonly string passWord = @"abc123456";

        protected string loginUrl;
        protected string jobListUrl;
        protected IWebDriver _driver;

         public BaseCrawlService()
        {
            _driver = new ChromeDriver();
        }


       
        protected Thread crawlThread;
        protected Thread extractThread;
        public void Process() 
        {
            Login();


        }

        protected abstract void Login();
        protected abstract void CrawlData();
        protected abstract void ExtractContent();


    }
}
