using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class ReadingText
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public CefrLevel CefrLevel { get; set; } = CefrLevel.A1;
    public int WordCount { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Attribution { get; set; } = string.Empty;
    public List<ComprehensionQuestion> Questions { get; set; } = [];
}
