namespace InglesApp.Application.DTOs.Progress;

public record SkillProgressDto(string Skill, long Mastered, long Total, double Percent);

public record ProgressStatsDto(
    long WordsMastered,
    long WordsDueToday,
    long TotalWordsStudied,
    int Streak,
    string CurrentLevel,
    string CurrentSubLevel,
    double A1MasteryPercent,
    int TotalPracticeMinutes,
    long GrammarMastered,
    long ReadingCompleted,
    long ListeningCompleted,
    long SpeakingCompleted,
    long WritingCompleted,
    IReadOnlyList<SkillProgressDto> Skills);
