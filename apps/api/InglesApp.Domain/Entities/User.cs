using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public CefrLevel CurrentLevel { get; set; } = CefrLevel.A1;
    public string CurrentSubLevel { get; set; } = "A1.1";
    public int Streak { get; set; }
    public int TotalPracticeMinutes { get; set; }
    public bool PlacementCompleted { get; set; }
    public List<string> CompletedLessonIds { get; set; } = [];
    public DateTime? LastActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
