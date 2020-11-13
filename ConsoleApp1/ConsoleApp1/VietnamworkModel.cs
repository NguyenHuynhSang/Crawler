using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Geoloc
    {
        public double lng { get; set; }
        public double lat { get; set; }
    }

    public class Benefit
    {
        public string benefitIconName { get; set; }
        public string benefitName { get; set; }
        public string benefitValue { get; set; }
        public int benefitId { get; set; }
    }

    public class Hit
    {
        public bool isManagementJob { get; set; }
        public int expiredDate { get; set; }
        public string jobTitle { get; set; }
        public int salaryMax { get; set; }
        public List<string> skills { get; set; }
        public bool isSalaryVisible { get; set; }
        public bool isShowLogo { get; set; }
        public string jobLevelVI { get; set; }
        public Geoloc _geoloc { get; set; }
        public int priorityOrder55 { get; set; }
        public int salaryMin { get; set; }
        public List<string> locationVIs { get; set; }
        public bool isMobileHotJob { get; set; }
        public bool isMobileTopJob { get; set; }
        public List<int> locationIds { get; set; }
        public int onlineDate { get; set; }
        public int jobId { get; set; }
        public int companyId { get; set; }
        public bool isShowLogoInSearchResult35 { get; set; }
        public bool isUrgentJob { get; set; }
        public bool isPremium { get; set; }
        public List<Benefit> benefits { get; set; }
        public bool isBoldAndRedJob { get; set; }
        public List<string> serviceCode { get; set; }
        public List<object> classifiedConfidenceRoles { get; set; }
        public int visibilityDisplay { get; set; }
        public int typeWorkingId { get; set; }
        public List<string> categoryVIs { get; set; }
        public List<string> jobLocations { get; set; }
        public int priorityOrder { get; set; }
        public bool isShowLogoInSearchResult { get; set; }
        public bool isPriorityJob { get; set; }
        public string alias { get; set; }
        public string company { get; set; }
        public List<string> categories { get; set; }
        public string timestamp { get; set; }
        public int priorityOrder35 { get; set; }
        public List<object> classifiedRoles { get; set; }
        public int mobileTopPriorityOrder { get; set; }
        public string companyLogo { get; set; }
        public int jobLevelId { get; set; }
        public bool isHeadhuntJob { get; set; }
        public int userId { get; set; }
        public string jobLevel { get; set; }
        public bool isTopManagementJob { get; set; }
        public List<int> categoryIds { get; set; }
        public List<object> classifiedConfidenceSkills { get; set; }
        public bool isShowLogoInSearchResult55 { get; set; }
        public List<string> locations { get; set; }
        public int publishedDate { get; set; }
        public List<object> classifiedSkills { get; set; }
        public string objectID { get; set; }
    }

    public class Result
    {
        public List<Hit> hits { get; set; }
        public int nbHits { get; set; }
        public int page { get; set; }
        public int nbPages { get; set; }
        public int hitsPerPage { get; set; }
        public bool exhaustiveNbHits { get; set; }
        public string query { get; set; }
        public string @params { get; set; }
        public string index { get; set; }
        public int processingTimeMS { get; set; }
    }

    public class VietNameWorkModle
    {
        public List<Result> results { get; set; }
    }


}
