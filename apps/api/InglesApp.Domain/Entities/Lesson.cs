using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class Lesson
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CefrLevel CefrLevel { get; set; } = CefrLevel.A1;
    public string Category { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
    public int Sequence { get; set; }
    public List<string> ExerciseIds { get; set; } = [];
    public List<string> GrammarRuleIds { get; set; } = [];
    public bool IsLocked { get; set; }
}
