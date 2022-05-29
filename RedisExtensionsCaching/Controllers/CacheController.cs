using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace RedisExtensionsCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        public CacheController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpDelete]
        [Route("{authorId}")]
        public async Task<IActionResult> RemoveCache(string cacheKey)
        {
            await _cache.RemoveAsync(cacheKey);
            return Ok();
        }
    }
}
