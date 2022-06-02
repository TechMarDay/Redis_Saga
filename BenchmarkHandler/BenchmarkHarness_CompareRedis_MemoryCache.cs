using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkHandler
{
    public class BenchmarkHarness_CompareRedis_MemoryCache
    {
        [Params(5, 100)]
        public int IterationCount;

        private static readonly HttpClient client = new HttpClient();

        [Benchmark]
        public async Task GetCourseWithoutCacheAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            for (int i = 0; i < IterationCount; i++)
            {
                await client.GetAsync(@"https://localhost:7097/api/Course/GetCourseByAuthorWithoutCache/author2");
            }
        }

        [Benchmark]
        public async Task GetCourseWithRedisCacheAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            for (int i = 0; i < IterationCount; i++)
            {
                await client.GetAsync(@"https://localhost:7097/api/Course/GetCourseByAuthorWithCache/author2");
            }
        }

        [Benchmark]
        public async Task GetCourseWithMemoryCacheAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            for (int i = 0; i < IterationCount; i++)
            {
                await client.GetAsync(@"https://localhost:7082/api/Course/GetCourseByAuthorWithCache/author2");
            }
        }
    }
}
