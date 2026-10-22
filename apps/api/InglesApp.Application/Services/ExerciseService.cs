using System.Text.Json;
using InglesApp.Application.DTOs.Exercises;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using InglesApp.Domain.Services;

namespace InglesApp.Application.Services;

public class ExerciseService : IExerciseService
{
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly SpacedRepetitionService _srs;

    public ExerciseService(
        IExerciseRepository exerciseRepository,
        IUserProgressRepository progressRepository,
        SpacedRepetitionService srs)
    {
        _exerciseRepository = exerciseRepository;
        _progressRepository = progressRepository;
        _srs = srs;
    }

    public Task<IReadOnlyList<ExerciseDueDto>> GetDueAsync(string userId, CancellationToken ct = default) =>
        GetDueAsync(userId, null, null, ct);

    public async Task<IReadOnlyList<ExerciseDueDto>> GetDueAsync(
        string userId,
        CefrLevel? level,
        string? category,
        CancellationToken ct = default)
    {
        var due = await _progressRepository.GetDueByItemTypeAsync(userId, ItemType.Grammar, DateTime.UtcNow, ct);
        var results = new List<ExerciseDueDto>();

        foreach (var p in due)
        {
            var ex = await _exerciseRepository.GetByIdAsync(p.ItemId, ct);
            if (ex is null) continue;
            if (level.HasValue && ex.CefrLevel != level.Value) continue;
            if (!MatchesCategory(ex.Category, category)) continue;
            results.Add(MapDue(ex, p));
        }

        if (results.Count == 0)
        {
            var available = await _exerciseRepository.GetByLevelAndSkillAsync(
                level ?? CefrLevel.A1, Skill.Grammar, ct);

            foreach (var ex in available.Where(e => MatchesCategory(e.Category, category)).Take(10))
            {
                var existing = await _progressRepository.GetByUserAndItemAsync(userId, ex.Id, ct);
                if (existing is not null) continue;
                var initial = _srs.CreateInitialProgress(userId, ex.Id, ItemType.Grammar);
                await _progressRepository.UpsertAsync(initial, ct);
                results.Add(MapDue(ex, initial));
            }
        }

        return results;
    }

    private static bool MatchesCategory(string itemCategory, string? category) =>
        string.IsNullOrWhiteSpace(category)
        || itemCategory.Equals(category, StringComparison.OrdinalIgnoreCase);

    public async Task<SubmitExerciseResponse> SubmitAsync(
        string userId, string exerciseId, SubmitExerciseRequest request, CancellationToken ct = default)
    {
        var quality = request.Quality;
        if (request.Correct.HasValue)
            quality = request.Correct.Value ? 5 : 2;

        var progress = await _progressRepository.GetByUserAndItemAsync(userId, exerciseId, ct)
            ?? _srs.CreateInitialProgress(userId, exerciseId, ItemType.Grammar);

        var updated = _srs.ApplyReview(progress, quality);
        var saved = await _progressRepository.UpsertAsync(updated, ct);

        return new SubmitExerciseResponse(
            saved.ItemId, saved.Interval, saved.Repetitions, saved.NextReviewDate,
            request.Correct ?? quality >= 3);
    }

    public async Task<IReadOnlyList<ExerciseDto>> GetPlacementExercisesAsync(CefrLevel level, CancellationToken ct = default)
    {
        var exercises = await _exerciseRepository.GetByLevelAndSkillAsync(level, Skill.Grammar, ct);
        return exercises.Take(5).Select(e => new ExerciseDto(
            e.Id, e.Type.ToString(), ExerciseGeneratorService.ParseContent(e.ContentJson),
            e.CefrLevel.ToString(), e.Skill.ToString(), e.GrammarRuleId, e.Category, e.Difficulty)).ToList();
    }

    private static ExerciseDueDto MapDue(Domain.Entities.Exercise ex, Domain.Entities.UserProgress p) =>
        new(ex.Id, ex.Type.ToString(), ExerciseGeneratorService.ParseContent(ex.ContentJson),
            ex.CefrLevel.ToString(), ex.Skill.ToString(), ex.Category, p.Repetitions, p.NextReviewDate);
}
