using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface IGrammarRepository
{
    Task<GrammarRule?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GrammarRule>> GetByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GrammarRule>> GetAllAsync(CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<GrammarRule> rules, CancellationToken cancellationToken = default);
    Task UpdateAsync(GrammarRule rule, CancellationToken cancellationToken = default);
    Task DeleteByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
