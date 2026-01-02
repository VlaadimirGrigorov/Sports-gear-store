using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGearStore.Data;
using SportsGearStore.Models;
using System.Security.Claims;

namespace SportsGearStore.Controllers;

[Authorize]
[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly SportsGearStoreDbContext _context;

    public CartController(SportsGearStoreDbContext context)
    {
        _context = context;
    }

    // ==========================
    // GET CART
    // ==========================
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value);

        if (cart == null)
        {
            return Ok(new { items = new List<object>() });
        }

        return Ok(cart);
    }

    // ==========================
    // ADD PRODUCT TO CART
    // ==========================
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
        if (!productExists)
            return BadRequest("Invalid product.");

        var cart = await GetOrCreateCart(userId.Value);

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity
            });
        }
        else
        {
            item.Quantity += quantity;
        }

        await _context.SaveChangesAsync();
        return Ok("Product added to cart.");
    }

    // ==========================
    // UPDATE QUANTITY
    // ==========================
    [HttpPut("update")]
    public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
    {
        if (quantity <= 0)
            return BadRequest("Quantity must be greater than 0.");

        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value);

        if (cart == null)
            return NotFound("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            return NotFound("Product not in cart.");

        item.Quantity = quantity;

        await _context.SaveChangesAsync();
        return Ok("Quantity updated.");
    }

    // ==========================
    // REMOVE FROM CART
    // ==========================
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value);

        if (cart == null)
            return NotFound("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            return NotFound("Product not in cart.");

        cart.Items.Remove(item);

        await _context.SaveChangesAsync();
        return Ok("Product removed from cart.");
    }

    // ==========================
    // HELPERS
    // ==========================
    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return null;

        return int.TryParse(claim.Value, out var id) ? id : null;
    }

    private async Task<Cart> GetOrCreateCart(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return cart;
    }
}
