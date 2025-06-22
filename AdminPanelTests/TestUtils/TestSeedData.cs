using AdminPanel.Models.Entities;
using System.Collections.Generic;

namespace AdminPanel.Tests.TestData;

public static class TestSeedData
{
    public static List<Client> Clients() =>
    [
        new() { Id = 1, Name = "Client A", Email = "a@test.com", Balance = 100 },
        new() { Id = 2, Name = "Client B", Email = "b@test.com", Balance = 200 }
    ];

    public static List<Payment> Payments() =>
    [
        new() { Id = 1, Date = DateTime.UtcNow.AddDays(-3), Amount = 100, ClientId = 0 },
        new() { Id = 2, Date = DateTime.UtcNow.AddDays(-1), Amount = 200, ClientId = 1 },
        new() { Id = 3, Date = DateTime.UtcNow.AddDays(-2), Amount = 300, ClientId = 0 }
    ];

    public static List<Tag> Tags() =>
    [
        new() { Id = 1, Name = "Tag1" },
        new() { Id = 2, Name = "Tag2" }
    ];

    public static Rate DefaultRate => new() { Id = 1, Value = 100.0m };
}