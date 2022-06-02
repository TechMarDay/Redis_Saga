using BenchmarkDotNet.Running;
using BenchmarkHandler;

//BenchmarkRunner.Run<BenchmarkHarness_RedisExtensionsCaching>();
//BenchmarkRunner.Run<BenchmarkHarness_StackExchangeRedis>();
//BenchmarkRunner.Run<BenchmarkHarness_CompareRedis_MemoryCache>();

BenchmarkRunner.Run<BenchmarkHarness_CompareRedis_MemoryCache_Task_WhenAll>();

Console.ReadKey();
