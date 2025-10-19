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
            .OrderBy(m => m.CreatedAt)  // Eski mesajlar önce, yeni mesajlar altta
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
        client.Timeout = TimeSpan.FromSeconds(60); // Model yavaş olabilir
        
        // Flask API endpoint (daha güvenilir)
        var apiUrl = "http://127.0.0.1:7861/analyze";
        
        try
        {
            // Flask API formatı: { "text": "mesaj" }
            var requestBody = new { text };
            
            Console.WriteLine($"[DEBUG] AI Servisine istek atılıyor: {apiUrl}");
            Console.WriteLine($"[DEBUG] Mesaj: {text}");
            
            var response = await client.PostAsJsonAsync(apiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"[DEBUG] Response Status: {response.StatusCode}");
            Console.WriteLine($"[DEBUG] Response: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}...");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"AI servisi hata döndü: {response.StatusCode}");
            }

            // Flask API direkt EmotionScores formatında döner
            // { "Pozitif": 0.x, "Negatif": 0.x, "Nötr": 0.x }
            var scores = await response.Content.ReadFromJsonAsync<EmotionScores>();
            
            if (scores == null)
            {
                throw new Exception("Duygu analizi sonucu parse edilemedi");
            }
            
            Console.WriteLine($"[SUCCESS] ✅ Pozitif: {scores.Pozitif:P0}, Negatif: {scores.Negatif:P0}, Nötr: {scores.Nötr:P0}");
            return scores;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ERROR] ❌ AI servisine bağlanılamadı: {ex.Message}");
            Console.WriteLine($"[ERROR] AI servisinin çalıştığından emin olun: http://127.0.0.1:7860");
            throw new Exception("AI servisi çalışmıyor. Lütfen 'python app.py' komutuyla başlatın.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] ❌ Duygu analizi hatası: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack: {ex.StackTrace}");
            throw;
        }
    }
}

public class EmotionScores
{
    public double Pozitif { get; set; }
    public double Negatif { get; set; }
    public double Nötr { get; set; }
}

