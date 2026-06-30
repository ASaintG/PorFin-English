using InglesApp.Domain.Entities;

namespace InglesApp.Domain.Interfaces;

public interface IContentSuggestionRepository
{
    Task<ContentSuggestion> CreateAsync(ContentSuggestion suggestion, CancellationToken cancellationToken = default);
}
