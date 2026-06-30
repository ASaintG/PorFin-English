using InglesApp.Application.DTOs.Listening;
using InglesApp.Application.DTOs.Reading;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using InglesApp.Domain.Services;

namespace InglesApp.Application.Services;

public class ReadingService : IReadingService
{
    private readonly IReadingRepository _readingRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly SpacedRepetitionService _srs;

    public ReadingService(IReadingRepository readingRepository, IUserProgressRepository progressRepository, SpacedRepetitionService srs)
    {
        _readingRepository = readingRepository;
        _progressRepository = progressRepository;
        _srs = srs;
    }

    public async Task<IReadOnlyList<ReadingSummaryDto>> GetByLevelAsync(CefrLevel level, int skip, int take, CancellationToken ct = default)
    {
        var texts = await _readingRepository.GetByLevelAsync(level, skip, take, ct);
        return texts.Select(t => new ReadingSummaryDto(t.Id, t.Title, t.CefrLevel.ToString(), t.WordCount, t.Attribution)).ToList();
    }

    public async Task<ReadingDetailDto?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var t = await _readingRepository.GetByIdAsync(id, ct);
        if (t is null) return null;
        return new ReadingDetailDto(t.Id, t.Title, t.Body, t.CefrLevel.ToString(), t.WordCount, t.Source, t.Attribution,
            t.Questions.Select(q => new ComprehensionQuestionDto(q.Id, q.Question, q.Options)).ToList());
    }

    public async Task<SubmitComprehensionResponse> SubmitAsync(string userId, string id, SubmitComprehensionRequest request, CancellationToken ct = default)
    {
        var text = await _readingRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        var correct = 0;
        for (var i = 0; i < text.Questions.Count && i < request.Answers.Count; i++)
            if (request.Answers[i] == text.Questions[i].CorrectIndex) correct++;

        var quality = correct >= text.Questions.Count * 0.6 ? 5 : 2;
        var progress = await _progressRepository.GetByUserAndItemAsync(userId, id, ct)
            ?? _srs.CreateInitialProgress(userId, id, ItemType.Reading);
        await _progressRepository.UpsertAsync(_srs.ApplyReview(progress, quality), ct);

        var total = text.Questions.Count;
        return new SubmitComprehensionResponse(correct, total, total > 0 ? Math.Round(correct * 100.0 / total, 1) : 0);
    }
}

public class ListeningService : IListeningService
{
    private readonly IListeningRepository _listeningRepository;
    private readonly IUserProgressRepository _progressRepository;
    private readonly SpacedRepetitionService _srs;

    public ListeningService(IListeningRepository listeningRepository, IUserProgressRepository progressRepository, SpacedRepetitionService srs)
    {
        _listeningRepository = listeningRepository;
        _progressRepository = progressRepository;
        _srs = srs;
    }

    public async Task<IReadOnlyList<ListeningSummaryDto>> GetByLevelAsync(CefrLevel level, CancellationToken ct = default)
    {
        var items = await _listeningRepository.GetByLevelAsync(level, ct);
        return items.Select(l => new ListeningSummaryDto(l.Id, l.Title, l.CefrLevel.ToString(), l.Category)).ToList();
    }

    public async Task<ListeningDetailDto?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var l = await _listeningRepository.GetByIdAsync(id, ct);
        if (l is null) return null;
        return new ListeningDetailDto(l.Id, l.Title, l.Transcript, l.CefrLevel.ToString(), l.Category,
            l.Questions.Select(q => new ComprehensionQuestionDto(q.Id, q.Question, q.Options)).ToList());
    }

    public async Task<SubmitComprehensionResponse> SubmitAsync(string userId, string id, SubmitListeningRequest request, CancellationToken ct = default)
    {
        var item = await _listeningRepository.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException();
        var correct = 0;
        for (var i = 0; i < item.Questions.Count && i < request.Answers.Count; i++)
            if (request.Answers[i] == item.Questions[i].CorrectIndex) correct++;

        var quality = correct >= item.Questions.Count * 0.6 ? 5 : 2;
        var progress = await _progressRepository.GetByUserAndItemAsync(userId, id, ct)
            ?? _srs.CreateInitialProgress(userId, id, ItemType.Listening);
        await _progressRepository.UpsertAsync(_srs.ApplyReview(progress, quality), ct);

        var total = item.Questions.Count;
        return new SubmitComprehensionResponse(correct, total, total > 0 ? Math.Round(correct * 100.0 / total, 1) : 0);
    }
}
