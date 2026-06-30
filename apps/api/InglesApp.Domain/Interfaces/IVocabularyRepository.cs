using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface IVocabularyRepository
{
    Task<VocabularyItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<VocabularyItem>> GetByLevelAsync(CefrLevel level, int skip, int take, CancellationToken cancellationToken = default);
    Task<long> CountByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<VocabularyItem> items, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
