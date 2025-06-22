using AdminPanel.Models;
using AdminPanel.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Users.Any() || db.Clients.Any() || db.Tags.Any() || db.Payments.Any())
        {
            return;
        }

        using var transaction = db.Database.BeginTransaction();

        try
        {
            // 1. Создаем теги
            var tags = new List<Tag>
            {
                new() { Name = "VIP" },
                new() { Name = "Задолженность" },
                new() { Name = "Партнёр" }
            };
            db.Tags.AddRange(tags);
            db.SaveChanges();

            // 2. Создаем админа
            var admin = new User
            {
                Email = "admin@mirra.dev",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
            };
            db.Users.Add(admin);
            db.SaveChanges();

            // 3. Создаем клиентов с привязкой тегов
            var clients = new List<Client>
            {
                new() { Name = "Client 1", Email = "client1@example.com", Balance = 100, Tags = [tags[0]] },
                new() { Name = "Client 2", Email = "client2@example.com", Balance = 200, Tags = tags[..2] },
                new() { Name = "Client 3", Email = "client3@example.com", Balance = 300, Tags = tags }
            };
            db.Clients.AddRange(clients);
            db.SaveChanges();

            // 4. Создаем платежи
            var payments = new List<Payment>
            {
                new() { Client = clients[0], Amount = 10, Date = DateTime.UtcNow.AddDays(-5), Description = "Payment 1" },
                new() { Client = clients[1], Amount = 20, Date = DateTime.UtcNow.AddDays(-4), Description = "Payment 2" },
                new() { Client = clients[2], Amount = 30, Date = DateTime.UtcNow.AddDays(-3), Description = "Payment 3" },
                new() { Client = clients[0], Amount = 40, Date = DateTime.UtcNow.AddDays(-2), Description = "Payment 4" },
                new() { Client = clients[1], Amount = 50, Date = DateTime.UtcNow.AddDays(-1), Description = "Payment 5" }
            };
            db.Payments.AddRange(payments);
            db.SaveChanges();

            // 5. Создаем тариф
            db.Rates.Add(new() { Value = 10 });
            db.SaveChanges();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}