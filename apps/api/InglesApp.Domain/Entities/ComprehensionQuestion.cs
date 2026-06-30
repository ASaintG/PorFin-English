namespace InglesApp.Domain.Entities;

public class ComprehensionQuestion
{
    public string Id { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public int CorrectIndex { get; set; }
}
