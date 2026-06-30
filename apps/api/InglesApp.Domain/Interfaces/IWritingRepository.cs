using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface IWritingRepository
{
    Task<WritingPrompt?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WritingPrompt>> GetByLevelAndCategoryAsync(CefrLevel level, string? category, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<WritingPrompt> prompts, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
