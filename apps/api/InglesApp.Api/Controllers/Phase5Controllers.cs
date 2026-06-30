using System.Security.Claims;
using InglesApp.Application.DTOs.Exam;
using InglesApp.Application.DTOs.Placement;
using InglesApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InglesApp.Api.Controllers;

[ApiController]
[Route("api/placement")]
[Authorize]
public class PlacementController : ControllerBase
{
    private readonly IPlacementService _placementService;

    public PlacementController(IPlacementService placementService) => _placementService = placementService;

    [HttpGet("questions")]
    public async Task<IActionResult> GetQuestions(CancellationToken ct = default) =>
        Ok(await _placementService.GetQuestionsAsync(ct));

    [HttpPost("complete")]
    public async Task<IActionResult> Complete([FromBody] PlacementCompleteRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _placementService.CompleteAsync(userId, request.Correct, request.Total, request.FinalLevel, ct));
    }
}

public record PlacementCompleteRequest(int Correct, int Total, string FinalLevel);

[ApiController]
[Route("api/daily")]
[Authorize]
public class DailyController : ControllerBase
{
    private readonly IDailySessionService _dailySessionService;

    public DailyController(IDailySessionService dailySessionService) => _dailySessionService = dailySessionService;

    [HttpGet("session")]
    public async Task<IActionResult> GetSession(CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _dailySessionService.GetDailySessionAsync(userId, ct));
    }
}

[ApiController]
[Route("api/exam")]
[Authorize]
public class ExamController : ControllerBase
{
    private readonly IExamSimulatorService _examService;
    private readonly IProgressService _progressService;

    public ExamController(IExamSimulatorService examService, IProgressService progressService)
    {
        _examService = examService;
        _progressService = progressService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _examService.StartExamAsync(userId, ct));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] SubmitExamRequest request, CancellationToken ct = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _progressService.AddPracticeMinutesAsync(userId, 30, ct);
        return Ok(await _examService.SubmitExamAsync(userId, request, ct));
    }
}
