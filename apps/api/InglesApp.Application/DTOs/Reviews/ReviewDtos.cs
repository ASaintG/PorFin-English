namespace InglesApp.Application.DTOs.Reviews;

public record ReviewItemDto(
    string ItemId,
    string Word,
    string Translation,
    string PartOfSpeech,
    string Category,
    IReadOnlyList<string> ExampleSentences,
    int Repetitions,
    DateTime NextReviewDate);

public record SubmitReviewRequest(int Quality);

public record SubmitReviewResponse(
    string ItemId,
    int Interval,
    int Repetitions,
    DateTime NextReviewDate,
    double EaseFactor);

public record BatchReviewItemRequest(string ItemId, int Quality);

public record BatchReviewRequest(IReadOnlyList<BatchReviewItemRequest> Reviews);

public record BatchReviewResponse(int Synced, IReadOnlyList<string> Failed);
