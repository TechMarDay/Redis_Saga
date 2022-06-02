using MemoryCache.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace MemoryCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public CourseController(MyDbContext context, IMemoryCache memoryCache)
        {
            _dbContext = context;
            _memoryCache = memoryCache;
        }

        //https://localhost:7082/api/Course/GetCourseByAuthorWithCache/author2
        [HttpGet]
        [Route("GetCourseByAuthorWithCache/{authorId}")]
        public async Task<List<Course>?> GetCourseByAuthorWithCache(string authorId)
        {
            var courses = new List<Course>();
            if(!_memoryCache.TryGetValue(authorId, out courses))
            {
                courses = await _dbContext.Courses
                    .Where(x => x.AuthorId == authorId)
                    .OrderByDescending(x => x.Id).ToListAsync();
                _memoryCache.Set(authorId, courses);
            }    
            return courses;
        }
    }
}
