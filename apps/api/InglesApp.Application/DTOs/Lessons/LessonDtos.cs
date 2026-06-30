namespace InglesApp.Application.DTOs.Lessons;

public record LessonSummaryDto(
    string Id,
    string Title,
    string Description,
    string CefrLevel,
    string Category,
    string Theme,
    int Sequence,
    int ExerciseCount,
    bool IsLocked);

public record LessonDetailDto(
    string Id,
    string Title,
    string Description,
    string CefrLevel,
    string Category,
    string Theme,
    int Sequence,
    IReadOnlyList<Grammar.GrammarRuleDto> GrammarRules,
    IReadOnlyList<Exercises.ExerciseDto> Exercises);
