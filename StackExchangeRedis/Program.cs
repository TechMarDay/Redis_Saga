using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using StackExchangeRedis.HealthCheck;
using StackExchangeRedis.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var multiplexer = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("RedisDBDemoConnection")));
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHealthChecks("/healthcheck");

app.Run();



//ConfigurationOptions config = new ConfigurationOptions
//{
//    EndPoints =
//    {
//        { "redis0", 6379 },
//        { "redis1", 6380 }
//    }
//};

//var multiplexer = ConnectionMultiplexer.Connect(config);