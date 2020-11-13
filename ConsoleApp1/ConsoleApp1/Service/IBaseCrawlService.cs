using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Service
{
   public  interface IBaseCrawlService
    {
        public void Process();
        internal void CrawlData();
        internal void ExtractContent();


    }
}
