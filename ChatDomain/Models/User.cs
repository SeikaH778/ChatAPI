

namespace ChatDomain.Models
{
    public record User(Guid Id, string userName, string password)
    {
        public ICollection<Message> messages { get; init; } = new List<Message>();

    }
}
