using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using InglesApp.Infrastructure.Data;
using MongoDB.Driver;

namespace InglesApp.Infrastructure.Repositories;

public class GrammarRepository : IGrammarRepository
{
    private readonly IMongoCollection<GrammarRule> _collection;

    public GrammarRepository(MongoDbContext context) => _collection = context.GrammarRules;

    public async Task<GrammarRule?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(g => g.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<GrammarRule>> GetByLevelAsync(CefrLevel level, CancellationToken ct = default) =>
        await _collection.Find(g => g.CefrLevel == level).ToListAsync(ct);

    public async Task InsertManyAsync(IEnumerable<GrammarRule> rules, CancellationToken ct = default)
    {
        var list = rules.ToList();
        if (list.Count > 0) await _collection.InsertManyAsync(list, cancellationToken: ct);
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default) =>
        await _collection.CountDocumentsAsync(FilterDefinition<GrammarRule>.Empty, cancellationToken: ct) > 0;
}

public class ExerciseRepository : IExerciseRepository
{
    private readonly IMongoCollection<Exercise> _collection;

    public ExerciseRepository(MongoDbContext context) => _collection = context.Exercises;

    public async Task<Exercise?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(e => e.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Exercise>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        return await _collection.Find(e => idList.Contains(e.Id)).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Exercise>> GetByLevelAndSkillAsync(CefrLevel level, Skill skill, CancellationToken ct = default) =>
        await _collection.Find(e => e.CefrLevel == level && e.Skill == skill).ToListAsync(ct);

    public async Task<IReadOnlyList<Exercise>> GetByLessonIdAsync(string lessonId, CancellationToken ct = default) =>
        await _collection.Find(e => e.LessonId == lessonId).ToListAsync(ct);

    public async Task InsertManyAsync(IEnumerable<Exercise> exercises, CancellationToken ct = default)
    {
        var list = exercises.ToList();
        if (list.Count > 0) await _collection.InsertManyAsync(list, cancellationToken: ct);
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default) =>
        await _collection.CountDocumentsAsync(FilterDefinition<Exercise>.Empty, cancellationToken: ct) > 0;
}

public class LessonRepository : ILessonRepository
{
    private readonly IMongoCollection<Lesson> _collection;

    public LessonRepository(MongoDbContext context) => _collection = context.Lessons;

    public async Task<Lesson?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await _collection.Find(l => l.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<Lesson>> GetByLevelAsync(CefrLevel level, CancellationToken ct = default) =>
        await _collection.Find(l => l.CefrLevel == level).SortBy(l => l.Sequence).ToListAsync(ct);

    public async Task<IReadOnlyList<Lesson>> GetAllSortedAsync(CancellationToken ct = default) =>
        await _collection.Find(FilterDefinition<Lesson>.Empty).SortBy(l => l.Sequence).ToListAsync(ct);

    public async Task InsertManyAsync(IEnumerable<Lesson> lessons, CancellationToken ct = default)
    {
        var list = lessons.ToList();
        if (list.Count > 0) await _collection.InsertManyAsync(list, cancellationToken: ct);
    }

    public async Task<bool> AnyAsync(CancellationToken ct = default) =>
        await _collection.CountDocumentsAsync(FilterDefinition<Lesson>.Empty, cancellationToken: ct) > 0;
}
