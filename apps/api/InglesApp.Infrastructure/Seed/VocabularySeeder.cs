using System.Reflection;
using System.Text.Json;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using MongoDB.Bson;

namespace InglesApp.Infrastructure.Seed;

public class VocabularySeeder
{
    private readonly IVocabularyRepository _vocabularyRepository;

    public VocabularySeeder(IVocabularyRepository vocabularyRepository)
    {
        _vocabularyRepository = vocabularyRepository;
    }

    public async Task<int> SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _vocabularyRepository.AnyAsync(cancellationToken))
            return 0;

        var items = LoadVocabulary();
        await _vocabularyRepository.InsertManyAsync(items, cancellationToken);
        return items.Count;
    }

    private static List<VocabularyItem> LoadVocabulary()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "InglesApp.Infrastructure.Seed.a1-vocabulary.json";
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        var seedItems = JsonSerializer.Deserialize<List<VocabularySeedItem>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? [];

        return seedItems.Select(s => new VocabularyItem
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Word = s.Word,
            Translation = s.Translation,
            PartOfSpeech = s.PartOfSpeech,
            CefrLevel = CefrLevel.A1,
            Category = s.Category,
            ExampleSentences = s.ExampleSentences
        }).ToList();
    }

    private sealed class VocabularySeedItem
    {
        public string Word { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string PartOfSpeech { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> ExampleSentences { get; set; } = [];
    }
}
