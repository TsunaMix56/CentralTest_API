using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PropertyFavoritesApi.Data;
using PropertyFavoritesApi.Models;

namespace PropertyFavoritesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PropertiesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/properties
    [HttpGet]
    public async Task<string> GetProperties()
    {
        var properties = await _context.Properties.ToListAsync();
        
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

    // GET: api/properties/5
    [HttpGet("{id}")]
    public async Task<string> GetProperty(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        
        if (property == null)
        {
            var error = new JObject { ["error"] = "ไม่พบอสังหาริมทรัพย์" };
            return error.ToString();
        }

        var result = new JObject
        {
            ["id"] = property.Id,
            ["title"] = property.Title,
            ["price"] = property.Price,
            ["location"] = property.Location,
            ["imageUrl"] = property.ImageUrl,
            ["description"] = property.Description
        };
        
        return result.ToString();
    }

    // POST: api/properties
    [HttpPost]
    public async Task<string> CreateProperty([FromBody] JObject data)
    {
        var property = new Property
        {
            Title = data["title"]?.ToString() ?? "",
            Price = data["price"]?.ToObject<decimal>() ?? 0,
            Location = data["location"]?.ToString() ?? "",
            ImageUrl = data["imageUrl"]?.ToString(),
            Description = data["description"]?.ToString()
        };

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        var result = new JObject
        {
            ["id"] = property.Id,
            ["title"] = property.Title,
            ["price"] = property.Price,
            ["location"] = property.Location,
            ["imageUrl"] = property.ImageUrl,
            ["description"] = property.Description
        };
        
        return result.ToString();
    }

    // PUT: api/properties/5
    [HttpPut("{id}")]
    public async Task<string> UpdateProperty(int id, [FromBody] JObject data)
    {
        var property = await _context.Properties.FindAsync(id);
        
        if (property == null)
        {
            var error = new JObject { ["error"] = "ไม่พบอสังหาริมทรัพย์" };
            return error.ToString();
        }

        property.Title = data["title"]?.ToString() ?? property.Title;
        property.Price = data["price"]?.ToObject<decimal>() ?? property.Price;
        property.Location = data["location"]?.ToString() ?? property.Location;
        property.ImageUrl = data["imageUrl"]?.ToString() ?? property.ImageUrl;
        property.Description = data["description"]?.ToString() ?? property.Description;

        await _context.SaveChangesAsync();

        var result = new JObject
        {
            ["id"] = property.Id,
            ["title"] = property.Title,
            ["price"] = property.Price,
            ["location"] = property.Location,
            ["imageUrl"] = property.ImageUrl,
            ["description"] = property.Description
        };
        
        return result.ToString();
    }

    // DELETE: api/properties/5
    [HttpDelete("{id}")]
    public async Task<string> DeleteProperty(int id)
    {
        var property = await _context.Properties.FindAsync(id);
        
        if (property == null)
        {
            var error = new JObject { ["error"] = "ไม่พบอสังหาริมทรัพย์" };
            return error.ToString();
        }

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();

        var result = new JObject { ["message"] = "ลบอสังหาริมทรัพย์สำเร็จ" };
        return result.ToString();
    }
}
