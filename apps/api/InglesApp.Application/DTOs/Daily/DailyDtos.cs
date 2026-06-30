namespace InglesApp.Application.DTOs.Daily;

public record DailySessionDto(
    IReadOnlyList<Reviews.ReviewItemDto> VocabularyReviews,
    IReadOnlyList<Exercises.ExerciseDueDto> GrammarReviews,
    string? RecommendedLessonId,
    string? RecommendedLessonTitle,
    int EstimatedMinutes);
