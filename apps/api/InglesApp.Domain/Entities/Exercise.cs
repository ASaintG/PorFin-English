using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class Exercise
{
    public string Id { get; set; } = string.Empty;
    public ExerciseType Type { get; set; }
    public string ContentJson { get; set; } = "{}";
    public CefrLevel CefrLevel { get; set; } = CefrLevel.A1;
    public Skill Skill { get; set; } = Skill.Grammar;
    public string? GrammarRuleId { get; set; }
    public string? LessonId { get; set; }
    public int Difficulty { get; set; } = 1;
    public string Category { get; set; } = string.Empty;
}
