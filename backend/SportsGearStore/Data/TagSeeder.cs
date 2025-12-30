using SportsGearStore.Data;
using SportsGearStore.Models;

namespace SportsGearStore.Data;

public static class TagSeeder
{
    public static void Seed(SportsGearStoreDbContext context)
    {
        // ако вече има тагове – не правим нищо
        if (context.Tags.Any())
            return;

        var tags = new List<Tag>
        {
            // Goals
            new Tag { Name = "weight_loss" },
            new Tag { Name = "muscle_gain" },
            new Tag { Name = "endurance" },

            // Level
            new Tag { Name = "beginner" },
            new Tag { Name = "intermediate" },
            new Tag { Name = "advanced" },

            // Training location
            new Tag { Name = "home_workout" },
            new Tag { Name = "gym_workout" },

            // General
            new Tag { Name = "cardio" },
            new Tag { Name = "strength" },
            new Tag { Name = "mobility" }
        };

        context.Tags.AddRange(tags);
        context.SaveChanges();
    }
}