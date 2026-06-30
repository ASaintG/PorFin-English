using InglesApp.Infrastructure.Seed;
using Microsoft.AspNetCore.Mvc;

namespace InglesApp.Api.Controllers;

[ApiController]
[Route("api/seed")]
public class SeedController : ControllerBase
{
    private readonly VocabularySeeder _seeder;
    private readonly IWebHostEnvironment _environment;

    public SeedController(VocabularySeeder seeder, IWebHostEnvironment environment)
    {
        _seeder = seeder;
        _environment = environment;
    }

    [HttpPost]
    public async Task<IActionResult> Seed(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
            return NotFound();

        var count = await _seeder.SeedAsync(cancellationToken);
        return Ok(new { message = count == 0 ? "Vocabulary already seeded." : $"Seeded {count} words.", count });
    }
}
