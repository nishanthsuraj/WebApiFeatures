// Ignore Spelling: Api

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiFeatures.Db;
using WebApiFeatures.Models;

namespace WebApiFeatures.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _shopContext;
        public ProductsController(ShopContext shopContext)
        {
            _shopContext = shopContext;

            _shopContext.Database.EnsureCreated(); // Ensures all the In-memory data is created.
        }

        [HttpGet]
        public IEnumerable<Product> GetAllProducts()
        {
            return _shopContext.Products.ToArray();
        }
    }
}
