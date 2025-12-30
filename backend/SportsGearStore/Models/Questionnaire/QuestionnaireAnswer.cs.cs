namespace SportsGearStore.Models.Questionnaire
{
    public class QuestionnaireAnswer
    {
        public string AnswerValue { get; set; } = null!;
        public List<string> AnswerTags { get; set; } = new();
    }
}
