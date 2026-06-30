using InglesApp.Application.DTOs.Vocabulary;
using InglesApp.Domain.Enums;

namespace InglesApp.Application.Interfaces;

public interface IVocabularyService
{
    Task<VocabularyListResponse> GetByLevelAsync(CefrLevel level, int skip, int take, CancellationToken cancellationToken = default);
}
