using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PropertyFavoritesApi.Data;
using PropertyFavoritesApi.Models;

namespace PropertyFavoritesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoritesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoritesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/favorites?userId=1
    [HttpGet]
    public async Task<string> GetFavorites([FromQuery] int? userId)
    {
        var query = _context.Favorites
            .Include(f => f.Property)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(f => f.UserId == userId.Value);
        }

        var favorites = await query.ToListAsync();

        var result = new JArray();
        foreach (var fav in favorites)
        {
            var favObj = new JObject
            {
                ["userId"] = fav.UserId,
                ["propertyId"] = fav.PropertyId,
                ["createdAt"] = fav.CreatedAt,
                ["property"] = new JObject
                {
                    ["id"] = fav.Property.Id,
                    ["title"] = fav.Property.Title,
                    ["price"] = fav.Property.Price,
                    ["location"] = fav.Property.Location,
                    ["imageUrl"] = fav.Property.ImageUrl
                }
            };
            result.Add(favObj);
        }

        return result.ToString();
    }

    // GET: api/favorites/user/1/properties
    [HttpGet("user/{userId}/properties")]
    public async Task<string> GetUserFavoriteProperties(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            var error = new JObject { ["error"] = "ไม่พบผู้ใช้" };
            return error.ToString();
        }

        var properties = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Property)
            .Select(f => f.Property)
            .ToListAsync();

        var result = new JArray();
        foreach (var prop in properties)
        {
            var propObj = new JObject
            {
                ["id"] = prop.Id,
                ["title"] = prop.Title,
                ["price"] = prop.Price,
                ["location"] = prop.Location,
                ["imageUrl"] = prop.ImageUrl,
                ["description"] = prop.Description
            };
            result.Add(propObj);
        }

        return result.ToString();
    }

    // POST: api/favorites
    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
    {
        if (request == null || request.UserId == 0 || request.PropertyId == 0)
        {
            return BadRequest(new { error = "กรุณาระบุ userId และ propertyId" });
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            return NotFound(new { error = "ไม่พบผู้ใช้" });
        }

        var propertyExists = await _context.Properties.AnyAsync(p => p.Id == request.PropertyId);
        if (!propertyExists)
        {
            return NotFound(new { error = "ไม่พบอสังหาริมทรัพย์" });
        }

        // ตรวจสอบว่ามีรายการโปรดอยู่แล้วหรือไม่
        var existingFavorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == request.UserId && f.PropertyId == request.PropertyId);

        if (existingFavorite != null)
        {
            // ถ้ามีอยู่แล้ว ให้ลบออก (Unlike)
            _context.Favorites.Remove(existingFavorite);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                userId = request.UserId,
                propertyId = request.PropertyId,
                message = "ลบรายการโปรดสำเร็จ"
            });
        }

        // ถ้ายังไม่มี ให้เพิ่มใหม่ (Like)
        var favorite = new Favorite
        {
            UserId = request.UserId,
            PropertyId = request.PropertyId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            userId = favorite.UserId,
            propertyId = favorite.PropertyId,
            createdAt = favorite.CreatedAt,
            message = "เพิ่มรายการโปรดสำเร็จ"
        });
    }

    // DELETE: api/favorites/user/1/property/2
    [HttpDelete("user/{userId}/property/{propertyId}")]
    public async Task<string> RemoveFavorite(int userId, int propertyId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.PropertyId == propertyId);

        if (favorite == null)
        {
            var error = new JObject { ["error"] = "ไม่พบรายการโปรดนี้" };
            return error.ToString();
        }

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();

        var result = new JObject { ["message"] = "ลบรายการโปรดสำเร็จ" };
        return result.ToString();
    }

    // GET: api/favorites/share/1
    [HttpGet("share/{userId}")]
    public async Task<string> GetShareUrl(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            var error = new JObject { ["error"] = "ไม่พบผู้ใช้" };
            return error.ToString();
        }

        var shareUrl = $"{Request.Scheme}://{Request.Host}/api/favorites/user/{userId}/properties";

        var result = new JObject
        {
            ["userId"] = userId,
            ["shareUrl"] = shareUrl,
            ["message"] = "คัดลอก URL นี้เพื่อแชร์รายการโปรดของคุณ"
        };

        return result.ToString();
    }
}
