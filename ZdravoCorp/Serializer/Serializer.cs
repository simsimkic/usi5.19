using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZdravoCorp.Model;
namespace ZdravoCorp.Serializer
{
    public class Serializer<T> 
    {
        public  Serializer() { }

        public void ToJSON(string fileName, List<T> objects)
        {
            //string json = JsonConvert.SerializeObject(objects, Formatting.Indented,
              //  new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
              string json_input = JsonConvert.SerializeObject(objects , Newtonsoft.Json.Formatting.Indented);
           File.WriteAllText(fileName, json_input);
        }

        public List<T> FromJSON(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            var jsonString = reader.ReadToEnd();
            var settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffZ" // set the custom date format string
            };
            List<T> objects = JsonConvert.DeserializeObject<List<T>>(jsonString, settings);

            return objects;
        }

    }
}
