using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Service
{
   public  interface IBaseCrawlService
    {
        public void Process();
        protected void CrawlData();
        protected void ExtractContent();


    }
}
