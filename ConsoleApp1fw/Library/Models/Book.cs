namespace ConsoleApp1fw
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }

        public override string ToString()
        {
            return $"{Id} | {Title} | {AuthorId}";
        }
    }
}
