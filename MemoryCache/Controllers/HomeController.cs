using MemoryCache.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace MemoryCache.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache = null)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index()
        {

            var cachedValue = await _memoryCache.GetOrCreateAsync<string>("product",cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
                return Task.FromResult($"DateTime: {DateTime.Now}");
            });

            ViewBag.CachedValue = cachedValue;

            var productList = new List<Product>();
            var productCached = await _memoryCache.GetOrCreateAsync<List<Product>>("productList", cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
                for (int i = 0; i < 1000; i++) {
                    productList.Add(new Product
                    {
                        Name = i.ToString(),
                        Id = i,
                        CreatedDate = DateTime.Now,
                    });
                }

                return Task.FromResult(productList);
            });

            ViewBag.ProductList = productCached;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
