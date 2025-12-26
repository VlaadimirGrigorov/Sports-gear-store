using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGearStore.Data;
using SportsGearStore.Models;

namespace SportsGearStore.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly SportsGearStoreDbContext _context;

    public ProductsController(SportsGearStoreDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }
}