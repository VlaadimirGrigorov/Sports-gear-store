namespace SportsGearStore.DTO
{
    public class UserProfileQuestionnaireDTO
    {
        public int Age { get; set; }
        public int Weight { get; set; }
        public int Height { get; set; }

        // Въпросник (enum-like)
        public string Goal { get; set; } = null!;          // weight_loss | muscle_gain
        public string Level { get; set; } = null!;         // beginner | intermediate | advanced
        public string TrainingPlace { get; set; } = null!; // home_workout | gym_workout
    }
}
