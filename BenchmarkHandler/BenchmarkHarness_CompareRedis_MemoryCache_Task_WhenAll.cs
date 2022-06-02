using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkHandler
{
    public class BenchmarkHarness_CompareRedis_MemoryCache_Task_WhenAll
    {
        [Params(5, 100)]
        public int IterationCount;

        private static readonly HttpClient client = new HttpClient();

        [Benchmark]
        public async Task GetCourseWithoutCacheAsync()
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            for (int i = 0; i < IterationCount; i++)
            {
                tasks.Add(
                    Task.Run(async () =>
                        await client.GetAsync(@"https://localhost:7097/api/Course/GetCourseByAuthorWithoutCache/author2")
                    ));
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task GetCourseWithRedisCacheAsync()
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            for (int i = 0; i < IterationCount; i++)
            {
                tasks.Add(
                    Task.Run(async () =>
                        await client.GetAsync(@"https://localhost:7097/api/Course/GetCourseByAuthorWithCache/author2")
                    ));
            }
            await Task.WhenAll(tasks);
        }

        [Benchmark]
        public async Task GetCourseWithMemoryCacheAsync()
        {
            var tasks = new List<Task<HttpResponseMessage>>();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            for (int i = 0; i < IterationCount; i++)
            {
                tasks.Add(
                    Task.Run(async () =>
                        await client.GetAsync(@"https://localhost:7082/api/Course/GetCourseByAuthorWithCache/author2")
                    ));
            }
            await Task.WhenAll(tasks);
        }
    }
}
