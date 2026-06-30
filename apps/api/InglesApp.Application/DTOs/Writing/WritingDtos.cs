namespace InglesApp.Application.DTOs.Writing;

public record WritingPromptDto(
    string Id,
    string Prompt,
    string CefrLevel,
    string Category,
    int WordLimit);

public record SubmitWritingRequest(string PromptId, string Text);

public record SubmitWritingResponse(string ModelAnswer, int WordCount, int WordLimit);
