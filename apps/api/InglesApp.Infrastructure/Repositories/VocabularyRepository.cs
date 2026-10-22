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

    public async Task<IReadOnlyList<VocabularyItem>> GetByLevelAndCategoryAsync(
        CefrLevel level,
        string? category,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var filter = string.IsNullOrWhiteSpace(category)
            ? Builders<VocabularyItem>.Filter.Eq(v => v.CefrLevel, level)
            : Builders<VocabularyItem>.Filter.Eq(v => v.CefrLevel, level)
              & Builders<VocabularyItem>.Filter.Eq(v => v.Category, category);

        return await _collection
            .Find(filter)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> CountByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(v => v.CefrLevel == level, cancellationToken: cancellationToken);
    }

    public async Task<long> CountByLevelAndCategoryAsync(
        CefrLevel level,
        string category,
        CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(
            v => v.CefrLevel == level && v.Category == category,
            cancellationToken: cancellationToken);
    }

    public async Task InsertManyAsync(IEnumerable<VocabularyItem> items, CancellationToken cancellationToken = default)
    {
        var list = items.ToList();
        if (list.Count == 0) return;
        await _collection.InsertManyAsync(list, cancellationToken: cancellationToken);
    }

    public async Task UpdateManyAsync(IEnumerable<VocabularyItem> items, CancellationToken cancellationToken = default)
    {
        var list = items.ToList();
        if (list.Count == 0) return;
        var models = list.Select(item => new ReplaceOneModel<VocabularyItem>(
            Builders<VocabularyItem>.Filter.Eq(v => v.Id, item.Id), item));
        await _collection.BulkWriteAsync(models, cancellationToken: cancellationToken);
    }

    public async Task DeleteManyAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
    {
        var list = ids.ToList();
        if (list.Count == 0) return;
        var filter = Builders<VocabularyItem>.Filter.In(v => v.Id, list);
        await _collection.DeleteManyAsync(filter, cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(FilterDefinition<VocabularyItem>.Empty, cancellationToken: cancellationToken) > 0;
    }
}
