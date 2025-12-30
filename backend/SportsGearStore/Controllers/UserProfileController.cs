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

    // CREATE or UPDATE PROFILE
    [HttpPost]
    public async Task<IActionResult> SaveProfile(UserProfileQuestionnaireDTO dto)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        // 1️⃣ Profile (upsert)
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new UserProfile
            {
                UserId = userId.Value
            };
            _context.UserProfiles.Add(profile);
        }

        profile.Age = dto.Age;
        profile.Weight = dto.Weight;
        profile.Height = dto.Height;

        await _context.SaveChangesAsync();

        await UpdateUserProfileTags(profile.Id, dto);

        return Ok("Profile saved successfully.");
    }

    // HELPERS
    private int? GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return null;

        return int.TryParse(claim.Value, out var id) ? id : null;
    }

    private async Task UpdateUserProfileTags(int userProfileId, UserProfileQuestionnaireDTO dto)
    {
        // махаме старите тагове
        var existing = _context.UserProfileTags.Where(upt => upt.UserProfileId == userProfileId);

        _context.UserProfileTags.RemoveRange(existing);

        // новите тагове (по име)
        var tagNames = new[]
        {
            dto.Goal,
            dto.Level,
            dto.TrainingPlace
        };

        var tags = await _context.Tags
            .Where(t => tagNames.Contains(t.Name))
            .ToListAsync();

        foreach (var tag in tags)
        {
            _context.UserProfileTags.Add(new UserProfileTag
            {
                UserProfileId = userProfileId,
                TagId = tag.Id
            });
        }

        await _context.SaveChangesAsync();
    }
}
