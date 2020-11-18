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

    public enum webSource
    {
        VietNamWork = 1,
        ITViet = 2,

    }




    class Program
    {
  
        public static ItViecService itViecService;
        public static VietNamWorkService vietNamWorkService;
        public static CareerBuilderService careerBuilderService;
        public static BaseCrawlService topcvService;


        static void Main(string[] args)
        {
            //  itViecService = new ItViecService();
            //  vietNamWorkService = new VietNamWorkService();
            // careerBuilderService = new CareerBuilderService();
            // itViecService.Process();
            //  vietNamWorkService.CrawlData();
            //topcvService = new TopcvService();

            //topcvService.Process();

            BaseCrawlService carviet = new CareerBuilderService();
            carviet.Process();



        }









    }
}
