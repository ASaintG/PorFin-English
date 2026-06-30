using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using InglesApp.Infrastructure.Data;
using MongoDB.Driver;

namespace InglesApp.Infrastructure.Repositories;

public class VocabularyRepository : IVocabularyRepository
{
    private readonly IMongoCollection<VocabularyItem> _collection;

    public VocabularyRepository(MongoDbContext context)
    {
        _collection = context.VocabularyItems;
    }

    public async Task<VocabularyItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(v => v.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VocabularyItem>> GetByLevelAsync(
        CefrLevel level,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(v => v.CefrLevel == level)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(v => v.CefrLevel == level, cancellationToken: cancellationToken);
    }

    public async Task InsertManyAsync(IEnumerable<VocabularyItem> items, CancellationToken cancellationToken = default)
    {
        var list = items.ToList();
        if (list.Count == 0) return;
        await _collection.InsertManyAsync(list, cancellationToken: cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(FilterDefinition<VocabularyItem>.Empty, cancellationToken: cancellationToken) > 0;
    }
}
