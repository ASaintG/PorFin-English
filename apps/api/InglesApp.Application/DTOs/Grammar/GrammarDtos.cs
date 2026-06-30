namespace InglesApp.Application.DTOs.Grammar;

public record GrammarRuleDto(
    string Id,
    string Title,
    string Explanation,
    string CefrLevel,
    string Category,
    IReadOnlyList<string> Examples,
    IReadOnlyList<string> RelatedStructures);
