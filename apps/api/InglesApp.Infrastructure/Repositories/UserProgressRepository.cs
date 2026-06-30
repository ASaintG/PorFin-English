using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using InglesApp.Infrastructure.Data;
using MongoDB.Driver;

namespace InglesApp.Infrastructure.Repositories;

public class UserProgressRepository : IUserProgressRepository
{
    private readonly IMongoCollection<UserProgress> _collection;

    public UserProgressRepository(MongoDbContext context)
    {
        _collection = context.UserProgress;
    }

    public async Task<UserProgress?> GetByUserAndItemAsync(
        string userId,
        string itemId,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.UserId == userId && p.ItemId == itemId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserProgress>> GetDueItemsAsync(
        string userId,
        DateTime asOf,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.UserId == userId && p.NextReviewDate <= asOf)
            .Limit(20)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserProgress>> GetDueByItemTypeAsync(
        string userId,
        ItemType itemType,
        DateTime asOf,
        CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(p => p.UserId == userId && p.ItemType == itemType && p.NextReviewDate <= asOf)
            .Limit(20)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserProgress> UpsertAsync(UserProgress progress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(progress.Id))
        {
            progress.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            await _collection.InsertOneAsync(progress, cancellationToken: cancellationToken);
            return progress;
        }

        await _collection.ReplaceOneAsync(
            p => p.Id == progress.Id,
            progress,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken);

        return progress;
    }

    public async Task<long> CountMasteredAsync(
        string userId,
        int minRepetitions = 3,
        CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(
            p => p.UserId == userId && p.Repetitions >= minRepetitions,
            cancellationToken: cancellationToken);
    }

    public async Task<long> CountMasteredByItemTypeAsync(
        string userId,
        ItemType itemType,
        int minRepetitions = 3,
        CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(
            p => p.UserId == userId && p.ItemType == itemType && p.Repetitions >= minRepetitions,
            cancellationToken: cancellationToken);
    }

    public async Task<long> CountDueAsync(
        string userId,
        DateTime asOf,
        CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(
            p => p.UserId == userId && p.NextReviewDate <= asOf,
            cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<UserProgress>> GetByUserAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(p => p.UserId == userId).ToListAsync(cancellationToken);
    }
}
