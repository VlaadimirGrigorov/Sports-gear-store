using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGearStore.Data;
using SportsGearStore.Models;
using System.Security.Claims;

namespace SportsGearStore.Controllers;

[Route("api/product-tags")]
[ApiController]
[Authorize]
public class ProductTagsController : ControllerBase
{
    private readonly SportsGearStoreDbContext _context;

    public ProductTagsController(SportsGearStoreDbContext context)
    {
        _context = context;
    }

    // ADD TAG TO PRODUCT
    [HttpPost]
    public async Task<IActionResult> AddTagToProduct(int productId, int tagId)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        var tagExists = await _context.Tags.AnyAsync(t => t.Id == tagId);

        if (!productExists || !tagExists)
        {
            return BadRequest("Invalid product or tag.");
        }

        var exists = await _context.ProductTags.AnyAsync(pt => pt.ProductId == productId && pt.TagId == tagId);

        if (exists)
        {
            return BadRequest("Tag already assigned to product.");
        }

        _context.ProductTags.Add(new ProductTag
        {
            ProductId = productId,
            TagId = tagId
        });

        await _context.SaveChangesAsync();
        return Ok("Tag added to product.");
    }

    // REMOVE TAG FROM PRODUCT
    [HttpDelete]
    public async Task<IActionResult> RemoveTagFromProduct(int productId, int tagId)
    {
        if (!IsAdmin())
        {
            return Forbid();
        }

        var productTag = await _context.ProductTags
            .FirstOrDefaultAsync(pt => pt.ProductId == productId && pt.TagId == tagId);

        if (productTag == null)
        {
            return NotFound("Tag not assigned to this product.");
        }

        _context.ProductTags.Remove(productTag);
        await _context.SaveChangesAsync();

        return Ok("Tag removed from product.");
    }

    // GET TAGS FOR PRODUCT
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetTagsForProduct(int productId)
    {
        var tags = await _context.ProductTags
            .Where(pt => pt.ProductId == productId)
            .Join(
                _context.Tags,
                pt => pt.TagId,
                t => t.Id,
                (pt, t) => new
                {
                    t.Id,
                    t.Name
                }
            )
            .ToListAsync();

        return Ok(tags);
    }

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
