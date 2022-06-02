using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using StackExchangeRedis.Models;

namespace StackExchangeRedis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IConnectionMultiplexer _redis;
        public CourseController(MyDbContext context, IConfiguration configuration,
            IConnectionMultiplexer redis)
        {
            _dbContext = context;
            _configuration = configuration;
            _redis = redis;
        }

        //https://localhost:7097/api/Course/GetCourseByAuthorWithCache/author2
        [HttpGet]
        [Route("GetCourseByAuthorWithCache/{authorId}")]
        public async Task<List<Course>?> GetCourseByAuthorWithCache(string authorId)
        {
            var redisDb = _redis.GetDatabase();
            var courses = new List<Course>();
            var exists = await redisDb.KeyExistsAsync(authorId);
            if (exists)
            {
                var cachedData = await redisDb.StringGetAsync(authorId);
                var coursesdata = JsonConvert.DeserializeObject<IEnumerable<Course>>(cachedData);
                return coursesdata?.ToList();
            }
            else
            {
                courses = await _dbContext.Courses
                    .Where(x => x.AuthorId == authorId)
                    .OrderByDescending(x => x.Id).ToListAsync();
                await redisDb.StringSetAsync(authorId,
                                    JsonConvert.SerializeObject(courses));
                //expiry: TimeSpan.FromSeconds(60)
            }

            return courses;
        }

        //https://localhost:7097/api/Course/GetCourseByAuthorWithoutCache/author2
        [HttpGet]
        [Route("GetCourseByAuthorWithoutCache/{authorId}")]
        public async Task<List<Course>?> GetCourseByAuthorWithoutCache(string authorId)
        {
            return await _dbContext.Courses
                .Where(x => x.AuthorId == authorId)
                .OrderByDescending(x => x.Id).ToListAsync();
        }
    }
}
