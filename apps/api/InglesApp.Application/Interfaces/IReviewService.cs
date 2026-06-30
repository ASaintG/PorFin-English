using InglesApp.Application.DTOs.Reviews;

namespace InglesApp.Application.Interfaces;

public interface IReviewService
{
    Task<IReadOnlyList<ReviewItemDto>> GetDueItemsAsync(string userId, CancellationToken cancellationToken = default);
    Task<SubmitReviewResponse> SubmitReviewAsync(string userId, string itemId, int quality, CancellationToken cancellationToken = default);
    Task<BatchReviewResponse> SubmitBatchAsync(string userId, BatchReviewRequest request, CancellationToken cancellationToken = default);
}
