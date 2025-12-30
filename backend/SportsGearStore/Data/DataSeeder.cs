using SportsGearStore.Helpers;
using SportsGearStore.Models;
using System;

namespace SportsGearStore.Data
{
    public static class DataSeeder
    {
        public static void Seed(SportsGearStoreDbContext context)
        {
            context.Database.EnsureCreated();

            SeedDepartments(context);
            SeedCategories(context);
            SeedProducts(context);
            SeedAdminUser(context);
        }

        private static void SeedDepartments(SportsGearStoreDbContext context)
        {
            if (context.Departments.Any()) return;

            var departments = new List<Department>
            {
                new() { Name = "Strength Training", Description = "Equipment for strength and muscle building." },
                new() { Name = "Cardio & Running", Description = "Cardio machines and running accessories." },
                new() { Name = "Yoga & Mobility", Description = "Yoga, flexibility and recovery equipment." }
            };

            context.Departments.AddRange(departments);
            context.SaveChanges();
        }

        private static void SeedCategories(SportsGearStoreDbContext context)
        {
            if (context.Categories.Any()) return;

            var strength = context.Departments.First(d => d.Name == "Strength Training");
            var cardio = context.Departments.First(d => d.Name == "Cardio & Running");
            var yoga = context.Departments.First(d => d.Name == "Yoga & Mobility");

            var categories = new List<Category>
            {
                new() { CategoryName = "Weights" },
                new() { CategoryName = "Resistance Equipment" },
                new() { CategoryName = "Gym Accessories" },

                new() { CategoryName = "Running Equipment" },
                new() { CategoryName = "Cardio Machines" },
                new() { CategoryName = "Outdoor Cardio" },

                new() { CategoryName = "Yoga Equipment" },
                new() { CategoryName = "Mobility Tools" },
                new() { CategoryName = "Recovery Accessories" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();
        }

        private static void SeedProducts(SportsGearStoreDbContext context)
        {
            if (context.Products.Any()) return;

            var categories = context.Categories.ToDictionary(c => c.CategoryName, c => c.Id);

            var products = new List<Product>
            {
                // ============================
                // Weights (8)
                // ============================
                new() { CategoryId = categories["Weights"], Name = "Adjustable Dumbbells 20kg", Description = "Compact adjustable dumbbell set.", Price = 149.99m },
                new() { CategoryId = categories["Weights"], Name = "Barbell Set 50kg", Description = "Full barbell set for home gym.", Price = 299.99m },
                new() { CategoryId = categories["Weights"], Name = "Kettlebell 16kg", Description = "Cast iron kettlebell.", Price = 49.99m },
                new() { CategoryId = categories["Weights"], Name = "Hex Dumbbells 10kg", Description = "Rubber coated dumbbells.", Price = 79.99m },
                new() { CategoryId = categories["Weights"], Name = "Weight Plates 20kg", Description = "Olympic weight plates.", Price = 89.99m },
                new() { CategoryId = categories["Weights"], Name = "EZ Curl Bar", Description = "EZ bar for arm training.", Price = 69.99m },
                new() { CategoryId = categories["Weights"], Name = "Adjustable Weight Bench", Description = "Multi-angle workout bench.", Price = 199.99m },
                new() { CategoryId = categories["Weights"], Name = "Grip Strength Trainer", Description = "Hand grip strengthener.", Price = 14.99m },

                // ============================
                // Resistance Equipment (8)
                // ============================
                new() { CategoryId = categories["Resistance Equipment"], Name = "Resistance Bands Set", Description = "Set of 5 resistance bands.", Price = 29.99m },
                new() { CategoryId = categories["Resistance Equipment"], Name = "Pull-Up Bar", Description = "Doorway pull-up bar.", Price = 39.99m },
                new() { CategoryId = categories["Resistance Equipment"], Name = "Suspension Trainer", Description = "Bodyweight suspension trainer.", Price = 99.99m },
                new() { CategoryId = categories["Resistance Equipment"], Name = "Cable Resistance Kit", Description = "Cable workout system.", Price = 59.99m },
                new() { CategoryId = categories["Resistance Equipment"], Name = "Power Twister", Description = "Upper body resistance trainer.", Price = 19.99m },
                new() { CategoryId = categories["Resistance Equipment"], Name = "Heavy Resistance Band", Description = "Extra heavy resistance band.", Price = 9.99m },
                new() { CategoryId = categories["Resistance Equipment"], Name = "Adjustable Ankle Weights", Description = "Adjustable ankle weights set.", Price = 24.99m },
                new() { CategoryId = categories["Resistance Equipment"], Name = "Chest Expander", Description = "Classic chest expander.", Price = 14.99m },

                // ============================
                // Gym Accessories (8)
                // ============================
                new() { CategoryId = categories["Gym Accessories"], Name = "Gym Gloves", Description = "Protective gloves for workouts.", Price = 19.99m },
                new() { CategoryId = categories["Gym Accessories"], Name = "Weightlifting Belt", Description = "Support belt for heavy lifts.", Price = 34.99m },
                new() { CategoryId = categories["Gym Accessories"], Name = "Wrist Wraps", Description = "Wrist support wraps.", Price = 12.99m },
                new() { CategoryId = categories["Gym Accessories"], Name = "Knee Sleeves", Description = "Compression knee sleeves.", Price = 24.99m },
                new() { CategoryId = categories["Gym Accessories"], Name = "Chalk Block", Description = "Grip chalk for lifting.", Price = 6.99m },
                new() { CategoryId = categories["Gym Accessories"], Name = "Gym Towel", Description = "Sweat absorbent towel.", Price = 9.99m },
                new() { CategoryId = categories["Gym Accessories"], Name = "Protein Shaker", Description = "Shaker bottle for supplements.", Price = 7.99m },
                new() { CategoryId = categories["Gym Accessories"], Name = "Sports Water Bottle", Description = "1L BPA-free water bottle.", Price = 11.99m },

                // ============================
                // Running Equipment (8)
                // ============================
                new() { CategoryId = categories["Running Equipment"], Name = "Running Shoes - Men", Description = "Men's running shoes.", Price = 119.99m },
                new() { CategoryId = categories["Running Equipment"], Name = "Running Shoes - Women", Description = "Women's running shoes.", Price = 109.99m },
                new() { CategoryId = categories["Running Equipment"], Name = "Running Shorts", Description = "Lightweight running shorts.", Price = 29.99m },
                new() { CategoryId = categories["Running Equipment"], Name = "Compression Socks", Description = "Compression running socks.", Price = 19.99m },
                new() { CategoryId = categories["Running Equipment"], Name = "Running Jacket", Description = "Windproof running jacket.", Price = 89.99m },
                new() { CategoryId = categories["Running Equipment"], Name = "GPS Running Watch", Description = "Track your runs.", Price = 199.99m },
                new() { CategoryId = categories["Running Equipment"], Name = "Reflective Vest", Description = "Night running safety vest.", Price = 14.99m },
                new() { CategoryId = categories["Running Equipment"], Name = "Running Cap", Description = "Breathable running cap.", Price = 12.99m },

                // ============================
                // Cardio Machines (8)
                // ============================
                new() { CategoryId = categories["Cardio Machines"], Name = "Treadmill", Description = "Home cardio treadmill.", Price = 899.99m },
                new() { CategoryId = categories["Cardio Machines"], Name = "Exercise Bike", Description = "Indoor cycling bike.", Price = 499.99m },
                new() { CategoryId = categories["Cardio Machines"], Name = "Elliptical Trainer", Description = "Low-impact cardio trainer.", Price = 699.99m },
                new() { CategoryId = categories["Cardio Machines"], Name = "Rowing Machine", Description = "Full body rowing machine.", Price = 599.99m },
                new() { CategoryId = categories["Cardio Machines"], Name = "Mini Stepper", Description = "Compact stepper machine.", Price = 79.99m },
                new() { CategoryId = categories["Cardio Machines"], Name = "Air Bike", Description = "High-intensity air bike.", Price = 649.99m },
                new() { CategoryId = categories["Cardio Machines"], Name = "Under Desk Bike", Description = "Pedal exerciser.", Price = 69.99m },
                new() { CategoryId = categories["Cardio Machines"], Name = "Foldable Treadmill", Description = "Space-saving treadmill.", Price = 749.99m },

                // ============================
                // Outdoor Cardio (8)
                // ============================
                new() { CategoryId = categories["Outdoor Cardio"], Name = "Jump Rope", Description = "Speed jump rope.", Price = 15.99m },
                new() { CategoryId = categories["Outdoor Cardio"], Name = "Speed Ladder", Description = "Agility training ladder.", Price = 24.99m },
                new() { CategoryId = categories["Outdoor Cardio"], Name = "Agility Cones", Description = "Set of training cones.", Price = 19.99m },
                new() { CategoryId = categories["Outdoor Cardio"], Name = "Battle Rope", Description = "Heavy battle rope.", Price = 89.99m },
                new() { CategoryId = categories["Outdoor Cardio"], Name = "Resistance Parachute", Description = "Sprint resistance parachute.", Price = 29.99m },
                new() { CategoryId = categories["Outdoor Cardio"], Name = "Outdoor Pull-Up Bar", Description = "Outdoor pull-up station.", Price = 149.99m },
                new() { CategoryId = categories["Outdoor Cardio"], Name = "Weighted Vest", Description = "Adjustable weighted vest.", Price = 79.99m },
                new() { CategoryId = categories["Outdoor Cardio"], Name = "Training Sled", Description = "Strength sled for sprints.", Price = 199.99m },

                // ============================
                // Yoga Equipment (8)
                // ============================
                new() { CategoryId = categories["Yoga Equipment"], Name = "Yoga Mat", Description = "Non-slip yoga mat.", Price = 24.99m },
                new() { CategoryId = categories["Yoga Equipment"], Name = "Yoga Block", Description = "Foam yoga block.", Price = 9.99m },
                new() { CategoryId = categories["Yoga Equipment"], Name = "Yoga Strap", Description = "Stretching yoga strap.", Price = 8.99m },
                new() { CategoryId = categories["Yoga Equipment"], Name = "Yoga Wheel", Description = "Yoga back wheel.", Price = 34.99m },
                new() { CategoryId = categories["Yoga Equipment"], Name = "Meditation Cushion", Description = "Comfort meditation cushion.", Price = 29.99m },
                new() { CategoryId = categories["Yoga Equipment"], Name = "Yoga Towel", Description = "Non-slip yoga towel.", Price = 19.99m },
                new() { CategoryId = categories["Yoga Equipment"], Name = "Cork Yoga Mat", Description = "Eco-friendly cork mat.", Price = 59.99m },
                new() { CategoryId = categories["Yoga Equipment"], Name = "Travel Yoga Mat", Description = "Foldable travel mat.", Price = 34.99m },

                // ============================
                // Mobility Tools (8)
                // ============================
                new() { CategoryId = categories["Mobility Tools"], Name = "Foam Roller", Description = "Muscle recovery roller.", Price = 19.99m },
                new() { CategoryId = categories["Mobility Tools"], Name = "Massage Ball", Description = "Deep tissue massage ball.", Price = 9.99m },
                new() { CategoryId = categories["Mobility Tools"], Name = "Mobility Stick", Description = "Stretching stick.", Price = 24.99m },
                new() { CategoryId = categories["Mobility Tools"], Name = "Mini Resistance Bands", Description = "Mini band set.", Price = 14.99m },
                new() { CategoryId = categories["Mobility Tools"], Name = "Trigger Point Roller", Description = "Trigger point roller.", Price = 29.99m },
                new() { CategoryId = categories["Mobility Tools"], Name = "Stretching Strap", Description = "Flexibility strap.", Price = 12.99m },
                new() { CategoryId = categories["Mobility Tools"], Name = "Balance Board", Description = "Balance training board.", Price = 39.99m },
                new() { CategoryId = categories["Mobility Tools"], Name = "Posture Corrector", Description = "Posture support brace.", Price = 19.99m },

                // ============================
                // Recovery Accessories (8)
                // ============================
                new() { CategoryId = categories["Recovery Accessories"], Name = "Massage Gun", Description = "Electric massage gun.", Price = 129.99m },
                new() { CategoryId = categories["Recovery Accessories"], Name = "Ice Pack Wrap", Description = "Reusable ice pack wrap.", Price = 14.99m },
                new() { CategoryId = categories["Recovery Accessories"], Name = "Heat Therapy Belt", Description = "Thermal pain relief belt.", Price = 39.99m },
                new() { CategoryId = categories["Recovery Accessories"], Name = "Compression Boots", Description = "Leg recovery boots.", Price = 499.99m },
                new() { CategoryId = categories["Recovery Accessories"], Name = "Knee Ice Wrap", Description = "Cold therapy knee wrap.", Price = 19.99m },
                new() { CategoryId = categories["Recovery Accessories"], Name = "Recovery Slides", Description = "Recovery sandals.", Price = 34.99m },
                new() { CategoryId = categories["Recovery Accessories"], Name = "Sleep Mask", Description = "Relaxation sleep mask.", Price = 9.99m },
                new() { CategoryId = categories["Recovery Accessories"], Name = "Epsom Salt Pack", Description = "Muscle recovery salts.", Price = 7.99m }
            };

            context.Products.AddRange(products);
            context.SaveChanges();
        }

        private static void SeedAdminUser(SportsGearStoreDbContext context)
        {
            //if (context.Users.Any(u => u.IsAdmin)) return;

            var hashedPassword = GenericHelpers.ComputeSha256Hash("admin123");

            var admin = new User
            {
                Username = "admin",
                Useremail = "admin@sportsgearstore.com",
                Password = hashedPassword,
                FirstName = "Admin",
                LastName = "User",
                IsAdmin = true
            };

            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}
