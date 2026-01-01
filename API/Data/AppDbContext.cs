using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ChatSession> ChatSessions { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<ChatSummary> ChatSummaries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ChatSession>(b =>
            {
                b.HasKey(x => x.ChatSessionId);
                b.Property(x => x.Status).HasConversion<string>();
                b.HasMany(x => x.Messages).WithOne(m => m.ChatSession).HasForeignKey(m => m.ChatSessionId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(x => x.Summary).WithOne(s => s.ChatSession).HasForeignKey<ChatSummary>(s => s.ChatSessionId);
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasKey(x => x.MessageId);
                b.Property(x => x.Role).HasConversion<string>();
                b.Property(x => x.Content).HasColumnType("nvarchar(max)");
                b.Property(x => x.ToolName).IsRequired(false);
            });

            modelBuilder.Entity<ChatSummary>(b =>
            {
                b.HasKey(x => x.ChatSessionId);
                b.Property(x => x.SummaryText).HasColumnType("nvarchar(max)");
            });
        }
    }
}
