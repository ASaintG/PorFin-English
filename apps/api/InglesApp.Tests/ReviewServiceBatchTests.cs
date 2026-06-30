using InglesApp.Application.DTOs.Reviews;
using InglesApp.Application.Services;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using InglesApp.Domain.Services;

namespace InglesApp.Tests;

public class ReviewServiceBatchTests
{
    [Fact]
    public async Task SubmitBatchAsync_ProcessesAllItems()
    {
        var progressRepo = new FakeUserProgressRepository();
        var vocabRepo = new FakeVocabularyRepository();
        var srs = new SpacedRepetitionService();
        var service = new ReviewService(progressRepo, vocabRepo, srs);

        var result = await service.SubmitBatchAsync("user1", new BatchReviewRequest([
            new BatchReviewItemRequest("w1", 5),
            new BatchReviewItemRequest("w2", 4),
        ]));

        Assert.Equal(2, result.Synced);
        Assert.Empty(result.Failed);
    }

    private sealed class FakeUserProgressRepository : IUserProgressRepository
    {
        private readonly Dictionary<string, UserProgress> _store = new();

        public Task<UserProgress?> GetByUserAndItemAsync(string userId, string itemId, CancellationToken cancellationToken = default)
        {
            _store.TryGetValue($"{userId}:{itemId}", out var p);
            return Task.FromResult(p);
        }

        public Task<UserProgress> UpsertAsync(UserProgress progress, CancellationToken cancellationToken = default)
        {
            progress.Id = string.IsNullOrEmpty(progress.Id) ? Guid.NewGuid().ToString() : progress.Id;
            _store[$"{progress.UserId}:{progress.ItemId}"] = progress;
            return Task.FromResult(progress);
        }

        public Task<IReadOnlyList<UserProgress>> GetDueItemsAsync(string userId, DateTime asOf, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<UserProgress>>([]);

        public Task<IReadOnlyList<UserProgress>> GetDueByItemTypeAsync(string userId, ItemType itemType, DateTime asOf, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<UserProgress>>([]);

        public Task<long> CountMasteredByItemTypeAsync(string userId, ItemType itemType, int minRepetitions = 3, CancellationToken cancellationToken = default) =>
            Task.FromResult(0L);

        public Task<long> CountMasteredAsync(string userId, int minRepetitions = 3, CancellationToken cancellationToken = default) =>
            Task.FromResult(0L);

        public Task<long> CountDueAsync(string userId, DateTime asOf, CancellationToken cancellationToken = default) =>
            Task.FromResult(0L);

        public Task<IReadOnlyList<UserProgress>> GetByUserAsync(string userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<UserProgress>>([]);
    }

    private sealed class FakeVocabularyRepository : IVocabularyRepository
    {
        public Task<VocabularyItem?> GetByIdAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult<VocabularyItem?>(null);
        public Task<IReadOnlyList<VocabularyItem>> GetByLevelAsync(CefrLevel level, int skip, int take, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<VocabularyItem>>([]);
        public Task InsertManyAsync(IEnumerable<VocabularyItem> items, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<long> CountByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default) => Task.FromResult(0L);
    }
}
