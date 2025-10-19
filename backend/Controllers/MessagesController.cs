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
        var client = _clientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(60); // Model yavaş olabilir
        
        // Environment variable'dan AI servisi URL'ini al
        var apiUrl = _configuration["HuggingFaceUrl"] ?? "http://127.0.0.1:7861/analyze";
        
        try
        {
            // Hugging Face Space API formatı: { "data": ["mesaj"] }
            var requestBody = new { data = new[] { text } };
            
            Console.WriteLine($"[DEBUG] AI Servisine istek atılıyor: {apiUrl}");
            Console.WriteLine($"[DEBUG] Mesaj: {text}");
            
            var response = await client.PostAsJsonAsync(apiUrl, requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"[DEBUG] Response Status: {response.StatusCode}");
            Console.WriteLine($"[DEBUG] Response: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}...");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"AI servisi hata döndü: {response.StatusCode}, İçerik: {responseContent}");
            }

            // Hugging Face Space API response formatı
            // { "data": [["pozitif", 0.8, 0.1, 0.1]] }
            var apiResponse = await response.Content.ReadFromJsonAsync<HuggingFaceResponse>();
            
            if (apiResponse?.Data == null || apiResponse.Data.Length == 0)
            {
                throw new Exception("Duygu analizi sonucu parse edilemedi");
            }
            
            var result = apiResponse.Data[0];
            if (result.Length < 4)
            {
                throw new Exception("Duygu analizi sonucu eksik");
            }
            
            var emotion = result[0].ToString();
            var positive = Convert.ToDouble(result[1]);
            var negative = Convert.ToDouble(result[2]);
            var neutral = Convert.ToDouble(result[3]);
            
            Console.WriteLine($"[SUCCESS] ✅ Duygu: {emotion}, Pozitif: {positive:P0}, Negatif: {negative:P0}, Nötr: {neutral:P0}");
            
            return new EmotionScores
            {
                Pozitif = positive,
                Negatif = negative,
                Nötr = neutral
            };
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"[ERROR] ❌ AI servisine bağlanılamadı: {ex.Message}");
            Console.WriteLine($"[ERROR] AI servisinin çalıştığından emin olun: {apiUrl}");
            
            // Hata durumunda neutral döndür
            return new EmotionScores
            {
                Pozitif = 0.33,
                Negatif = 0.33,
                Nötr = 0.34
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] ❌ Duygu analizi hatası: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack: {ex.StackTrace}");
            
            // Hata durumunda neutral döndür
            return new EmotionScores
            {
                Pozitif = 0.33,
                Negatif = 0.33,
                Nötr = 0.34
            };
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

