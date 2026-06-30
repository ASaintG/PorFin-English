using InglesApp.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace InglesApp.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new EnumRepresentationConvention(BsonType.String)
        };
        ConventionRegistry.Register("inglesapp", pack, _ => true);

        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        EnsureIndexes();
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<VocabularyItem> VocabularyItems => _database.GetCollection<VocabularyItem>("vocabularyItems");
    public IMongoCollection<UserProgress> UserProgress => _database.GetCollection<UserProgress>("userProgress");
    public IMongoCollection<GrammarRule> GrammarRules => _database.GetCollection<GrammarRule>("grammarRules");
    public IMongoCollection<Exercise> Exercises => _database.GetCollection<Exercise>("exercises");
    public IMongoCollection<Lesson> Lessons => _database.GetCollection<Lesson>("lessons");
    public IMongoCollection<ReadingText> ReadingTexts => _database.GetCollection<ReadingText>("readingTexts");
    public IMongoCollection<ListeningExercise> ListeningExercises => _database.GetCollection<ListeningExercise>("listeningExercises");
    public IMongoCollection<SpeakingPrompt> SpeakingPrompts => _database.GetCollection<SpeakingPrompt>("speakingPrompts");
    public IMongoCollection<WritingPrompt> WritingPrompts => _database.GetCollection<WritingPrompt>("writingPrompts");
    public IMongoCollection<UserSubmission> UserSubmissions => _database.GetCollection<UserSubmission>("userSubmissions");
    public IMongoCollection<ContentSuggestion> ContentSuggestions => _database.GetCollection<ContentSuggestion>("contentSuggestions");

    private void EnsureIndexes()
    {
        Users.Indexes.CreateOne(new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true }));

        UserProgress.Indexes.CreateOne(new CreateIndexModel<UserProgress>(
            Builders<UserProgress>.IndexKeys.Ascending(p => p.UserId).Ascending(p => p.ItemId),
            new CreateIndexOptions { Unique = true }));

        UserProgress.Indexes.CreateOne(new CreateIndexModel<UserProgress>(
            Builders<UserProgress>.IndexKeys.Ascending(p => p.UserId).Ascending(p => p.NextReviewDate)));

        Lessons.Indexes.CreateOne(new CreateIndexModel<Lesson>(
            Builders<Lesson>.IndexKeys.Ascending(l => l.CefrLevel).Ascending(l => l.Sequence)));
    }
}
