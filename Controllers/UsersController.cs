using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PropertyFavoritesApi.Data;
using PropertyFavoritesApi.Models;

namespace PropertyFavoritesApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/users
    [HttpGet]
    public async Task<string> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        
        var result = new JArray();
        foreach (var user in users)
        {
            var userObj = new JObject
            {
                ["id"] = user.Id,
                ["username"] = user.Username,
                ["createdAt"] = user.CreatedAt
            };
            result.Add(userObj);
        }
        
        return result.ToString();
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<string> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
        {
            var error = new JObject
            {
                ["error"] = "ไม่พบผู้ใช้"
            };
            return error.ToString();
        }

        var result = new JObject
        {
            ["id"] = user.Id,
            ["username"] = user.Username,
            ["createdAt"] = user.CreatedAt
        };
        
        return result.ToString();
    }

    // POST: api/users
    [HttpPost]
    public async Task<string> CreateUser([FromBody] JObject data)
    {
        var username = data["username"]?.ToString();
        
        if (string.IsNullOrEmpty(username))
        {
            var error = new JObject { ["error"] = "กรุณาระบุ username" };
            return error.ToString();
        }

        var exists = await _context.Users.AnyAsync(u => u.Username == username);
        if (exists)
        {
            var error = new JObject { ["error"] = "ชื่อผู้ใช้นี้ถูกใช้แล้ว" };
            return error.ToString();
        }

        var user = new User
        {
            Username = username,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = new JObject
        {
            ["id"] = user.Id,
            ["username"] = user.Username,
            ["createdAt"] = user.CreatedAt
        };
        
        return result.ToString();
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<string> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null)
        {
            var error = new JObject { ["error"] = "ไม่พบผู้ใช้" };
            return error.ToString();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        var result = new JObject { ["message"] = "ลบผู้ใช้สำเร็จ" };
        return result.ToString();
    }
}
