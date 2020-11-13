using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Model
{
    public class CUrl
    {
        public Dictionary<string,string> Headers { set; get; }
        public string FormContent { set; get; }
        public string BaseUrl { set; get; }

        public CUrl()
        {
            Headers = new Dictionary<string, string>();
        }

        public void ReadFile()
        {


        }



    }
}
