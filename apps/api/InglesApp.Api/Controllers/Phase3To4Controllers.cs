using System.Security.Claims;
using InglesApp.Application.DTOs.Listening;
using InglesApp.Application.DTOs.Reading;
using InglesApp.Application.DTOs.Speaking;
using InglesApp.Application.DTOs.Suggestions;
using InglesApp.Application.DTOs.Writing;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InglesApp.Api.Controllers;

[ApiController]
[Route("api/reading")]
[Authorize]
public class ReadingController : ControllerBase
{
    private readonly IReadingService _readingService;
    private readonly IProgressService _progressService;

    public ReadingController(IReadingService readingService, IProgressService progressService)
    {
        _readingService = readingService;
        _progressService = progressService;
    }

    [HttpGet]
    public async Task<IActionResult> GetByLevel([FromQuery] string cefrLevel = "A1", [FromQuery] int skip = 0, [FromQuery] int take = 20, CancellationToken ct = default)
    {
        if (!Enum.TryParse<CefrLevel>(cefrLevel, true, out var level)) return BadRequest();
        return Ok(await _readingService.GetByLevelAsync(level, skip, take, ct));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        var text = await _readingService.GetByIdAsync(id, ct);
        return text is null ? NotFound() : Ok(text);
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(string id, [FromBody] SubmitComprehensionRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _progressService.AddPracticeMinutesAsync(userId, 5, ct);
        return Ok(await _readingService.SubmitAsync(userId, id, request, ct));
    }
}

[ApiController]
[Route("api/listening")]
[Authorize]
public class ListeningController : ControllerBase
{
    private readonly IListeningService _listeningService;
    private readonly IProgressService _progressService;

    public ListeningController(IListeningService listeningService, IProgressService progressService)
    {
        _listeningService = listeningService;
        _progressService = progressService;
    }

    [HttpGet("due")]
    public async Task<IActionResult> GetDue([FromQuery] string cefrLevel = "A1", CancellationToken ct = default)
    {
        if (!Enum.TryParse<CefrLevel>(cefrLevel, true, out var level)) return BadRequest();
        return Ok(await _listeningService.GetByLevelAsync(level, ct));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        var item = await _listeningService.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(string id, [FromBody] SubmitListeningRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _progressService.AddPracticeMinutesAsync(userId, 5, ct);
        return Ok(await _listeningService.SubmitAsync(userId, id, request, ct));
    }
}

[ApiController]
[Route("api/speaking")]
[Authorize]
public class SpeakingController : ControllerBase
{
    private readonly ISpeakingService _speakingService;
    private readonly IProgressService _progressService;

    public SpeakingController(ISpeakingService speakingService, IProgressService progressService)
    {
        _speakingService = speakingService;
        _progressService = progressService;
    }

    [HttpGet("prompts")]
    public async Task<IActionResult> GetPrompts([FromQuery] string cefrLevel = "A1", [FromQuery] string? category = null, CancellationToken ct = default)
    {
        if (!Enum.TryParse<CefrLevel>(cefrLevel, true, out var level)) return BadRequest();
        return Ok(await _speakingService.GetPromptsAsync(level, category, ct));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitSpeakingRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _progressService.AddPracticeMinutesAsync(userId, 5, ct);
        return Ok(await _speakingService.SubmitAsync(userId, request, ct));
    }
}

[ApiController]
[Route("api/writing")]
[Authorize]
public class WritingController : ControllerBase
{
    private readonly IWritingService _writingService;
    private readonly IProgressService _progressService;

    public WritingController(IWritingService writingService, IProgressService progressService)
    {
        _writingService = writingService;
        _progressService = progressService;
    }

    [HttpGet("prompts")]
    public async Task<IActionResult> GetPrompts([FromQuery] string cefrLevel = "A1", [FromQuery] string? category = null, CancellationToken ct = default)
    {
        if (!Enum.TryParse<CefrLevel>(cefrLevel, true, out var level)) return BadRequest();
        return Ok(await _writingService.GetPromptsAsync(level, category, ct));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitWritingRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _progressService.AddPracticeMinutesAsync(userId, 5, ct);
        return Ok(await _writingService.SubmitAsync(userId, request, ct));
    }
}

[ApiController]
[Route("api/suggestions")]
[Authorize]
public class SuggestionsController : ControllerBase
{
    private readonly ISuggestionService _suggestionService;

    public SuggestionsController(ISuggestionService suggestionService) => _suggestionService = suggestionService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSuggestionRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _suggestionService.CreateAsync(userId, request, ct);
        return Ok(new { message = "Suggestion received." });
    }
}
