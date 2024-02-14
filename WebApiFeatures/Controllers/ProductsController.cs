// Ignore Spelling: Api

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public ActionResult GetAllProducts()
        {
            return Ok(_shopContext.Products.ToList());
        }

        [HttpGet("async")]
        public async Task<ActionResult> GetAllProductsAsync()
        {
            return Ok(await _shopContext.Products.ToListAsync());
        }

        [HttpGet("{id}")]
        public ActionResult GetProduct(int id)
        {
            Product? isFound = _shopContext.Products.Find(id);
            if (isFound == null)
            {
                return NotFound();
            }

            return Ok(isFound);
        }

        [HttpGet("{id}/async")]
        public async Task<ActionResult> GetProductAsync(int id)
        {
            Product? isFound = await _shopContext.Products.FindAsync(id);
            if (isFound == null)
            {
                return NotFound();
            }

            return Ok(isFound);
        }
    }
}
