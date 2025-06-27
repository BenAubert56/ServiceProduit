namespace ServiceLecture.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int QualityRating { get; set; }
        public int ValueForMoneyRating { get; set; }
        public int EaseOfUseRating { get; set; }
        public double Rating => (QualityRating + ValueForMoneyRating + EaseOfUseRating) / 3.0;
        public int ProductId { get; set; }
    }
}
