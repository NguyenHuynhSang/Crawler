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
    public abstract class BaseCrawlService
    {
        /// <summary>
        /// Dùng chung cho 4 trang
        /// </summary>
        protected readonly string email = @"nguyentuoc123789a@gmail.com";
        protected readonly string passWord = @"abc123456";
        protected readonly string outPutPath = @"../../../Output/";

        /// <summary>
        /// đường dẫn login nếu cần thiết
        /// </summary>
        protected string loginUrl;

        /// <summary>
        /// base url của ds job
        /// </summary>
        protected string jobListUrl;

        protected IWebDriver _driver;
        protected WebDriverWait _wait;

        protected Thread crawlThread;
        protected Thread extractThread;

        /// <summary>
        /// Nếu trang cần dùng selenium thì contructor trang con gọi base, k thì thôi
        /// </summary>
        public BaseCrawlService()
        {
            _driver = new ChromeDriver();
            _wait = new OpenQA.Selenium.Support.UI.WebDriverWait(_driver, TimeSpan.FromSeconds(30.00));
        }



       
        /// <summary>
        /// Trang nào k cần dùng đến 2 thread thì overwrite lại
        /// </summary>
        public virtual void Process()
        {

            Login();
            crawlThread = new Thread(CrawlData);
            extractThread = new Thread(ExtractContent);
            crawlThread.Start();
            extractThread.Start();
            // wait for both of them finish
            // crawlThread.Join(extractThread);

        }

        protected virtual void Login() { Console.WriteLine("Website no need to login!!!"); }
        protected virtual void CrawlData()
        {
            Console.WriteLine("No need to CrawlData");
        }
        protected virtual void ExtractContent()
        {
            Console.WriteLine("No need to extract content");
        }
        /// <summary>
        /// ghi ra file json
        /// </summary>
        protected abstract void WriteToFile();


    }
}
