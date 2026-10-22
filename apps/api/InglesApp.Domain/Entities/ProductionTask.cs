namespace InglesApp.Domain.Entities;

public class ProductionTask
{
    public string Type { get; set; } = "writing";
    public string Prompt { get; set; } = string.Empty;
    public string? TtsText { get; set; }
    public string? AudioUrl { get; set; }
    public List<string> Questions { get; set; } = [];
}
