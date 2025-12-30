namespace SportsGearStore.Models.Questionnaire
{
    public class QuestionnaireQuestion
    {
        public string Label { get; set; } = null!;
        public List<QuestionnaireAnswer> Answers { get; set; } = new();
    }
}
