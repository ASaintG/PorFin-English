namespace InglesApp.Application.DTOs.Speaking;

public record SpeakingPromptDto(
    string Id,
    string Prompt,
    string CefrLevel,
    string Category,
    string ModelAnswer,
    IReadOnlyList<string> KeyPhrases);

public record SubmitSpeakingRequest(string PromptId, string Transcript, int DurationSeconds);

public record SubmitSpeakingResponse(
    IReadOnlyList<string> MatchedKeyPhrases,
    int MatchedCount,
    int TotalKeyPhrases,
    string ModelAnswer);
