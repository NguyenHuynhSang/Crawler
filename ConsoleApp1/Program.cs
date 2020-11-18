using ConsoleApp1.Service;
using Crawler.Model;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace ConsoleApp1
{

  




    class Program
    {

        public   enum EWEB
        {
            CareerBuilderService,
            TopcvService,
            VietNamWorkService,
            ItViecService
        }
        static void Main(string[] args)
        {
            List<BaseCrawlService> crawlServices = new List<BaseCrawlService>()
            {
                new CareerBuilderService(),
                new TopcvService(),
                new VietNamWorkService(),
                new ItViecService()

            };
            crawlServices[0].Process();




        }









    }
}
