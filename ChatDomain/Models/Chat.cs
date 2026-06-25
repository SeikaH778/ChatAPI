
namespace ChatDomain.Models
{
    public record Chat(Guid id, string chatName)
    {
        public ICollection<ChatMember> Members { get; init; } = new List<ChatMember>();
    }
}
