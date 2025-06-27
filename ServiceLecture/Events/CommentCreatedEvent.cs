namespace ServiceLecture.Events
{
    public record CommentCreatedEvent(int Id, string Text, int QualityRating, int ValueForMoneyRating, int EaseOfUseRating, int ProductId);
}
