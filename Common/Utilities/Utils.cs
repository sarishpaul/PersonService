using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Common.Utilities
{
    public class Utils
    {
        public static T DeSerializeObject<T>(string json)
        {           
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object DeserializeObject(string json)
        {

            return JsonConvert.DeserializeObject(json);
        }
        public static string SerializeObject<T>(T item)
        {
            return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFK" });
        }

        public static string SerializeObject(object item)
        {
            return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFK" });
        }
    }
}
