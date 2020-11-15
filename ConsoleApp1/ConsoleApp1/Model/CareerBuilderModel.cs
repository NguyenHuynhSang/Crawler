using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class CareerBuilderModel
    {
        public string job_title_id { get; set; }
        public string job_title { get; set; }
        public string job_location_id { get; set; }
        public string job_location { get; set; }
        public string employer_name_id { get; set; }
        [JsonConverter(typeof(EscapeQuoteConverter))]
        public string employer_name { get; set; }
        public string job_level_id { get; set; }
        public string job_level { get; set; }
        public string salary { get; set; }
        public int from_salary { get; set; }
        public int to_salary { get; set; }
        public string unit_salary { get; set; }
        public string member { get; set; }
    }
    public class EscapeQuoteConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().Replace("'", "\\'"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var value = JToken.Load(reader).Value<string>();
            return value.Replace("\\'", "'");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }

}
