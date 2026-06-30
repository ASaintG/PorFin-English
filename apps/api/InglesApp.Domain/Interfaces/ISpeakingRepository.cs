using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface ISpeakingRepository
{
    Task<SpeakingPrompt?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SpeakingPrompt>> GetByLevelAndCategoryAsync(CefrLevel level, string? category, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<SpeakingPrompt> prompts, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
