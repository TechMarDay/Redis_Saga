using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RedisExtensionsCaching.Middleware;
using RedisExtensionsCaching.Models;
using Microsoft.Extensions.Caching.StackExchangeRedis;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyDbContext>(options => 
options.UseSqlServer(configuration.GetConnectionString("DBDemoConnection")));

builder.Services.AddStackExchangeRedisCache(options => 
{ options.Configuration = configuration["RedisCacheUrl"]; });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseResponseTime();

app.Run();
