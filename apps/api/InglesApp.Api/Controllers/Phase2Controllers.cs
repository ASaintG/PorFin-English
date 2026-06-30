using System.Security.Claims;
using InglesApp.Application.DTOs.Exercises;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InglesApp.Api.Controllers;

[ApiController]
[Route("api/grammar")]
[Authorize]
public class GrammarController : ControllerBase
{
    private readonly IGrammarService _grammarService;

    public GrammarController(IGrammarService grammarService) => _grammarService = grammarService;

    [HttpGet]
    public async Task<IActionResult> GetByLevel([FromQuery] string cefrLevel = "A1", CancellationToken ct = default)
    {
        if (!Enum.TryParse<CefrLevel>(cefrLevel, true, out var level))
            return BadRequest(new { message = "Invalid level." });
        return Ok(await _grammarService.GetByLevelAsync(level, ct));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        var rule = await _grammarService.GetByIdAsync(id, ct);
        return rule is null ? NotFound() : Ok(rule);
    }
}

[ApiController]
[Route("api/lessons")]
[Authorize]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService) => _lessonService = lessonService;

    [HttpGet]
    public async Task<IActionResult> GetByLevel([FromQuery] string cefrLevel = "A1", CancellationToken ct = default)
    {
        if (!Enum.TryParse<CefrLevel>(cefrLevel, true, out var level))
            return BadRequest(new { message = "Invalid level." });
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _lessonService.GetByLevelAsync(userId, level, ct));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        var lesson = await _lessonService.GetByIdAsync(id, ct);
        return lesson is null ? NotFound() : Ok(lesson);
    }

    [HttpPost("{id}/complete")]
    public async Task<IActionResult> Complete(string id, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _lessonService.CompleteLessonAsync(userId, id, ct);
        return Ok(new { message = "Lesson completed." });
    }
}

[ApiController]
[Route("api/exercises")]
[Authorize]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _exerciseService;
    private readonly IProgressService _progressService;

    public ExercisesController(IExerciseService exerciseService, IProgressService progressService)
    {
        _exerciseService = exerciseService;
        _progressService = progressService;
    }

    [HttpGet("due")]
    public async Task<IActionResult> GetDue(CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _progressService.UpdateStreakAsync(userId, ct);
        return Ok(await _exerciseService.GetDueAsync(userId, ct));
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(string id, [FromBody] SubmitExerciseRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _progressService.AddPracticeMinutesAsync(userId, 1, ct);
        return Ok(await _exerciseService.SubmitAsync(userId, id, request, ct));
    }
}
