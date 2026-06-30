namespace InglesApp.Domain.Entities;

public class ContentSuggestion
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? RelatedItemId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
