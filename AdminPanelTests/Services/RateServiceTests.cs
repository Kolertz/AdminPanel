using AdminPanel.Models;
using AdminPanel.Models.Dtos;
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
    private readonly Rate _initialRate;

    public RateServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"RateServiceTests_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _service = new RateService(_context);
        _initialRate = new() { Id = 1, Value = 100.0m };

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

        Assert.Multiple(() =>
        {
            Assert.Equal(_initialRate.Value, result.Value);
            Assert.Equal(_initialRate.Id, result.Id);
        });
    }

    [Fact]
    public async Task UpdateRateAsync_CreatesNewRate_WhenNoRatesExist()
    {
        // Arrange
        var newRate = new RateDto { Value = 150.5m };
        await ClearRatesAsync();

        // Act
        var result = await _service.UpdateRateAsync(newRate);

        // Assert
        var dbRate = await _context.Rates.FirstAsync();

        Assert.Multiple(() =>
        {
            Assert.Equal(newRate.Value, dbRate.Value);
            Assert.Equal(newRate.Value, result.Value);
            Assert.NotEqual(default, dbRate.Id);
        });
    }

    [Fact]
    public async Task UpdateRateAsync_UpdatesExistingRate_WhenRateExists()
    {
        // Arrange
        var updatedRate = new RateDto { Value = 200.0m };
        var originalRate = await _context.Rates.FirstAsync();

        // Act
        var result = await _service.UpdateRateAsync(updatedRate);

        // Assert
        var dbRate = await _context.Rates.FirstAsync();

        Assert.Multiple(() =>
        {
            Assert.Equal(updatedRate.Value, dbRate.Value);
            Assert.Equal(updatedRate.Value, result.Value);
            Assert.Equal(_initialRate.Id, dbRate.Id);
        });
    }

    [Fact]
    public async Task UpdateRateAsync_ReturnsCorrectRate_AfterUpdate()
    {
        // Arrange
        var updatedRateValue = 250.0m;

        // Act
        var updatedRate = await _service.UpdateRateAsync(new RateDto { Value = updatedRateValue });

        // Assert
        var currentRate = await _service.GetCurrentRateAsync();

        Assert.Multiple(() =>
        {
            Assert.NotNull(currentRate);
            Assert.Equal(updatedRateValue, currentRate?.Value);
            Assert.Equal(_initialRate.Id, currentRate?.Id);
        });
    }

    private void SeedTestData()
    {
        _context.Rates.Add(_initialRate);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }

    private async Task ClearRatesAsync()
    {
        _context.Rates.RemoveRange(_context.Rates);
        await _context.SaveChangesAsync();
    }
}