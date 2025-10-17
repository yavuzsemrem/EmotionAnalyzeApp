using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmotionAnalyzeApi.Data;
using EmotionAnalyzeApi.Models;
using System.Net.Http;
using System.Text.Json;

namespace EmotionAnalyzeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;

    public MessagesController(
        ApplicationDbContext context,
        IHttpClientFactory clientFactory,
        IConfiguration configuration)
    {
        _context = context;
        _clientFactory = clientFactory;
        _configuration = configuration;
    }

    // GET: api/messages
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
    {
        return await _context.Messages
            .Include(m => m.User)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    // GET: api/messages/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Message>> GetMessage(int id)
    {
        var message = await _context.Messages
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            return NotFound();
        }

        return message;
    }

    // POST: api/messages
    [HttpPost]
    public async Task<ActionResult<Message>> CreateMessage(Message message)
    {
        // Kullanıcıyı kontrol et
        var user = await _context.Users.FindAsync(message.UserId);
        if (user == null)
        {
            return BadRequest("Geçersiz kullanıcı ID'si");
        }

        try
        {
            // Duygu analizi yap
            var emotionScores = await AnalyzeEmotion(message.Content);
            
            // Sonuçları kaydet
            message.PositiveScore = emotionScores.Pozitif;
            message.NegativeScore = emotionScores.Negatif;
            message.NeutralScore = emotionScores.Nötr;
            
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Duygu analizi sırasında bir hata oluştu: {ex.Message}");
        }
    }

    private async Task<EmotionScores> AnalyzeEmotion(string text)
    {
        var client = _clientFactory.CreateClient();
        var huggingFaceUrl = _configuration["HuggingFaceUrl"] ?? throw new InvalidOperationException("HuggingFace URL bulunamadı");
        
        var response = await client.PostAsJsonAsync(huggingFaceUrl, new { text });
        
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Duygu analizi API'si hata döndü: {response.StatusCode}");
        }

        var result = await response.Content.ReadFromJsonAsync<EmotionScores>();
        return result ?? throw new Exception("Duygu analizi sonucu alınamadı");
    }
}

public class EmotionScores
{
    public double Pozitif { get; set; }
    public double Negatif { get; set; }
    public double Nötr { get; set; }
}
