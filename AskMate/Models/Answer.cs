namespace AskMate.Models
{
    public class Answer
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int Question_Id { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
