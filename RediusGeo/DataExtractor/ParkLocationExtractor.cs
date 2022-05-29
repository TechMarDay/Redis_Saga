using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RediusGeo.DataExtractor
{
    public class ParkLocationExtractor
    {
        public static IEnumerable<Tuple<double, double, string>> Extract()
        {
            using (StreamReader r = new StreamReader(@"C:\MyWorkspace\Redis\RedisNET6\RediusGeo\park_location.json"))
            {
                string json = r.ReadToEnd();
                dynamic baseketBallContent = JObject.Parse(json);

                foreach (var feature in baseketBallContent.features)
                {
                    var parkName = feature?.attributes?.NAME?.ToString();
                    var coordinates = feature?.geometry;
                    var longtitue = Convert.ToDouble(coordinates?.x);
                    var latitude = Convert.ToDouble(coordinates?.y);
                    yield return new Tuple<double, double, string>(longtitue, latitude, parkName);
                }
            }


        }
    }
}
