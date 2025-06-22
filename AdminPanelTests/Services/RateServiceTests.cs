using AdminPanel.Models;
using AdminPanel.Models.Entities;
using AdminPanel.Services;
using AdminPanel.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPanel.Tests.Services;

public class RateServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RateService _service;

    public RateServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"RateServiceTests_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _service = new RateService(_context);

        SeedTestData();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetCurrentRateAsync_ReturnsNull_WhenNoRatesExist()
    {
        // Arrange
        await ClearRatesAsync();

        // Act
        var result = await _service.GetCurrentRateAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCurrentRateAsync_ReturnsRate_WhenExists()
    {
        // Act
        var result = await _service.GetCurrentRateAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TestSeedData.DefaultRate.Value, result.Value);
    }

    [Fact]
    public async Task UpdateRateAsync_CreatesNewRate_WhenNoRatesExist()
    {
        // Arrange
        var newRate = new Rate { Value = 150.5m };

        await ClearRatesAsync();

        // Act
        var result = await _service.UpdateRateAsync(newRate);

        // Assert
        var dbRate = await _context.Rates.FirstAsync();
        Assert.Equal(newRate.Value, dbRate.Value);
        Assert.Equal(newRate.Value, result.Value);
    }

    [Fact]
    public async Task UpdateRateAsync_UpdatesExistingRate_WhenRateExists()
    {
        // Arrange
        var updatedRate = new Rate { Value = 200.0m };

        // Act
        var result = await _service.UpdateRateAsync(updatedRate);

        // Assert
        var dbRate = await _context.Rates.FirstAsync();
        Assert.Equal(updatedRate.Value, dbRate.Value);
        Assert.Equal(TestSeedData.DefaultRate.Id, dbRate.Id);
        Assert.Equal(updatedRate.Value, result.Value);
    }

    [Fact]
    public async Task UpdateRateAsync_ReturnsCorrectRate_AfterUpdate()
    {
        // Act
        var updatedRate = await _service.UpdateRateAsync(new Rate { Value = 250.0m });

        // Assert
        var currentRate = await _service.GetCurrentRateAsync();
        Assert.Equal(updatedRate.Value, currentRate?.Value);
    }

    private void SeedTestData()
    {
        _context.Rates.Add(TestSeedData.DefaultRate);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }

    private async Task ClearRatesAsync()
    {
        _context.Rates.RemoveRange(_context.Rates);
        await _context.SaveChangesAsync();
    }
}