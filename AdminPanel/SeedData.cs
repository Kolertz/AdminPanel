using AdminPanel.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (!db.Tags.Any())
        {
            var tags = new List<Tag>
            {
                new() { Name = "VIP" },
                new() { Name = "Задолженность" },
                new() { Name = "Партнёр" }
            };

            db.Tags.AddRange(tags);

            db.SaveChanges();
        }

        if (!db.Users.Any())
        {
            db.Users.Add(new()
            {
                Email = "admin@mirra.dev",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
            });

            db.SaveChanges();
        }

        if (!db.Clients.Any())
        {
            var tags = db.Tags.ToList();

            List<Client> clients =
            [
                new() { Name = "Client 1", Email = "client1@example.com", Balance = 100, Tags = [tags[0]] },
                new() { Name = "Client 2", Email = "client2@example.com", Balance = 200, Tags = tags[..2] },
                new() { Name = "Client 3", Email = "client3@example.com", Balance = 300, Tags = tags }
            ];

            db.Clients.AddRange(clients);

            db.SaveChanges();
        }

        if (!db.Payments.Any())
        {
            var clients = db.Clients.ToList();

            db.Payments.AddRange(
                new() { Client = clients[0], Amount = 10, Date = DateTime.UtcNow.AddDays(-5), Description = "Payment 1" },
                new() { Client = clients[1], Amount = 20, Date = DateTime.UtcNow.AddDays(-4), Description = "Payment 2" },
                new() { Client = clients[2], Amount = 30, Date = DateTime.UtcNow.AddDays(-3), Description = "Payment 3" },
                new() { Client = clients[0], Amount = 40, Date = DateTime.UtcNow.AddDays(-2), Description = "Payment 4" },
                new() { Client = clients[1], Amount = 50, Date = DateTime.UtcNow.AddDays(-1), Description = "Payment 5" }
            );

            db.SaveChanges();
        }

        if (!db.Rates.Any())
        {
            db.Rates.Add(new() { Value = 10 });
        }

        db.SaveChanges();
    }
}
