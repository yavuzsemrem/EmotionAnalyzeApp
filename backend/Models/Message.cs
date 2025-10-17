namespace EmotionAnalyzeApi.Models;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Emotion analysis results
    public double PositiveScore { get; set; }
    public double NegativeScore { get; set; }
    public double NeutralScore { get; set; }
    
    // Foreign key
    public int UserId { get; set; }
    public User? User { get; set; }
}
