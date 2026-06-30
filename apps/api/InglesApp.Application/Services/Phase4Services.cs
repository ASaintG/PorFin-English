using InglesApp.Application.DTOs.Speaking;
using InglesApp.Application.DTOs.Writing;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using InglesApp.Domain.Services;

namespace InglesApp.Application.Services;

public class SpeakingService : ISpeakingService
{
    private readonly ISpeakingRepository _speakingRepository;
    private readonly IUserSubmissionRepository _submissionRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly SpacedRepetitionService _srs;

    public SpeakingService(
        ISpeakingRepository speakingRepository,
        IUserSubmissionRepository submissionRepository,
        IUserProgressRepository progressRepository,
        SpacedRepetitionService srs)
    {
        _speakingRepository = speakingRepository;
        _submissionRepository = submissionRepository;
        _progressRepository = progressRepository;
        _srs = srs;
    }

    public async Task<IReadOnlyList<SpeakingPromptDto>> GetPromptsAsync(CefrLevel level, string? category, CancellationToken ct = default)
    {
        var prompts = await _speakingRepository.GetByLevelAndCategoryAsync(level, category, ct);
        return prompts.Select(p => new SpeakingPromptDto(
            p.Id, p.Prompt, p.CefrLevel.ToString(), p.Category, p.ModelAnswer, p.KeyPhrases)).ToList();
    }

    public async Task<SubmitSpeakingResponse> SubmitAsync(string userId, SubmitSpeakingRequest request, CancellationToken ct = default)
    {
        var prompt = await _speakingRepository.GetByIdAsync(request.PromptId, ct)
            ?? throw new KeyNotFoundException("Prompt not found.");

        var transcript = request.Transcript.ToLowerInvariant();
        var matched = prompt.KeyPhrases
            .Where(k => transcript.Contains(k.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase))
            .ToList();

        await _submissionRepository.CreateAsync(new UserSubmission
        {
            UserId = userId,
            PromptId = request.PromptId,
            SubmissionType = "speaking",
            Content = request.Transcript,
            DurationSeconds = request.DurationSeconds,
            MatchedKeyPhrases = matched
        }, ct);

        var progress = await _progressRepository.GetByUserAndItemAsync(userId, request.PromptId, ct)
            ?? _srs.CreateInitialProgress(userId, request.PromptId, ItemType.Speaking);
        var quality = matched.Count >= prompt.KeyPhrases.Count / 2 ? 5 : 3;
        await _progressRepository.UpsertAsync(_srs.ApplyReview(progress, quality), ct);

        return new SubmitSpeakingResponse(matched, matched.Count, prompt.KeyPhrases.Count, prompt.ModelAnswer);
    }
}

public class WritingService : IWritingService
{
    private readonly IWritingRepository _writingRepository;
    private readonly IUserSubmissionRepository _submissionRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly SpacedRepetitionService _srs;

    public WritingService(
        IWritingRepository writingRepository,
        IUserSubmissionRepository submissionRepository,
        IUserProgressRepository progressRepository,
        SpacedRepetitionService srs)
    {
        _writingRepository = writingRepository;
        _submissionRepository = submissionRepository;
        _progressRepository = progressRepository;
        _srs = srs;
    }

    public async Task<IReadOnlyList<WritingPromptDto>> GetPromptsAsync(CefrLevel level, string? category, CancellationToken ct = default)
    {
        var prompts = await _writingRepository.GetByLevelAndCategoryAsync(level, category, ct);
        return prompts.Select(p => new WritingPromptDto(
            p.Id, p.Prompt, p.CefrLevel.ToString(), p.Category, p.WordLimit)).ToList();
    }

    public async Task<SubmitWritingResponse> SubmitAsync(string userId, SubmitWritingRequest request, CancellationToken ct = default)
    {
        var prompt = await _writingRepository.GetByIdAsync(request.PromptId, ct)
            ?? throw new KeyNotFoundException("Prompt not found.");

        var wordCount = request.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        await _submissionRepository.CreateAsync(new UserSubmission
        {
            UserId = userId,
            PromptId = request.PromptId,
            SubmissionType = "writing",
            Content = request.Text
        }, ct);

        var progress = await _progressRepository.GetByUserAndItemAsync(userId, request.PromptId, ct)
            ?? _srs.CreateInitialProgress(userId, request.PromptId, ItemType.Writing);
        var quality = wordCount >= prompt.WordLimit / 3 ? 5 : 3;
        await _progressRepository.UpsertAsync(_srs.ApplyReview(progress, quality), ct);

        return new SubmitWritingResponse(prompt.ModelAnswer, wordCount, prompt.WordLimit);
    }
}
