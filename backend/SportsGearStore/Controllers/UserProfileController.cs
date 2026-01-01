using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsGearStore.Data;
using SportsGearStore.DTO;
using SportsGearStore.Models;
using System.Security.Claims;

namespace SportsGearStore.Controllers;

[Authorize]
[ApiController]
[Route("api/user-profile")]
public class UserProfileController : ControllerBase
{
    private readonly SportsGearStoreDbContext _context;

    public UserProfileController(SportsGearStoreDbContext context)
    {
        _context = context;
    }

    [Authorize(Policy = "CanSubmitQuestionnaire")]
    [HttpPost("create")]
    public async Task<IActionResult> SaveProfile(QuestionnaireQuestionTagsDTO dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        int profileId;

        if (profile != null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "User already has a profile");
        }

        profile = new UserProfile
        {
            UserId = userId.Value
        };

        _context.UserProfiles.Add(profile);

        await _context.SaveChangesAsync();
        profileId = profile.Id;

        var questionnaireQuestionTagLabels = dto.QuestionnaireQuestionTags;

        var matchingTags = await _context.Tags
            .Where(t => questionnaireQuestionTagLabels.Contains(t.Name))
            .ToListAsync();

        foreach(var tag in matchingTags)
        {
            _context.UserProfileTags.Add(new UserProfileTag
            {
                UserProfileId = profileId,
                TagId = tag.Id
            });
        }

        await _context.SaveChangesAsync();

        return Ok("Profile saved successfully.");
    }

    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        new Claim("permission", "questionnaire.submit");
        if (claim == null) return null;

        return int.TryParse(claim.Value, out var id) ? id : null;
    }
}
