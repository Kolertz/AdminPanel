using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using NuGet.Frameworks;
using Xunit;

namespace AdminPanel.Tests.Services;

public class ClientServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ClientService _service;

    public ClientServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new ClientService(_context);

        SeedTestData();
    }

    [Fact]
    public async Task GetAllClientsAsync_ReturnsAllClients()
    {
        // Act
        var result = await _service.GetAllClientsAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetClientByIdAsync_ReturnsClient_WhenExists()
    {
        // Act
        var result = await _service.GetClientByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Client A", result.Name);
    }

    [Fact]
    public async Task GetClientByIdAsync_ReturnsNull_WhenNotExists()
    {
        // Act
        var result = await _service.GetClientByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateClientAsync_AddsNewClient()
    {
        // Arrange
        var newClient = new Client { Name = "New Client", Email = "new@test.com" };

        // Act
        var result = await _service.CreateClientAsync(newClient);

        // Assert
        Assert.Equal(3, await _context.Clients.CountAsync());
        Assert.Equal("New Client", result.Name);
    }
    
    [Fact]
    public async Task UpdateClientAsync_UpdatesExistingClient()
    {
        // Arrange
        var updatedClient = new Client { Id = 1, Name = "Updated", Email = "updated@test.com", Balance = 500 };

        // Act
        var result = await _service.UpdateClientAsync(updatedClient.Id, updatedClient);

        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(updatedClient, result);
    }

    [Fact]
    public async Task UpdateClientAsync_ReturnsFalse_WhenClientNotFound()
    {
        // Act
        var result = await _service.UpdateClientAsync(99, new Client());

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteClientAsync_RemovesClient()
    {
        // Act
        var result = await _service.DeleteClientAsync(1);

        // Assert
        Assert.True(result);
        Assert.Null(await _context.Clients.FindAsync(1));
    }

    [Fact]
    public async Task DeleteClientAsync_ReturnsFalse_WhenClientNotFound()
    {
        // Act
        var result = await _service.DeleteClientAsync(99);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetClientTagsAsync_ReturnsTags()
    {
        // Arrange
        var client = await _context.Clients.Include(c => c.Tags).FirstAsync();
        client.Tags!.Add(TestSeedData.Tags()[0]);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _service.GetClientTagsAsync(1);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task AddTagToClientAsync_AddsTag()
    {
        // Act
        var result = await _service.AddTagToClientAsync(1, 1);

        // Assert
        Assert.True(result);
        var client = await _context.Clients.Include(c => c.Tags).FirstAsync();
        Assert.NotNull(client.Tags);
        Assert.Contains(client.Tags, t => t.Id == 1);
    }

    [Fact]
    public async Task AddTagToClientAsync_ReturnsFalse_WhenClientOrTagNotFound()
    {
        // Assert
        Assert.False(await _service.AddTagToClientAsync(99, 1));
        Assert.False(await _service.AddTagToClientAsync(1, 99));
    }

    [Fact]
    public async Task RemoveTagFromClientAsync_RemovesTag()
    {
        // Arrange
        var client = await _context.Clients.Include(c => c.Tags).FirstAsync();
        client.Tags!.Add(TestSeedData.Tags()[0]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.RemoveTagFromClientAsync(1, 1);

        // Assert
        Assert.True(result);
        Assert.DoesNotContain(client.Tags, t => t.Id == 1);
    }

    [Fact]
    public async Task RemoveTagFromClientAsync_ReturnsFalse_WhenTagNotAssigned()
    {
        // Act
        var result = await _service.RemoveTagFromClientAsync(1, 1);

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Dispose();
    }

    private void SeedTestData()
    {
        _context.Clients.AddRange(TestSeedData.Clients());
        _context.Tags.AddRange(TestSeedData.Tags());
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }
}