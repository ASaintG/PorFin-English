using InglesApp.Application.DTOs.Vocabulary;
using InglesApp.Application.Interfaces;
using InglesApp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InglesApp.Api.Controllers;

[ApiController]
[Route("api/vocabulary")]
[Authorize]
public class VocabularyController : ControllerBase
{
    private readonly IVocabularyService _vocabularyService;

    public VocabularyController(IVocabularyService vocabularyService)
    {
        _vocabularyService = vocabularyService;
    }

    [HttpGet]
    public async Task<ActionResult<VocabularyListResponse>> GetByLevel(
        [FromQuery] string cefrLevel = "A1",
        [FromQuery] int skip = 0,
        [FromQuery] int take = 20,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<CefrLevel>(cefrLevel, true, out var level))
            return BadRequest(new { message = "Invalid CEFR level." });

        var result = await _vocabularyService.GetByLevelAsync(level, skip, take, cancellationToken);
        return Ok(result);
    }
}
