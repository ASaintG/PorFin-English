using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class ListeningExercise
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Transcript { get; set; } = string.Empty;
    public CefrLevel CefrLevel { get; set; } = CefrLevel.A1;
    public string Category { get; set; } = string.Empty;
    public List<ComprehensionQuestion> Questions { get; set; } = [];
}
