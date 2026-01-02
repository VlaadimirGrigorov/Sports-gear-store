using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGearStore.Data;
using SportsGearStore.Models;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly SportsGearStoreDbContext _context;

    public OrdersController(SportsGearStoreDbContext context)
    {
        _context = context;
    }

    // CREATE ORDER FROM CART
    [HttpPost]
    public async Task<IActionResult> CreateOrder()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId.Value);

        if (cart == null || !cart.Items.Any())
        {
            return BadRequest("Cart is empty.");
        }

        var order = new Order
        {
            UserId = userId.Value
        };

        foreach (var item in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        _context.Orders.Add(order);

        // изчистваме cart-а
        _context.CartItems.RemoveRange(cart.Items);

        await _context.SaveChangesAsync();

        return Ok("Order created successfully.");
    }

    // GET MY ORDERS
    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();

        var orders = await _context.Orders
            .Where(o => o.UserId == userId.Value)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return Ok(orders);
    }

    // HELPER
    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return null;

        return int.TryParse(claim.Value, out var id) ? id : null;
    }
}
