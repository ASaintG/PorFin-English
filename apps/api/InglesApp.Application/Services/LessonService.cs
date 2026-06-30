using InglesApp.Application.DTOs.Exercises;
using InglesApp.Application.DTOs.Lessons;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Application.Services;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IGrammarRepository _grammarRepository;
    private readonly IUserRepository _userRepository;

    public LessonService(
        ILessonRepository lessonRepository,
        IExerciseRepository exerciseRepository,
        IGrammarRepository grammarRepository,
        IUserRepository userRepository)
    {
        _lessonRepository = lessonRepository;
        _exerciseRepository = exerciseRepository;
        _grammarRepository = grammarRepository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<LessonSummaryDto>> GetByLevelAsync(
        string userId, CefrLevel level, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new KeyNotFoundException("User not found.");
        var lessons = await _lessonRepository.GetByLevelAsync(level, ct);
        var allLessons = await _lessonRepository.GetAllSortedAsync(ct);
        var completed = user.CompletedLessonIds.ToHashSet();

        return lessons.Select(l =>
        {
            var isLocked = IsLessonLocked(l, allLessons, completed);
            return new LessonSummaryDto(
                l.Id, l.Title, l.Description, l.CefrLevel.ToString(),
                l.Category, l.Theme, l.Sequence, l.ExerciseIds.Count, isLocked);
        }).ToList();
    }

    public async Task<LessonDetailDto?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var lesson = await _lessonRepository.GetByIdAsync(id, ct);
        if (lesson is null) return null;

        var exercises = await _exerciseRepository.GetByIdsAsync(lesson.ExerciseIds, ct);
        var grammarRules = new List<DTOs.Grammar.GrammarRuleDto>();
        foreach (var ruleId in lesson.GrammarRuleIds)
        {
            var r = await _grammarRepository.GetByIdAsync(ruleId, ct);
            if (r is not null)
                grammarRules.Add(new DTOs.Grammar.GrammarRuleDto(
                    r.Id, r.Title, r.Explanation, r.CefrLevel.ToString(),
                    r.Category, r.Examples, r.RelatedStructures));
        }

        return new LessonDetailDto(
            lesson.Id, lesson.Title, lesson.Description, lesson.CefrLevel.ToString(),
            lesson.Category, lesson.Theme, lesson.Sequence,
            grammarRules,
            exercises.Select(e => new ExerciseDto(
                e.Id, e.Type.ToString(), ExerciseGeneratorService.ParseContent(e.ContentJson),
                e.CefrLevel.ToString(), e.Skill.ToString(), e.GrammarRuleId, e.Category, e.Difficulty)).ToList());
    }

    public async Task CompleteLessonAsync(string userId, string lessonId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct)
            ?? throw new KeyNotFoundException("User not found.");
        var lesson = await _lessonRepository.GetByIdAsync(lessonId, ct)
            ?? throw new KeyNotFoundException("Lesson not found.");

        if (!user.CompletedLessonIds.Contains(lessonId))
        {
            user.CompletedLessonIds.Add(lessonId);
            await _userRepository.UpdateAsync(user, ct);
        }
    }

    public static bool IsLessonLocked(
        Domain.Entities.Lesson lesson,
        IReadOnlyList<Domain.Entities.Lesson> allLessons,
        HashSet<string> completed)
    {
        if (lesson.Sequence <= 1) return false;
        var previous = allLessons.FirstOrDefault(l => l.Sequence == lesson.Sequence - 1);
        if (previous is null) return false;
        return !completed.Contains(previous.Id);
    }
}
