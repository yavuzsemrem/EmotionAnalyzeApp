using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmotionAnalyzeApi.Data;
using EmotionAnalyzeApi.Models;

namespace EmotionAnalyzeApi.Controllers;

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
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return user;
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // GET: api/users/5/messages
    [HttpGet("{id}/messages")]
    public async Task<ActionResult<IEnumerable<Message>>> GetUserMessages(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return await _context.Messages
            .Where(m => m.UserId == id)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}
