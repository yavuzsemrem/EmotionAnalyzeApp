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
        // Geçici: AI servisi API desteklemiyor, geliştirilmiş kelime analizi yap
        Console.WriteLine($"[INFO] 🤖 Geçici AI servisi (kelime analizi): {text}");
        
        // Türkçe duygu kelimeleri - daha kapsamlı
        var positiveWords = new[] { 
            "iyi", "güzel", "harika", "mükemmel", "mutlu", "neşeli", "sevinçli",
            "hoş", "memnun", "başarılı", "başarı", "süper", "müthiş", "pozitif",
            "iyiyim", "iyiyiz", "iyiler", "güzeller", "harikalar", "mükemmeller",
            "mutluyum", "mutluyuz", "mutlular", "neşeliyim", "neşeliyiz", "neşeliler",
            "sevindim", "sevindik", "sevindiler", "hoşum", "hoşuz", "hoşlar",
            "memnunum", "memnunuz", "memnunlar", "başarılıyım", "başarılıyız", "başarılılar"
        };
        
        var negativeWords = new[] { 
            "kötü", "üzgün", "kızgın", "sinirli", "berbat", "fena", "üzücü",
            "acı", "hüzünlü", "mutsuz", "öfke", "hüzün", "kötüyüm", "kötüyüz", "kötüler",
            "üzgünüm", "üzgünüz", "üzgünler", "kızgınım", "kızgınız", "kızgınlar",
            "sinirliyim", "sinirliyiz", "sinirliler", "berbatım", "berbatız", "berbatlar",
            "fenayım", "fenayız", "fenalar", "üzücüyüm", "üzücüyüz", "üzücüler",
            "acıyım", "acıyız", "acılar", "hüzünlüyüm", "hüzünlüyüz", "hüzünlüler",
            "mutsuzum", "mutsuzuz", "mutsuzlar", "öfkeli", "öfkeliyim", "öfkeliyiz", "öfkeliler"
        };
        
        var neutralWords = new[] {
            "nasıl", "nasılsın", "selam", "merhaba", "normal", "ok", "tamam",
            "anladım", "evet", "hayır", "belki", "muhtemelen", "nasılsınız",
            "selamlar", "merhabalar", "normaller", "okay", "tamamlar", "anladık",
            "evetler", "hayırlar", "belkiler", "muhtemelenler"
        };
        
        var lowerText = text.ToLower().Trim();
        
        // Kelime sayılarını hesapla
        var positiveCount = positiveWords.Count(w => lowerText.Contains(w));
        var negativeCount = negativeWords.Count(w => lowerText.Contains(w));
        var neutralCount = neutralWords.Count(w => lowerText.Contains(w));
        
        Console.WriteLine($"[DEBUG] Pozitif kelimeler: {positiveCount}, Negatif: {negativeCount}, Nötr: {neutralCount}");
        
        // Toplam kelime sayısı
        var totalWords = positiveCount + negativeCount + neutralCount;
        
        // Duygu analizi - daha akıllı algoritma
        if (totalWords == 0)
        {
            // Hiç duygu kelimesi yoksa nötr
            var scores = new EmotionScores { Pozitif = 0.33, Negatif = 0.33, Nötr = 0.34 };
            Console.WriteLine($"[INFO] ℹ️ Duygu kelimesi bulunamadı, varsayılan nötr: {text}");
            return scores;
        }
        
        // En yüksek skorlu duygu türünü bul
        var maxCount = Math.Max(positiveCount, Math.Max(negativeCount, neutralCount));
        
        if (positiveCount == maxCount && positiveCount > 0)
        {
            // Pozitif duygu - dinamik skorlama
            var positiveRatio = (double)positiveCount / totalWords;
            var positiveScore = Math.Min(0.9, 0.5 + (positiveRatio * 0.4)); // 0.5-0.9 arası
            var remainingScore = 1.0 - positiveScore;
            var negativeScore = remainingScore * 0.2; // %20 negatif
            var neutralScore = remainingScore * 0.8; // %80 nötr
            
            var scores = new EmotionScores { 
                Pozitif = positiveScore, 
                Negatif = negativeScore, 
                Nötr = neutralScore 
            };
            Console.WriteLine($"[SUCCESS] ✅ Pozitif duygu tespit edildi: {text} (Pozitif: {positiveScore:P0})");
            return scores;
        }
        else if (negativeCount == maxCount && negativeCount > 0)
        {
            // Negatif duygu - dinamik skorlama
            var negativeRatio = (double)negativeCount / totalWords;
            var negativeScore = Math.Min(0.9, 0.5 + (negativeRatio * 0.4)); // 0.5-0.9 arası
            var remainingScore = 1.0 - negativeScore;
            var positiveScore = remainingScore * 0.2; // %20 pozitif
            var neutralScore = remainingScore * 0.8; // %80 nötr
            
            var scores = new EmotionScores { 
                Pozitif = positiveScore, 
                Negatif = negativeScore, 
                Nötr = neutralScore 
            };
            Console.WriteLine($"[SUCCESS] ✅ Negatif duygu tespit edildi: {text} (Negatif: {negativeScore:P0})");
            return scores;
        }
        else
        {
            // Nötr duygu - dinamik skorlama
            var neutralRatio = (double)neutralCount / totalWords;
            var neutralScore = Math.Min(0.8, 0.4 + (neutralRatio * 0.4)); // 0.4-0.8 arası
            var remainingScore = 1.0 - neutralScore;
            var positiveScore = remainingScore * 0.5; // %50 pozitif
            var negativeScore = remainingScore * 0.5; // %50 negatif
            
            var scores = new EmotionScores { 
                Pozitif = positiveScore, 
                Negatif = negativeScore, 
                Nötr = neutralScore 
            };
            Console.WriteLine($"[SUCCESS] ✅ Nötr duygu tespit edildi: {text} (Nötr: {neutralScore:P0})");
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

