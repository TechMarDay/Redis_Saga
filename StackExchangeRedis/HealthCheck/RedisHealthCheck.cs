using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace StackExchangeRedis.HealthCheck
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _redisCache;
        private readonly IConfiguration configuration;

        public RedisHealthCheck(IConfiguration configuration, IConnectionMultiplexer redisCache)
        {
            _redisCache = redisCache;
            this.configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                foreach (var endPoint in _redisCache.GetEndPoints(configuredOnly: true))
                {
                    var server = _redisCache.GetServer(endPoint);

                    if (server.ServerType != ServerType.Cluster)
                    {
                        await _redisCache.GetDatabase().PingAsync();
                        await server.PingAsync();
                    }
                    else
                    {
                        var clusterInfo = await server.ExecuteAsync("CLUSTER", "INFO");

                        if (clusterInfo is object && !clusterInfo.IsNull)
                        {
                            if (!clusterInfo.ToString()!.Contains("cluster_state:ok"))
                            {
                                return new HealthCheckResult(context.Registration.FailureStatus, description: $"CLUSTER is not is healthy for endpoint {endPoint}");
                            }
                        }
                        else
                        {
                            return new HealthCheckResult(context.Registration.FailureStatus, description: $"CLUSTER unhealthy for endpoint {endPoint}");
                        }
                    }
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
