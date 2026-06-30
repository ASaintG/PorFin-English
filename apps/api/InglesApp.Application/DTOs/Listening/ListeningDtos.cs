namespace InglesApp.Application.DTOs.Listening;

public record ListeningSummaryDto(string Id, string Title, string CefrLevel, string Category);

public record ListeningDetailDto(
    string Id,
    string Title,
    string Transcript,
    string CefrLevel,
    string Category,
    IReadOnlyList<Reading.ComprehensionQuestionDto> Questions);

public record SubmitListeningRequest(IReadOnlyList<int> Answers);
