namespace InglesApp.Application.DTOs.Vocabulary;

public record VocabularyDto(
    string Id,
    string Word,
    string Translation,
    string PartOfSpeech,
    string CefrLevel,
    string Category,
    IReadOnlyList<string> ExampleSentences);

public record VocabularyListResponse(
    IReadOnlyList<VocabularyDto> Items,
    long Total,
    int Skip,
    int Take);
