using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPanel.Tests.Services;

public class PaymentServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _paymentService = new PaymentService(_dbContext);

        SeedTestData();
    }

    [Fact]
    public async Task GetRecentPaymentsAsync_ShouldReturnOrderedPayments()
    {
        var payments = (await _paymentService.GetRecentPaymentsAsync(2)).OrderByDescending(p => p.Date).ToList();

        Assert.Equal(2, payments.Count);
        Assert.Multiple(
            () => Assert.Equal(2, payments[0].Id),
            () => Assert.Equal(3, payments[1].Id),
            () => Assert.Equal(200, payments[0].Amount),
            () => Assert.Equal(300, payments[1].Amount)
        );
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    private void SeedTestData()
    {
        var a = new List<Client>
        {
            new() { Id = 1, Name = "Client A", Email = "a@test.com", Balance = 100 },
            new() { Id = 2, Name = "Client B", Email = "b@test.com", Balance = 200 }
        };

        var b = new List<Payment>
        {
            new() { Id = 1, Date = DateTime.UtcNow.AddDays(-3), Amount = 100, ClientId = 1 },
            new() { Id = 2, Date = DateTime.UtcNow.AddDays(-1), Amount = 200, ClientId = 2 },
            new() { Id = 3, Date = DateTime.UtcNow.AddDays(-2), Amount = 300, ClientId = 1 }
        };

        _dbContext.Clients.AddRange(a);
        _dbContext.Payments.AddRange(b);
        _dbContext.SaveChanges();
        _dbContext.ChangeTracker.Clear();
    }
}