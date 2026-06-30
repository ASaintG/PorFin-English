namespace InglesApp.Application.DTOs.Exercises;

public record ExerciseDto(
    string Id,
    string Type,
    object Content,
    string CefrLevel,
    string Skill,
    string? GrammarRuleId,
    string Category,
    int Difficulty);

public record ExerciseDueDto(
    string Id,
    string Type,
    object Content,
    string CefrLevel,
    string Skill,
    int Repetitions,
    DateTime NextReviewDate);

public record SubmitExerciseRequest(int Quality, bool? Correct);

public record SubmitExerciseResponse(
    string ItemId,
    int Interval,
    int Repetitions,
    DateTime NextReviewDate,
    bool WasCorrect);
