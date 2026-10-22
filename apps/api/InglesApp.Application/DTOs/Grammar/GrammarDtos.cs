namespace InglesApp.Application.DTOs.Grammar;

public record ProductionTaskDto(
    string Type,
    string Prompt,
    string? TtsText,
    string? AudioUrl,
    IReadOnlyList<string> Questions);

public record GrammarRuleDto(
    string Id,
    string Title,
    string Explanation,
    string CefrLevel,
    string Category,
    IReadOnlyList<string> Examples,
    IReadOnlyList<string> RelatedStructures,
    IReadOnlyList<ProductionTaskDto> ProductionTasks);
