using InglesApp.Application.DTOs.Listening;
using InglesApp.Application.DTOs.Reading;
using InglesApp.Domain.Enums;

namespace InglesApp.Application.Interfaces;

public interface IReadingService
{
    Task<IReadOnlyList<ReadingSummaryDto>> GetByLevelAsync(CefrLevel level, int skip, int take, CancellationToken cancellationToken = default);
    Task<ReadingDetailDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<SubmitComprehensionResponse> SubmitAsync(string userId, string id, SubmitComprehensionRequest request, CancellationToken cancellationToken = default);
}

public interface IListeningService
{
    Task<IReadOnlyList<ListeningSummaryDto>> GetByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default);
    Task<ListeningDetailDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<SubmitComprehensionResponse> SubmitAsync(string userId, string id, SubmitListeningRequest request, CancellationToken cancellationToken = default);
}

public interface ISpeakingService
{
    Task<IReadOnlyList<DTOs.Speaking.SpeakingPromptDto>> GetPromptsAsync(CefrLevel level, string? category, CancellationToken cancellationToken = default);
    Task<DTOs.Speaking.SubmitSpeakingResponse> SubmitAsync(string userId, DTOs.Speaking.SubmitSpeakingRequest request, CancellationToken cancellationToken = default);
}

public interface IWritingService
{
    Task<IReadOnlyList<DTOs.Writing.WritingPromptDto>> GetPromptsAsync(CefrLevel level, string? category, CancellationToken cancellationToken = default);
    Task<DTOs.Writing.SubmitWritingResponse> SubmitAsync(string userId, DTOs.Writing.SubmitWritingRequest request, CancellationToken cancellationToken = default);
}

public interface IPlacementService
{
    Task<IReadOnlyList<DTOs.Placement.PlacementQuestionDto>> GetQuestionsAsync(CancellationToken cancellationToken = default);
    Task<DTOs.Placement.PlacementResultDto> CompleteAsync(string userId, int correct, int total, string finalLevel, CancellationToken cancellationToken = default);
}

public interface IDailySessionService
{
    Task<DTOs.Daily.DailySessionDto> GetDailySessionAsync(string userId, CancellationToken cancellationToken = default);
    Task<DTOs.Daily.DailySessionDto> GetDailySessionAsync(string userId, CefrLevel? level, string? category, CancellationToken cancellationToken = default);
}

public interface IExamSimulatorService
{
    Task<DTOs.Exam.ExamStartDto> StartExamAsync(string userId, CancellationToken cancellationToken = default);
    Task<DTOs.Exam.ExamResultDto> SubmitExamAsync(string userId, DTOs.Exam.SubmitExamRequest request, CancellationToken cancellationToken = default);
}

public interface ISuggestionService
{
    Task CreateAsync(string userId, DTOs.Suggestions.CreateSuggestionRequest request, CancellationToken cancellationToken = default);
}
