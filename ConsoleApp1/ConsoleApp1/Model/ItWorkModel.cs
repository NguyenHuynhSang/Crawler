using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    [JsonObject]
    public class ItWorkModel
    {
        public int id { set; get; }
        public string CompanyName { set; get; }
        public string JobName { set; get; }
        public List<String> Skills { set; get; }

        public List<String> SkillsExperience { set; get; }

        public string Level { set; get; }


        public ItWorkModel()
        {
            Skills = new List<string>();
            SkillsExperience = new List<string>();
        }
    }

    public class WorkList
    {
        public List<ItWorkModel> WorkModels { set; get; }

        public WorkList()
        {
            WorkModels = new List<ItWorkModel>();
        }


    }

}
