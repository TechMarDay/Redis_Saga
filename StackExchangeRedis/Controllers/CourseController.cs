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

        [HttpGet]
        [Route("GetAll/{authorId}/{enableCache}")]
        public async Task<List<Course>?> GetAll(string authorId, bool enableCache)
        {
            if (!enableCache)
            {
                return await _dbContext.Courses
                    .Where(x => x.AuthorId == authorId)
                    .OrderByDescending(x => x.Id).ToListAsync();
            }

            var db = _redis.GetDatabase();
            var cacheKey = authorId;
            var courses = new List<Course>();
            var exists = await db.KeyExistsAsync(authorId);
            if (exists)
            {
                var cachedData = await db.StringGetAsync(cacheKey);
                var coursesdata = JsonConvert.DeserializeObject<IEnumerable<Course>>(cachedData);
                return coursesdata?.ToList();
            }
            else
            {
                // If the data is not found in the cache, then fetch data from database
                courses = await _dbContext.Courses
                    .Where(x => x.AuthorId == authorId)
                    .OrderByDescending(x => x.Id).ToListAsync();

                // Serializing the data
                db.StringSet(authorId,
                                    JsonConvert.SerializeObject(courses),
                                    expiry: TimeSpan.FromSeconds(60));
            }

            return courses;
        }
    }
}
