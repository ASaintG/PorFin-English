using InglesApp.Application.DTOs.Grammar;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Application.Services;

public class GrammarService : IGrammarService
{
    private readonly IGrammarRepository _grammarRepository;

    public GrammarService(IGrammarRepository grammarRepository) => _grammarRepository = grammarRepository;

    public async Task<IReadOnlyList<GrammarRuleDto>> GetByLevelAsync(CefrLevel level, CancellationToken ct = default)
    {
        var rules = await _grammarRepository.GetByLevelAsync(level, ct);
        return rules.Select(r => new GrammarRuleDto(
            r.Id, r.Title, r.Explanation, r.CefrLevel.ToString(),
            r.Category, r.Examples, r.RelatedStructures)).ToList();
    }

    public async Task<GrammarRuleDto?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var r = await _grammarRepository.GetByIdAsync(id, ct);
        return r is null ? null : new GrammarRuleDto(
            r.Id, r.Title, r.Explanation, r.CefrLevel.ToString(),
            r.Category, r.Examples, r.RelatedStructures);
    }
}
