using Newtonsoft.Json;
using StackExchange.Redis;

//Configuration
var cfg = new ConfigurationOptions
{
    AllowAdmin = true,
};
cfg.EndPoints.Add("localhost");
var connStr = cfg.ToString();
using var redis = ConnectionMultiplexer.Connect(cfg);
var db = redis.GetDatabase();
var server = redis.GetServer("localhost", 6379);

//Test connection
var pong = await db.PingAsync();
Console.WriteLine(pong);

var allKeys = server.Keys().Select(k => k.ToString());
Console.WriteLine("Current list keys: {0}", JsonConvert.SerializeObject(allKeys));

//Delete all the keys of the database.
await server.FlushDatabaseAsync();

//String
await db.StringSetAsync("Course_CSharp", "C# from zero to hero",
    expiry: TimeSpan.FromMinutes(2));
var courseValue = await db.StringGetAsync("Course_CSharp");
Console.WriteLine("String value of key Course_CSharp: {0}", courseValue);

//Hash
/*Hash is a data type that stores a hash table of key-value pairs, 
 * where the keys are arranged randomly, in no order at all. 
 * Hash is often used to store objects (users have fields name, age, address,...). 
 * Each hash can store 2^32 - 1 key-value pair
 */
await db.HashSetAsync("user", new[]
           {
                new HashEntry("name", "Thao"),
                new HashEntry("email", "ThaoYeuTho@gmail.com"),
                new HashEntry("age", 18),
            });
var user = await db.HashGetAllAsync("user");
Console.WriteLine("Hash value: {0}", JsonConvert.SerializeObject(user.ToDictionary()));

//List
/*The list data type in redis is simply a list of strings sorted by insertion order
 */
await db.ListRightPushAsync("ListUser", new RedisValue[] { "User_1", "User3","User_2" });
var listUser = await db.ListRangeAsync("ListUser");
Console.WriteLine("List value: {0}", JsonConvert.SerializeObject(listUser));

//SET
/*Set is an unsorted collection of strings. 
 * Set supports operations to add elements, read, delete each element, 
 * check the occurrence of elements in the set 
 * with the default time of O(1) 
 * regardless of the number of elements of that set. 
 * The maximum number of elements in a set is 2^32 - 1 (4294967295, 
 * which is more than 4 billion elements in each set)
 */
await db.SetAddAsync("set_key", new RedisValue[] { "JS", "C#", "Java", "Go" });
var setVal = await db.SetMembersAsync("set_key");
Console.WriteLine("Set value: {0}", JsonConvert.SerializeObject(setVal));


//SORTED SET (ZSET)
/*Is a set of non-repeating strings, where each element is a map of 
 * a string (member) and a floating-point number (score), 
 * the list is ordered by this score, the elements are unique, 
 * the score is repeatable
 */
await db.SortedSetAddAsync("RankerUsers", new[]
            {
                new SortedSetEntry("User_1", 1),
                new SortedSetEntry("User_3",4),
                new SortedSetEntry("User_2", 3),
                new SortedSetEntry("User_4", 2),
                //new SortedSetEntry( "Yukihiro Matsumoto", 5),
            });
var sortedSet = await db.SortedSetRangeByRankAsync("RankerUsers");
Console.WriteLine("Sorted set value: {0}", JsonConvert.SerializeObject(sortedSet));


//HyperLogLog
/*Redis HyperLogLog is an algorithm that uses randomization 
 * in order to provide an approximation of the number 
 * of unique elements in a set using just a constant, and small amount of memory.
HyperLogLog provides a very good approximation of the cardinality 
of a set even using a very small amount of memory around 12 kbytes per key 
with a standard error of 0.81%. 
There is no limit to the number of items you can count, unless you approach 264 items.
 */
await db.HyperLogLogAddAsync("tutorials", "Redis");
await db.HyperLogLogAddAsync("tutorials", "MongoDB");
await db.HyperLogLogAddAsync("tutorials", "MySql");
await db.HyperLogLogAddAsync("tutorials", "Postgress");
await db.HyperLogLogAddAsync("tutorials", "MongoDB");
await db.HyperLogLogAddAsync("tutorials", "MySql");
await db.HyperLogLogAddAsync("tutorials", "MongoDB");
var hllCount = await db.HyperLogLogLengthAsync("tutorials");
Console.WriteLine("HyperLogLog value: {0}", hllCount);

Console.ReadKey();