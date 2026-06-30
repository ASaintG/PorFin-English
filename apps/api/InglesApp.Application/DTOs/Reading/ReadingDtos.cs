namespace InglesApp.Application.DTOs.Reading;

public record ReadingSummaryDto(string Id, string Title, string CefrLevel, int WordCount, string Attribution);

public record ReadingDetailDto(
    string Id,
    string Title,
    string Body,
    string CefrLevel,
    int WordCount,
    string Source,
    string Attribution,
    IReadOnlyList<ComprehensionQuestionDto> Questions);

public record ComprehensionQuestionDto(string Id, string Question, IReadOnlyList<string> Options);

public record SubmitComprehensionRequest(IReadOnlyList<int> Answers);

public record SubmitComprehensionResponse(int Correct, int Total, double ScorePercent);
