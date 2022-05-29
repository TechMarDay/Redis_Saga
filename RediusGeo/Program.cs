using RediusGeo.DataExtractor;
using StackExchange.Redis;

string RedisConnectionString = "localhost";
ConnectionMultiplexer connection =
  ConnectionMultiplexer.Connect(RedisConnectionString);

var redis = connection.GetDatabase();
var list = new List<GeoEntry>();
foreach (var item in ParkLocationExtractor.Extract())
{
    var longtitue = Convert.ToDouble(item.Item1);
    var latitude = Convert.ToDouble(item.Item2);
    var name = item.Item3;
    list.Add(new GeoEntry(longtitue, latitude, name));
}

var key = "locations";
//clear out the data
redis.KeyDelete(key, CommandFlags.FireAndForget);

//add all the geoentry
redis.GeoAdd(key, list.ToArray());

//find the distance between parks
var val = redis.GeoDistance(key, "Cong vien Lang Hoa", 
    "Cong vien Gia Dinh", GeoUnit.Kilometers);

Console.WriteLine($"Distance from Cong Vien lang Hoa to Cong vien Gia Dinh is {val} km");

//get the position of Cong vien Hoang Van Thu
var pos = redis.GeoPosition(key, "Cong vien Hoang Van Thu");

if(pos != null)
Console.WriteLine($"Cong vien Hoang Van Thu is located at: Lat {pos.Value.Latitude}, " +
    $"Long {pos.Value.Longitude} ");

//lets say we are in Saigon Technology
var currentLatitude= 106.65006634617346;
var currentLongtitude = 10.7991283316642;

//find me all the locations of parks nearby
var results = redis.GeoRadius(key, currentLatitude, currentLongtitude, 
    5, GeoUnit.Kilometers, -1, Order.Ascending, GeoRadiusOptions.WithCoordinates);
Console.WriteLine("Find me all the locations of parks nearby STS");
foreach (var geoRadiusResult in results)
{
    Console.WriteLine($"- {geoRadiusResult.Member}, Position {geoRadiusResult.Position}");
}

Console.ReadKey();