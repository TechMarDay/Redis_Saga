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
        public CourseController(MyDbContext context, 
            IConfiguration configuration, 
            IDistributedCache cache)
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

        //https://localhost:7226/api/Course/GetCourseByAuthorWithCache/author2
        [HttpGet]
        [Route("GetCourseByAuthorWithCache/{authorId}")]
        public async Task<List<Course>> GetCourseByAuthorWithCache(string authorId)
        {
            var cacheKey = authorId;
            byte[] cachedData = await _cache.GetAsync(cacheKey);
            var courses = new List<Course>();
            if (cachedData != null)
            {
                var cachedDataString = Encoding.UTF8.GetString(cachedData);
                courses = JsonSerializer.Deserialize<List<Course>>(cachedDataString);
            }
            else
            {
                courses = await _dbContext.Courses
                    .Where(x => x.AuthorId == authorId)
                    .OrderByDescending(x => x.Id).ToListAsync();
                var dataToCache = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(courses));
                await _cache.SetAsync(cacheKey, dataToCache);
            }
            return courses;
        }

        //https://localhost:7226/api/Course/GetCourseByAuthorWithoutCache/author2
        [HttpGet]
        [Route("GetCourseByAuthorWithoutCache/{authorId}")]
        public async Task<List<Course>> GetCourseByAuthorWithoutCache(string authorId)
        {
            return await _dbContext.Courses
                .Where(x => x.AuthorId == authorId).ToListAsync();
        }
    }
}
