using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Entities;

public class GrammarRule
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public CefrLevel CefrLevel { get; set; } = CefrLevel.A1;
    public string Category { get; set; } = string.Empty;
    public List<string> Examples { get; set; } = [];
    public List<string> RelatedStructures { get; set; } = [];
    public List<ProductionTask> ProductionTasks { get; set; } = [];
}
