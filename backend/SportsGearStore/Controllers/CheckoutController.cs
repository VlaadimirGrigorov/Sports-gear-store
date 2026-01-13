using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGearStore.Data;
using System.Security.Claims;
using Stripe.Checkout;

namespace SportsGearStore.Controllers;

[Authorize]
[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly SportsGearStoreDbContext _context;

    public CheckoutController(SportsGearStoreDbContext context)
    {
        _context = context;
    }

    [HttpPost("create-session")]
    public async Task<IActionResult> CreateSession()
    {
        // 1) вземи userId
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);

        // 2) вземи cart + items
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.Items.Any())
            return BadRequest("Cart is empty.");

        // 3) създай line items за Stripe
        var lineItems = new List<SessionLineItemOptions>();

        foreach (var item in cart.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
            if (product == null) continue;

            lineItems.Add(new SessionLineItemOptions
            {
                Quantity = item.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "usd", // или "eur"/"bgn" (ако ти трябва)
                    UnitAmount = (long)(product.Price * 100m), // Stripe работи в стотинки
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Name,
                        Description = product.Description
                    }
                }
            });
        }

        // 4) create Checkout session
        var options = new SessionCreateOptions
        {
            Mode = "payment",
            LineItems = lineItems,
            SuccessUrl = "http://localhost:5173/checkout-success/index.html",
            CancelUrl = "http://localhost:5173/cart/cart.html"
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        // връщаме url за redirect
        return Ok(new { url = session.Url });
    }
}
