using InglesApp.Application.DTOs.Progress;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Application.Services;

public class ProgressService : IProgressService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly IVocabularyRepository _vocabularyRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly IReadingRepository _readingRepository;
    private readonly IListeningRepository _listeningRepository;

    public ProgressService(
        IUserRepository userRepository,
        IUserProgressRepository progressRepository,
        IVocabularyRepository vocabularyRepository,
        IExerciseRepository exerciseRepository,
        IReadingRepository readingRepository,
        IListeningRepository listeningRepository)
    {
        _userRepository = userRepository;
        _progressRepository = progressRepository;
        _vocabularyRepository = vocabularyRepository;
        _exerciseRepository = exerciseRepository;
        _readingRepository = readingRepository;
        _listeningRepository = listeningRepository;
    }

    public async Task<ProgressStatsDto> GetStatsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        var wordsMastered = await _progressRepository.CountMasteredByItemTypeAsync(userId, ItemType.Vocabulary, cancellationToken: cancellationToken);
        var grammarMastered = await _progressRepository.CountMasteredByItemTypeAsync(userId, ItemType.Grammar, cancellationToken: cancellationToken);
        var readingCompleted = await _progressRepository.CountMasteredByItemTypeAsync(userId, ItemType.Reading, cancellationToken: cancellationToken);
        var listeningCompleted = await _progressRepository.CountMasteredByItemTypeAsync(userId, ItemType.Listening, cancellationToken: cancellationToken);
        var speakingCompleted = await _progressRepository.CountMasteredByItemTypeAsync(userId, ItemType.Speaking, cancellationToken: cancellationToken);
        var writingCompleted = await _progressRepository.CountMasteredByItemTypeAsync(userId, ItemType.Writing, cancellationToken: cancellationToken);

        var dueToday = await _progressRepository.CountDueAsync(userId, DateTime.UtcNow, cancellationToken);
        var allProgress = await _progressRepository.GetByUserAsync(userId, cancellationToken);
        var totalA1 = await _vocabularyRepository.CountByLevelAsync(CefrLevel.A1, cancellationToken);
        var totalGrammar = (await _exerciseRepository.GetByLevelAndSkillAsync(CefrLevel.A1, Skill.Grammar, cancellationToken)).Count;
        var totalReading = (await _readingRepository.GetByLevelAsync(CefrLevel.A1, 0, 100, cancellationToken)).Count;
        var totalListening = (await _listeningRepository.GetByLevelAsync(CefrLevel.A1, cancellationToken)).Count;

        var masteryPercent = totalA1 > 0 ? Math.Round(wordsMastered * 100.0 / totalA1, 1) : 0;

        var skills = new List<SkillProgressDto>
        {
            new("Vocabulary", wordsMastered, totalA1, Pct(wordsMastered, totalA1)),
            new("Grammar", grammarMastered, totalGrammar, Pct(grammarMastered, totalGrammar)),
            new("Reading", readingCompleted, totalReading, Pct(readingCompleted, totalReading)),
            new("Listening", listeningCompleted, totalListening, Pct(listeningCompleted, totalListening)),
            new("Speaking", speakingCompleted, 10, Pct(speakingCompleted, 10)),
            new("Writing", writingCompleted, 10, Pct(writingCompleted, 10)),
        };

        return new ProgressStatsDto(
            wordsMastered,
            dueToday,
            allProgress.Count,
            user.Streak,
            user.CurrentLevel.ToString(),
            user.CurrentSubLevel,
            masteryPercent,
            user.TotalPracticeMinutes,
            grammarMastered,
            readingCompleted,
            listeningCompleted,
            speakingCompleted,
            writingCompleted,
            skills);
    }

    public async Task UpdateStreakAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        var today = DateTime.UtcNow.Date;
        var lastActive = user.LastActive?.Date;

        if (lastActive == today) return;

        user.Streak = lastActive == today.AddDays(-1) ? user.Streak + 1 : 1;
        user.LastActive = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task AddPracticeMinutesAsync(string userId, int minutes, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");
        user.TotalPracticeMinutes += minutes;
        user.LastActive = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, cancellationToken);
    }

    private static double Pct(long part, long total) => total > 0 ? Math.Round(part * 100.0 / total, 1) : 0;
}
