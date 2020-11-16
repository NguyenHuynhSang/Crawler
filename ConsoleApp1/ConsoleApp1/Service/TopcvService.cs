using ConsoleApp1;
using ConsoleApp1.Service;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Crawler.Model
{
    public class TopcvService : BaseCrawlService
    {
        
        public TopcvService():base()
        {
            this.loginUrl = @"https://www.topcv.vn/login";
            this.jobListUrl = @"https://www.topcv.vn/tim-viec-lam-it-phan-mem-c10026";
           
        }

        protected override void CrawlData()
        {
            
          
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
    }
}
