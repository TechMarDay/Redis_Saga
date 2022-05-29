using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RedisExtensionsCaching.Models;
using System.Text;
using System.Text.Json;

namespace RedisExtensionsCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _cache;
        public CourseController(MyDbContext context, IConfiguration configuration, IDistributedCache cache)
        {
            _dbContext = context;
            _configuration = configuration;
            _cache = cache;
        }

        [HttpGet("MockCourse/{authorId}")]
        public async Task<IActionResult> MockCourse(string authorId)
        {
            for(var i = 0; i < 200; i++)
            {
                var course = new Course
                {
                    Name = $"Name {i}",
                    Description = $"Description {i}",
                    Author = $"Author {i}",
                    AuthorId = authorId,
                    Link = $"Link {i}",
                };
               await _dbContext.Courses.AddAsync(course);
            }

            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("GetAll/{authorId}/{enableCache}")]
        public async Task<List<Course>> GetAll(string authorId, bool enableCache)
        {
            if (!enableCache)
            {
                return await _dbContext.Courses
                    .Where(x => x.AuthorId == authorId)
                    .OrderByDescending(x => x.Id).ToListAsync();
            }

            var cacheKey = authorId;
            // Trying to get data from the Redis cache
            byte[] cachedData = await _cache.GetAsync(cacheKey);
            var courses = new List<Course>();
            if (cachedData != null)
            {
                // If the data is found in the cache, encode and deserialize cached data.
                var cachedDataString = Encoding.UTF8.GetString(cachedData);
                courses = JsonSerializer.Deserialize<List<Course>>(cachedDataString);
            }
            else
            {
                // If the data is not found in the cache, then fetch data from database
                courses = await _dbContext.Courses
                    .Where(x => x.AuthorId == authorId)
                    .OrderByDescending(x => x.Id).ToListAsync();

                // Serializing the data
                string cachedDataString = JsonSerializer.Serialize(courses);
                var dataToCache = Encoding.UTF8.GetBytes(cachedDataString);

                // Setting up the cache options
                //DistributedCacheEntryOptions options = new DistributedCacheEntryOptions()
                //    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                //    .SetSlidingExpiration(TimeSpan.FromMinutes(3));

                // Add the data into the cache
                await _cache.SetAsync(cacheKey, dataToCache);
            }

            return courses;
        }
    }
}
