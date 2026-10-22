using InglesApp.Application.DTOs.Daily;
using InglesApp.Application.DTOs.Exam;
using InglesApp.Application.DTOs.Placement;
using InglesApp.Application.DTOs.Suggestions;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Application.Services;

public class PlacementService : IPlacementService
{
    private readonly IExerciseService _exerciseService;
    private readonly IUserRepository _userRepository;

    public PlacementService(IExerciseService exerciseService, IUserRepository userRepository)
    {
        _exerciseService = exerciseService;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<PlacementQuestionDto>> GetQuestionsAsync(CancellationToken ct = default)
    {
        var levels = new[] { CefrLevel.A1, CefrLevel.A2, CefrLevel.B1 };
        var questions = new List<PlacementQuestionDto>();
        foreach (var level in levels)
        {
            var exercises = await _exerciseService.GetPlacementExercisesAsync(level, ct);
            questions.AddRange(exercises.Take(2).Select(e => new PlacementQuestionDto(
                e.Id, e.Type, e.Content, e.CefrLevel)));
        }
        return questions;
    }

    public async Task<PlacementResultDto> CompleteAsync(string userId, int correct, int total, string finalLevel, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct) ?? throw new KeyNotFoundException();
        if (Enum.TryParse<CefrLevel>(finalLevel, true, out var level))
            user.CurrentLevel = level;
        user.CurrentSubLevel = $"{user.CurrentLevel}.1";
        user.PlacementCompleted = true;
        await _userRepository.UpdateAsync(user, ct);
        return new PlacementResultDto(user.CurrentLevel.ToString(), user.CurrentSubLevel, correct, total);
    }
}

public class DailySessionService : IDailySessionService
{
    private readonly IReviewService _reviewService;
    private readonly IExerciseService _exerciseService;
    private readonly ILessonRepository _lessonRepository;
    private readonly IUserRepository _userRepository;

    public DailySessionService(
        IReviewService reviewService,
        IExerciseService exerciseService,
        ILessonRepository lessonRepository,
        IUserRepository userRepository)
    {
        _reviewService = reviewService;
        _exerciseService = exerciseService;
        _lessonRepository = lessonRepository;
        _userRepository = userRepository;
    }

    public Task<DailySessionDto> GetDailySessionAsync(string userId, CancellationToken ct = default) =>
        GetDailySessionAsync(userId, null, null, ct);

    public async Task<DailySessionDto> GetDailySessionAsync(
        string userId,
        CefrLevel? level,
        string? category,
        CancellationToken ct = default)
    {
        var vocab = await _reviewService.GetDueItemsAsync(userId, level, category, ct);
        var grammar = await _exerciseService.GetDueAsync(userId, level, category, ct);
        var user = await _userRepository.GetByIdAsync(userId, ct);
        var targetLevel = level ?? user?.CurrentLevel ?? CefrLevel.A1;
        var lessons = await _lessonRepository.GetByLevelAsync(targetLevel, ct);
        var completed = user?.CompletedLessonIds.ToHashSet() ?? [];
        var nextLesson = lessons.FirstOrDefault(lesson => !completed.Contains(lesson.Id))
            ?? lessons.FirstOrDefault();

        return new DailySessionDto(
            vocab.Take(10).ToList(),
            grammar.Take(5).ToList(),
            nextLesson?.Id,
            nextLesson?.Title,
            15);
    }
}

public class ExamSimulatorService : IExamSimulatorService
{
    private readonly IExerciseService _exerciseService;
    private readonly IReviewService _reviewService;
    private readonly IReadingRepository _readingRepository;
    private readonly IListeningRepository _listeningRepository;

    public ExamSimulatorService(
        IExerciseService exerciseService,
        IReviewService reviewService,
        IReadingRepository readingRepository,
        IListeningRepository listeningRepository)
    {
        _exerciseService = exerciseService;
        _reviewService = reviewService;
        _readingRepository = readingRepository;
        _listeningRepository = listeningRepository;
    }

    public async Task<ExamStartDto> StartExamAsync(string userId, CancellationToken ct = default)
    {
        var vocab = await _reviewService.GetDueItemsAsync(userId, ct);
        var grammar = await _exerciseService.GetDueAsync(userId, ct);
        var reading = await _readingRepository.GetByLevelAsync(CefrLevel.A1, 0, 1, ct);
        var listening = await _listeningRepository.GetByLevelAsync(CefrLevel.A1, ct);

        var sections = new List<ExamSectionDto>
        {
            new("Vocabulary", "Vocabulary", vocab.Take(5).Cast<object>().ToList()),
            new("Grammar", "Grammar", grammar.Take(5).Cast<object>().ToList()),
            new("Reading", "Reading", reading.Cast<object>().ToList()),
            new("Listening", "Listening", listening.Take(1).Cast<object>().ToList())
        };

        return new ExamStartDto(Guid.NewGuid().ToString(), sections, 30);
    }

    public Task<ExamResultDto> SubmitExamAsync(string userId, SubmitExamRequest request, CancellationToken ct = default)
    {
        var correct = request.Answers.Count(a => a.Correct);
        var total = request.Answers.Count;
        var bySection = request.Answers
            .GroupBy(a => a.Skill)
            .Select(g => new ExamSectionScoreDto(g.Key, g.Count(a => a.Correct), g.Count()))
            .ToList();

        return Task.FromResult(new ExamResultDto(
            correct, total, total > 0 ? Math.Round(correct * 100.0 / total, 1) : 0, bySection));
    }
}

public class SuggestionService : ISuggestionService
{
    private readonly IContentSuggestionRepository _suggestionRepository;

    public SuggestionService(IContentSuggestionRepository suggestionRepository) =>
        _suggestionRepository = suggestionRepository;

    public async Task CreateAsync(string userId, CreateSuggestionRequest request, CancellationToken ct = default)
    {
        await _suggestionRepository.CreateAsync(new Domain.Entities.ContentSuggestion
        {
            UserId = userId,
            Type = request.Type,
            Content = request.Content,
            RelatedItemId = request.RelatedItemId
        }, ct);
    }
}
