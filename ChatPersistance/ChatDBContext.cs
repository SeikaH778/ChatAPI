using Microsoft.EntityFrameworkCore;
using ChatDomain.Models;
namespace ChatPersistance
{
    public class ChatDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatMember> ChatMembers { get; set; }
        public ChatDBContext(DbContextOptions<ChatDBContext> options):base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<ChatMember>()
                .HasKey(cm => new { cm.ChatId, cm.UserId });

            modelBuilder.Entity<ChatMember>()
                .HasOne(cm => cm.Chat)
                .WithMany() 
                .HasForeignKey(cm => cm.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMember>()
                .HasOne(cm => cm.User)
                .WithMany() 
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ChatMember>()
                .Property(cm => cm.JoinedAt)
                .HasDefaultValueSql("NOW()");

            modelBuilder.Entity<Message>().HasKey(m => m.id);
            base.OnModelCreating(modelBuilder);
        }
    }
}
