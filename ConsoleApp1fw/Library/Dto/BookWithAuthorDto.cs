namespace ConsoleApp1fw
{
    public class BookWithAuthorDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        public override string ToString()
        {
            return $"{Id} | {Title} | {Author}";
        }
    }
}
