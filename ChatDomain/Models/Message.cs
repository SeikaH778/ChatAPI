namespace ChatDomain.Models
{
    public record Message (Guid id, Guid userId, string content)
    {
        public User user { get; init; } = null!;
    }
}
