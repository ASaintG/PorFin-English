namespace InglesApp.Application.DTOs.Exam;

public record ExamSectionDto(string Section, string Skill, IReadOnlyList<object> Items);

public record ExamStartDto(string ExamId, IReadOnlyList<ExamSectionDto> Sections, int DurationMinutes);

public record SubmitExamRequest(IReadOnlyList<ExamAnswerDto> Answers);

public record ExamAnswerDto(string ItemId, string Skill, bool Correct);

public record ExamResultDto(int TotalCorrect, int TotalQuestions, double ScorePercent, IReadOnlyList<ExamSectionScoreDto> SectionScores);

public record ExamSectionScoreDto(string Section, int Correct, int Total);
