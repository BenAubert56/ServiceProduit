namespace ServiceCommentaire.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
