using AdminPanel.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Rate> Rates { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>()
            .HasAlternateKey(u => u.Name);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Client)
            .WithMany()
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
