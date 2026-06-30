namespace InglesApp.Application.DTOs.Suggestions;

public record CreateSuggestionRequest(string Type, string Content, string? RelatedItemId);
