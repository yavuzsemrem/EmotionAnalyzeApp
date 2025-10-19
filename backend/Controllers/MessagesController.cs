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
            // Duygu analizi başarısız olsa bile mesajı kaydet
            Console.WriteLine($"[WARNING] ⚠️ Duygu analizi başarısız, mesaj neutral olarak kaydediliyor: {ex.Message}");
            
            message.PositiveScore = 0.33;
            message.NegativeScore = 0.33;
            message.NeutralScore = 0.34;
            
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMessage), new { id = message.Id }, message);
        }
    }

    private async Task<EmotionScores> AnalyzeEmotion(string text)
    {
        // Geçici: AI servisi API desteklemiyor, basit kelime analizi yap
        Console.WriteLine($"[INFO] 🤖 Geçici AI servisi (kelime analizi): {text}");
        
        // Türkçe duygu kelimeleri
        var positiveWords = new[] { 
            "iyi", "güzel", "harika", "mükemmel", "mutlu", "neşeli", "sevinçli",
            "hoş", "memnun", "başarılı", "başarı", "süper", "müthiş", "pozitif"
        };
        
        var negativeWords = new[] { 
            "kötü", "üzgün", "kızgın", "sinirli", "berbat", "fena", "üzücü",
            "acı", "hüzünlü", "mutsuz", "öfke", "hüzün", "kötü", "berbat"
        };
        
        var neutralWords = new[] {
            "nasıl", "nasılsın", "selam", "merhaba", "normal", "ok", "tamam",
            "anladım", "evet", "hayır", "belki", "muhtemelen", "iyi"
        };
        
        var lowerText = text.ToLower();
        
        // Kelime sayılarını hesapla
        var positiveCount = positiveWords.Count(w => lowerText.Contains(w));
        var negativeCount = negativeWords.Count(w => lowerText.Contains(w));
        var neutralCount = neutralWords.Count(w => lowerText.Contains(w));
        
        Console.WriteLine($"[DEBUG] Pozitif kelimeler: {positiveCount}, Negatif: {negativeCount}, Nötr: {neutralCount}");
        
        // Duygu analizi
        if (positiveCount > negativeCount && positiveCount > neutralCount)
        {
            var scores = new EmotionScores { Pozitif = 0.8, Negatif = 0.1, Nötr = 0.1 };
            Console.WriteLine($"[SUCCESS] ✅ Pozitif duygu tespit edildi: {text}");
            return scores;
        }
        else if (negativeCount > positiveCount && negativeCount > neutralCount)
        {
            var scores = new EmotionScores { Pozitif = 0.1, Negatif = 0.8, Nötr = 0.1 };
            Console.WriteLine($"[SUCCESS] ✅ Negatif duygu tespit edildi: {text}");
            return scores;
        }
        else if (neutralCount > 0 || (positiveCount == negativeCount))
        {
            var scores = new EmotionScores { Pozitif = 0.2, Negatif = 0.2, Nötr = 0.6 };
            Console.WriteLine($"[SUCCESS] ✅ Nötr duygu tespit edildi: {text}");
            return scores;
        }
        else
        {
            // Varsayılan: nötr
            var scores = new EmotionScores { Pozitif = 0.33, Negatif = 0.33, Nötr = 0.34 };
            Console.WriteLine($"[INFO] ℹ️ Varsayılan nötr duygu: {text}");
            return scores;
        }
    }
}

public class EmotionScores
{
    public double Pozitif { get; set; }
    public double Negatif { get; set; }
    public double Nötr { get; set; }
}

public class HuggingFaceResponse
{
    public object[][] Data { get; set; }
}

