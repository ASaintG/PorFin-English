using InglesApp.Application.DTOs.Vocabulary;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Application.Services;

public class VocabularyService : IVocabularyService
{
    private readonly IVocabularyRepository _vocabularyRepository;

    public VocabularyService(IVocabularyRepository vocabularyRepository)
    {
        _vocabularyRepository = vocabularyRepository;
    }

    public async Task<VocabularyListResponse> GetByLevelAsync(
        CefrLevel level,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 100);
        skip = Math.Max(0, skip);

        var items = await _vocabularyRepository.GetByLevelAsync(level, skip, take, cancellationToken);
        var total = await _vocabularyRepository.CountByLevelAsync(level, cancellationToken);

        var dtos = items.Select(v => new VocabularyDto(
            v.Id,
            v.Word,
            v.Translation,
            v.PartOfSpeech,
            v.CefrLevel.ToString(),
            v.Category,
            v.ExampleSentences)).ToList();

        return new VocabularyListResponse(dtos, total, skip, take);
    }
}
