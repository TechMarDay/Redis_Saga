using RedisChatApp;
using StackExchange.Redis;

string RedisConnectionString = "localhost";
ConnectionMultiplexer connection =
  ConnectionMultiplexer.Connect(RedisConnectionString);

string ChatChannel = "Chat-Simple-Channel";
string userName = string.Empty;

// Enter name and put it into variable userName
Console.Write("Enter your name: ");
userName = Console.ReadLine();
// Create pub / sub
var pubsub = connection.GetSubscriber();

// Subscriber subscribes to a channel
pubsub.Subscribe(ChatChannel,
      (channel, message) => Message.MessageAction(message));

// Notify subscriber(s) if you're joining
pubsub.Publish(ChatChannel, $"'{userName}' joined the chat room.");

// Messaging here
while (true)
{
    pubsub.Publish(ChatChannel, $"{userName}: {Console.ReadLine()}  " +
        $"({DateTime.Now.Hour}:{DateTime.Now.Minute})");
}