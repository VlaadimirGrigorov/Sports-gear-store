using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGearStore.Models.Questionnaire;
using System.Text.Json;

namespace SportsGearStore.Controllers;

[ApiController]
[Route("api/questionnaire")]
[AllowAnonymous]
public class QuestionnaireController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public QuestionnaireController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestionnaire()
    {
        var path = Path.Combine(_env.ContentRootPath, "Questionnaire", "Questions.json");

        if (!System.IO.File.Exists(path))
        {
            return NotFound("Questionnaire file not found.");
        }

        var json = await System.IO.File.ReadAllTextAsync(path);

        var questions = JsonSerializer.Deserialize<List<QuestionnaireQuestion>>(json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return Ok(questions);
    }
}
