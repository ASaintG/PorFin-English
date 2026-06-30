using System.Security.Claims;
using InglesApp.Application.DTOs.Progress;
using InglesApp.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InglesApp.Api.Controllers;

[ApiController]
[Route("api/progress")]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressService _progressService;

    public ProgressController(IProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ProgressStatsDto>> GetStats(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException();

        var stats = await _progressService.GetStatsAsync(userId, cancellationToken);
        return Ok(stats);
    }
}
