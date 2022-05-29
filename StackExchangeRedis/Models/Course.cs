namespace StackExchangeRedis.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? AuthorId { get; set; }
        public string? Author { get; set; }
        public string? Link { get; set; }

        public Course()
        {
            Link = string.Empty;
            AuthorId = string.Empty;
            Author = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            Id = 0;
        }
    }
}
