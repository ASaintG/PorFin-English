using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using InglesApp.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InglesApp.Infrastructure.Repositories;

public class ReadingRepository : IReadingRepository
{
    private readonly IMongoCollection<ReadingText> _collection;

    public ReadingRepository(MongoDbContext context) => _collection = context.ReadingTexts;

    public async Task<ReadingText?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(r => r.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ReadingText>> GetByLevelAsync(CefrLevel level, int skip, int take, CancellationToken ct = default) =>
        await _collection.Find(r => r.CefrLevel == level).Skip(skip).Limit(take).ToListAsync(ct);

    public async Task InsertManyAsync(IEnumerable<ReadingText> texts, CancellationToken ct = default)
    {
        var list = texts.ToList();
        if (list.Count > 0) await _collection.InsertManyAsync(list, cancellationToken: ct);
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default) =>
        await _collection.CountDocumentsAsync(FilterDefinition<ReadingText>.Empty, cancellationToken: ct) > 0;
}

public class ListeningRepository : IListeningRepository
{
    private readonly IMongoCollection<ListeningExercise> _collection;

    public ListeningRepository(MongoDbContext context) => _collection = context.ListeningExercises;

    public async Task<ListeningExercise?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(l => l.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ListeningExercise>> GetByLevelAsync(CefrLevel level, CancellationToken ct = default) =>
        await _collection.Find(l => l.CefrLevel == level).ToListAsync(ct);

    public async Task InsertManyAsync(IEnumerable<ListeningExercise> exercises, CancellationToken ct = default)
    {
        var list = exercises.ToList();
        if (list.Count > 0) await _collection.InsertManyAsync(list, cancellationToken: ct);
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default) =>
        await _collection.CountDocumentsAsync(FilterDefinition<ListeningExercise>.Empty, cancellationToken: ct) > 0;
}

public class SpeakingRepository : ISpeakingRepository
{
    private readonly IMongoCollection<SpeakingPrompt> _collection;

    public SpeakingRepository(MongoDbContext context) => _collection = context.SpeakingPrompts;

    public async Task<SpeakingPrompt?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(s => s.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<SpeakingPrompt>> GetByLevelAndCategoryAsync(CefrLevel level, string? category, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(category))
            return await _collection.Find(s => s.CefrLevel == level).ToListAsync(ct);
        return await _collection.Find(s => s.CefrLevel == level && s.Category == category).ToListAsync(ct);
    }

    public async Task InsertManyAsync(IEnumerable<SpeakingPrompt> prompts, CancellationToken ct = default)
    {
        var list = prompts.ToList();
        if (list.Count > 0) await _collection.InsertManyAsync(list, cancellationToken: ct);
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default) =>
        await _collection.CountDocumentsAsync(FilterDefinition<SpeakingPrompt>.Empty, cancellationToken: ct) > 0;
}

public class WritingRepository : IWritingRepository
{
    private readonly IMongoCollection<WritingPrompt> _collection;

    public WritingRepository(MongoDbContext context) => _collection = context.WritingPrompts;

    public async Task<WritingPrompt?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(w => w.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<WritingPrompt>> GetByLevelAndCategoryAsync(CefrLevel level, string? category, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(category))
            return await _collection.Find(w => w.CefrLevel == level).ToListAsync(ct);
        return await _collection.Find(w => w.CefrLevel == level && w.Category == category).ToListAsync(ct);
    }

    public async Task InsertManyAsync(IEnumerable<WritingPrompt> prompts, CancellationToken ct = default)
    {
        var list = prompts.ToList();
        if (list.Count > 0) await _collection.InsertManyAsync(list, cancellationToken: ct);
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default) =>
        await _collection.CountDocumentsAsync(FilterDefinition<WritingPrompt>.Empty, cancellationToken: ct) > 0;
}

public class UserSubmissionRepository : IUserSubmissionRepository
{
    private readonly IMongoCollection<UserSubmission> _collection;

    public UserSubmissionRepository(MongoDbContext context) => _collection = context.UserSubmissions;

    public async Task<UserSubmission> CreateAsync(UserSubmission submission, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(submission.Id))
            submission.Id = ObjectId.GenerateNewId().ToString();
        await _collection.InsertOneAsync(submission, cancellationToken: ct);
        return submission;
    }

    public async Task<IReadOnlyList<UserSubmission>> GetByUserAsync(string userId, CancellationToken ct = default) =>
        await _collection.Find(s => s.UserId == userId).ToListAsync(ct);
}

public class ContentSuggestionRepository : IContentSuggestionRepository
{
    private readonly IMongoCollection<ContentSuggestion> _collection;

    public ContentSuggestionRepository(MongoDbContext context) => _collection = context.ContentSuggestions;

    public async Task<ContentSuggestion> CreateAsync(ContentSuggestion suggestion, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(suggestion.Id))
            suggestion.Id = ObjectId.GenerateNewId().ToString();
        await _collection.InsertOneAsync(suggestion, cancellationToken: ct);
        return suggestion;
    }
}
