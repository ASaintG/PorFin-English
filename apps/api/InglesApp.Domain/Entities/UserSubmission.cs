namespace InglesApp.Domain.Entities;

public class UserSubmission
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string PromptId { get; set; } = string.Empty;
    public string SubmissionType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int? DurationSeconds { get; set; }
    public List<string> MatchedKeyPhrases { get; set; } = [];
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
