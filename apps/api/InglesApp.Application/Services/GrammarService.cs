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
        return rules.Select(ToDto).ToList();
    }

    public async Task<GrammarRuleDto?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var rule = await _grammarRepository.GetByIdAsync(id, ct);
        return rule is null ? null : ToDto(rule);
    }

    private static GrammarRuleDto ToDto(Domain.Entities.GrammarRule rule) =>
        new(
            rule.Id,
            rule.Title,
            rule.Explanation,
            rule.CefrLevel.ToString(),
            rule.Category,
            rule.Examples,
            rule.RelatedStructures,
            rule.ProductionTasks.Select(task => new ProductionTaskDto(
                task.Type,
                task.Prompt,
                task.TtsText,
                task.AudioUrl,
                task.Questions)).ToList());
}
