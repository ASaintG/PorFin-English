using System.Security.Claims;
using InglesApp.Application.DTOs.Reviews;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InglesApp.Api.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IProgressService _progressService;

    public ReviewsController(IReviewService reviewService, IProgressService progressService)
    {
        _reviewService = reviewService;
        _progressService = progressService;
    }

    [HttpGet("due")]
    public async Task<ActionResult<IReadOnlyList<ReviewItemDto>>> GetDue(
        [FromQuery] CefrLevel? cefrLevel,
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();

        var items = await _reviewService.GetDueItemsAsync(userId, cefrLevel, category, cancellationToken);
        await _progressService.UpdateStreakAsync(userId, cancellationToken);
        return Ok(items);
    }

    [HttpPost("{itemId}")]
    public async Task<ActionResult<SubmitReviewResponse>> Submit(
        string itemId,
        [FromBody] SubmitReviewRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();

        var result = await _reviewService.SubmitReviewAsync(userId, itemId, request.Quality, cancellationToken);
        return Ok(result);
    }

    [HttpPost("batch")]
    public async Task<ActionResult<BatchReviewResponse>> SubmitBatch(
        [FromBody] BatchReviewRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();

        return Ok(await _reviewService.SubmitBatchAsync(userId, request, cancellationToken));
    }
}
