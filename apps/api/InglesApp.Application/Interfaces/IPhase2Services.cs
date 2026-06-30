using InglesApp.Application.DTOs.Grammar;
using InglesApp.Application.DTOs.Exercises;
using InglesApp.Application.DTOs.Lessons;
using InglesApp.Domain.Enums;

namespace InglesApp.Application.Interfaces;

public interface IGrammarService
{
    Task<IReadOnlyList<GrammarRuleDto>> GetByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default);
    Task<GrammarRuleDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}

public interface ILessonService
{
    Task<IReadOnlyList<LessonSummaryDto>> GetByLevelAsync(string userId, CefrLevel level, CancellationToken cancellationToken = default);
    Task<LessonDetailDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task CompleteLessonAsync(string userId, string lessonId, CancellationToken cancellationToken = default);
}

public interface IExerciseService
{
    Task<IReadOnlyList<ExerciseDueDto>> GetDueAsync(string userId, CancellationToken cancellationToken = default);
    Task<SubmitExerciseResponse> SubmitAsync(string userId, string exerciseId, SubmitExerciseRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExerciseDto>> GetPlacementExercisesAsync(CefrLevel level, CancellationToken cancellationToken = default);
}
