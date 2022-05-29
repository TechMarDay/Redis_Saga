using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BenchmarkHandler
{
    public class BenchmarkHarness
    {
        [Params(5, 100)]
        public int IterationCount;

        private static readonly HttpClient client = new HttpClient();

        [Benchmark]
        public async Task GetCourseWithoutRedisCacheAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            for (int i = 0; i < IterationCount; i++)
            {
                await client.GetAsync(@"https://localhost:7226/api/Course/GetAll/author2/false");
                //await client.GetAsync(@"https://localhost:7097/api/Course/GetAll/author2/false");
            }
        }

        [Benchmark]
        public async Task GetCourseWithRedisCacheAsync()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            for (int i = 0; i < IterationCount; i++)
            {
                await client.GetAsync(@"https://localhost:7226/api/Course/GetAll/author2/true");
                //await client.GetAsync(@"https://localhost:7097/api/Course/GetAll/author2/true");
            }
        }

    }
}
