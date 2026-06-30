using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class SpeakingPrompt
{
    public string Id { get; set; } = string.Empty;
    public string Prompt { get; set; } = string.Empty;
    public CefrLevel CefrLevel { get; set; } = CefrLevel.A1;
    public string Category { get; set; } = string.Empty;
    public string ModelAnswer { get; set; } = string.Empty;
    public List<string> KeyPhrases { get; set; } = [];
}
