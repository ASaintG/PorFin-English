using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class UserProgress
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public ItemType ItemType { get; set; } = ItemType.Vocabulary;
    public double EaseFactor { get; set; } = 2.5;
    public int Interval { get; set; }
    public int Repetitions { get; set; }
    public DateTime NextReviewDate { get; set; } = DateTime.UtcNow;
    public int CorrectCount { get; set; }
    public DateTime? LastReviewed { get; set; }
}
