

namespace ChatDomain.Models
{
    public record ChatMember(Guid ChatId, Guid UserId, DateTime JoinedAt, ChatRole Role)
    {
        public Chat Chat { get; init; } = null!;
        public User User { get; init; } = null!;
    }
    public enum ChatRole
    {
        Member,
        Moderator,
        Owner
    }
}
