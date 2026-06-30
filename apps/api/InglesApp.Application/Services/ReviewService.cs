using InglesApp.Application.DTOs.Reviews;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Interfaces;
using InglesApp.Domain.Services;

namespace InglesApp.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUserProgressRepository _progressRepository;
    private readonly IVocabularyRepository _vocabularyRepository;
    private readonly SpacedRepetitionService _srs;

    public ReviewService(
        IUserProgressRepository progressRepository,
        IVocabularyRepository vocabularyRepository,
        SpacedRepetitionService srs)
    {
        _progressRepository = progressRepository;
        _vocabularyRepository = vocabularyRepository;
        _srs = srs;
    }

    public async Task<IReadOnlyList<ReviewItemDto>> GetDueItemsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var dueProgress = await _progressRepository.GetDueItemsAsync(userId, DateTime.UtcNow, cancellationToken);
        var results = new List<ReviewItemDto>();

        foreach (var progress in dueProgress)
        {
            var vocab = await _vocabularyRepository.GetByIdAsync(progress.ItemId, cancellationToken);
            if (vocab is null) continue;

            results.Add(new ReviewItemDto(
                vocab.Id,
                vocab.Word,
                vocab.Translation,
                vocab.PartOfSpeech,
                vocab.Category,
                vocab.ExampleSentences,
                progress.Repetitions,
                progress.NextReviewDate));
        }

        if (results.Count == 0)
        {
            var newWords = await _vocabularyRepository.GetByLevelAsync(
                Domain.Enums.CefrLevel.A1, 0, 20, cancellationToken);

            foreach (var vocab in newWords)
            {
                var existing = await _progressRepository.GetByUserAndItemAsync(userId, vocab.Id, cancellationToken);
                if (existing is not null) continue;

                var initial = _srs.CreateInitialProgress(userId, vocab.Id);
                await _progressRepository.UpsertAsync(initial, cancellationToken);

                results.Add(new ReviewItemDto(
                    vocab.Id,
                    vocab.Word,
                    vocab.Translation,
                    vocab.PartOfSpeech,
                    vocab.Category,
                    vocab.ExampleSentences,
                    0,
                    initial.NextReviewDate));
            }
        }

        return results;
    }

    public async Task<SubmitReviewResponse> SubmitReviewAsync(
        string userId,
        string itemId,
        int quality,
        CancellationToken cancellationToken = default)
    {
        var progress = await _progressRepository.GetByUserAndItemAsync(userId, itemId, cancellationToken)
            ?? _srs.CreateInitialProgress(userId, itemId);

        var updated = _srs.ApplyReview(progress, quality);
        var saved = await _progressRepository.UpsertAsync(updated, cancellationToken);

        return new SubmitReviewResponse(
            saved.ItemId,
            saved.Interval,
            saved.Repetitions,
            saved.NextReviewDate,
            saved.EaseFactor);
    }

    public async Task<BatchReviewResponse> SubmitBatchAsync(
        string userId,
        BatchReviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var failed = new List<string>();
        var synced = 0;

        foreach (var item in request.Reviews)
        {
            try
            {
                await SubmitReviewAsync(userId, item.ItemId, item.Quality, cancellationToken);
                synced++;
            }
            catch
            {
                failed.Add(item.ItemId);
            }
        }

        return new BatchReviewResponse(synced, failed);
    }
}
