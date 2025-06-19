using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Tests.TestData;
using Microsoft.EntityFrameworkCore;
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
        var result = await _service.GetAllClientsAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetClientByIdAsync_ReturnsClient_WhenExists()
    {
        var result = await _service.GetClientByIdAsync(1);
        Assert.NotNull(result);
        Assert.Equal("Client A", result.Name);
    }

    [Fact]
    public async Task GetClientByIdAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _service.GetClientByIdAsync(99);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateClientAsync_AddsNewClient()
    {
        var newClient = new Client { Name = "New Client", Email = "new@test.com" };
        var result = await _service.CreateClientAsync(newClient);

        Assert.Equal(3, await _context.Clients.CountAsync());
        Assert.Equal("New Client", result.Name);
    }

    [Fact]
    public async Task UpdateClientAsync_UpdatesExistingClient()
    {
        var updatedClient = new Client { Name = "Updated", Email = "updated@test.com", Balance = 500 };
        var result = await _service.UpdateClientAsync(1, updatedClient);

        Assert.True(result);
        var client = await _context.Clients.FindAsync(1);
        Assert.NotNull(client);
        Assert.Equal("Updated", client.Name);
    }

    [Fact]
    public async Task UpdateClientAsync_ReturnsFalse_WhenClientNotFound()
    {
        var result = await _service.UpdateClientAsync(99, new Client());
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteClientAsync_RemovesClient()
    {
        var result = await _service.DeleteClientAsync(1);
        Assert.True(result);
        Assert.Null(await _context.Clients.FindAsync(1));
    }

    [Fact]
    public async Task DeleteClientAsync_ReturnsFalse_WhenClientNotFound()
    {
        var result = await _service.DeleteClientAsync(99);
        Assert.False(result);
    }

    [Fact]
    public async Task GetClientTagsAsync_ReturnsTags()
    {
        var client = await _context.Clients.Include(c => c.Tags).FirstAsync();
        client.Tags.Add(TestSeedData.Tags()[0]);
        await _context.SaveChangesAsync();

        var result = await _service.GetClientTagsAsync(1);
        Assert.Single(result);
    }

    [Fact]
    public async Task AddTagToClientAsync_AddsTag()
    {
        var result = await _service.AddTagToClientAsync(1, 1);
        Assert.True(result);

        var client = await _context.Clients.Include(c => c.Tags).FirstAsync();
        Assert.Contains(client.Tags, t => t.Id == 1);
    }

    [Fact]
    public async Task AddTagToClientAsync_ReturnsFalse_WhenClientOrTagNotFound()
    {
        Assert.False(await _service.AddTagToClientAsync(99, 1));
        Assert.False(await _service.AddTagToClientAsync(1, 99));
    }

    [Fact]
    public async Task RemoveTagFromClientAsync_RemovesTag()
    {
        var client = await _context.Clients.Include(c => c.Tags).FirstAsync();
        client.Tags.Add(TestSeedData.Tags()[0]);
        await _context.SaveChangesAsync();

        var result = await _service.RemoveTagFromClientAsync(1, 1);
        Assert.True(result);
        Assert.DoesNotContain(client.Tags, t => t.Id == 1);
    }

    [Fact]
    public async Task RemoveTagFromClientAsync_ReturnsFalse_WhenTagNotAssigned()
    {
        var result = await _service.RemoveTagFromClientAsync(1, 1);
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