using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGearStore.Data;
using SportsGearStore.Models;
using System.Security.Claims;

namespace SportsGearStore.Controllers
{
    [Route("api/products")]
    [ApiController]
    //[Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly SportsGearStoreDbContext _context;
    
        public ProductsController(SportsGearStoreDbContext context)
        {
            _context = context;
        }
    
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }
    
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    
            if (product == null)
            {
                return NotFound();
            }
    
            return Ok(product);
        }
    
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Search term cannot be empty.");
            }
    
            var products = await _context.Products
                .Where(p =>
                    p.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
    
            return Ok(products);
        }

        // ADMIN ENDPOINTS

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Product updatedProduct)
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            if (id != updatedProduct.Id)
            {
                return BadRequest();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            product.CategoryId = updatedProduct.CategoryId;
            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommendedProducts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            // 1️⃣ намираме профила
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                return Ok(new List<object>());
            }

            // 2️⃣ user tags
            var userTagIds = await _context.UserProfileTags
                .Where(upt => upt.UserProfileId == profile.Id)
                .Select(upt => upt.TagId)
                .ToListAsync();

            if (!userTagIds.Any())
            {
                return Ok(new List<object>());
            }

            // 3️⃣ matching
            var matches = await _context.ProductTags
                .Where(pt => userTagIds.Contains(pt.TagId))
                .GroupBy(pt => pt.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    MatchScore = g.Count()
                })
                .OrderByDescending(x => x.MatchScore)
                .Take(10)
                .ToListAsync();

            var productIds = matches.Select(m => m.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                MatchScore = matches.First(m => m.ProductId == p.Id).MatchScore
            });

            return Ok(result);
        }

        // HELPERS
        private bool IsAdmin()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return false;
            }

            var user = _context.Users.Find(int.Parse(userId));
            return user?.IsAdmin == true;
        }
    }
}