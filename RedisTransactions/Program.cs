using StackExchange.Redis;

string RedisConnectionString = "localhost";
ConnectionMultiplexer connection =
  ConnectionMultiplexer.Connect(RedisConnectionString);

var redis = connection.GetDatabase();

RedisKey alphaKey = "alphaKey";
RedisKey betaKey = "betaKey";
RedisKey gammaKey = "gammaKey";

redis.KeyDelete(alphaKey);
redis.KeyDelete(betaKey);
redis.KeyDelete(gammaKey);

var trans = redis.CreateTransaction();


trans.StringSetAsync(alphaKey, "JS");
trans.StringSetAsync(betaKey, "C#");
var exec = trans.ExecuteAsync();
var result = redis.Wait(exec);
var alphaValue = redis.StringGet(alphaKey);
var betaValue = redis.StringGet(betaKey);

Console.WriteLine($"Alpha key is {alphaValue} and result is {result}");
Console.WriteLine($"Beta key is {betaValue} and result is {result}");

//using conditions to watch keys
var condition = trans.AddCondition(Condition.KeyNotExists(gammaKey));
trans.StringSetAsync(gammaKey, "Java");
exec = trans.ExecuteAsync();
result = redis.Wait(exec);

var gammaValue = redis.StringGet(gammaKey);

Console.WriteLine($"Gamma key is {gammaValue} and result is {result}");

//Fail condition
condition = trans.AddCondition(Condition.KeyNotExists(gammaKey));
trans.StringSetAsync(gammaKey, "Java");
exec = trans.ExecuteAsync();

//result false 
result = redis.Wait(exec);

gammaValue = redis.StringGet(gammaKey);

//value is still 1 and result is false
Console.WriteLine($"Gamma key is {gammaValue} and result is {result}");

Console.ReadKey();