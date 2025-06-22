using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;
using AdminPanel.Services;
using Microsoft.EntityFrameworkCore;

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
        var newClient = new ClientDto { Name = "New Client", Email = "new@test.com" };

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
        var updatedClient = new ClientDto { Id = 1, Name = "Updated", Email = "updated@test.com", Balance = 500 };

        // Act
        var result = await _service.UpdateClientAsync(updatedClient.Id, updatedClient);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedClient.Name, result.Name);
        Assert.Equal(updatedClient.Email, result.Email);
        Assert.Equal(updatedClient.Balance, result.Balance);
    }

    [Fact]
    public async Task UpdateClientAsync_ReturnsFalse_WhenClientNotFound()
    {
        // Act
        var result = await _service.UpdateClientAsync(99, new ClientDto());

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
        var tag = new Tag { Id = 1, Name = "VIP" };
        client.Tags!.Add(tag);
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
        // Act
        var result1 = await _service.AddTagToClientAsync(99, 1);
        var result2 = await _service.AddTagToClientAsync(1, 99);

        // Assert
        Assert.False(result1);
        Assert.False(result2);
    }

    [Fact]
    public async Task RemoveTagFromClientAsync_RemovesTag()
    {
        // Arrange
        var client = await _context.Clients.Include(c => c.Tags).FirstAsync();
        var tag = new Tag { Id = 1, Name = "VIP" };
        client.Tags!.Add(tag);
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
        // Создание тестовых клиентов
        var clients = new List<Client>
        {
            new() { Id = 1, Name = "Client A", Email = "clientA@test.com", Balance = 1000 },
            new() { Id = 2, Name = "Client B", Email = "clientB@test.com", Balance = 2000 }
        };

        // Создание тестовых меток
        var tags = new List<Tag>
        {
            new() { Id = 1, Name = "VIP" },
            new() { Id = 2, Name = "Regular" }
        };

        _context.Clients.AddRange(clients);
        _context.Tags.AddRange(tags);
        _context.SaveChanges();
        _context.ChangeTracker.Clear();
    }
}