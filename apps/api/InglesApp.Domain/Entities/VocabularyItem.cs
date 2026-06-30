using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class VocabularyItem
{
    public string Id { get; set; } = string.Empty;
    public string Word { get; set; } = string.Empty;
    public string Translation { get; set; } = string.Empty;
    public string PartOfSpeech { get; set; } = string.Empty;
    public CefrLevel CefrLevel { get; set; } = CefrLevel.A1;
    public string Category { get; set; } = string.Empty;
    public List<string> ExampleSentences { get; set; } = [];
}
