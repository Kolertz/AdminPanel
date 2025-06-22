using AdminPanel.Interfaces;
using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class ClientService(AppDbContext db) : IClientService
{
    private readonly AppDbContext _db = db;

    public async Task<List<ClientDto>> GetAllClientsAsync() =>
        (await _db.Clients.AsNoTracking().ToListAsync())
        .Adapt<List<ClientDto>>();

    public async Task<ClientDto?> GetClientByIdAsync(int id) =>
        (await _db.Clients.FindAsync(id))
        .Adapt<ClientDto>();

    public async Task<ClientDto> CreateClientAsync(ClientDto client)
    {
        var newClient = client.Adapt<Client>();

        _db.Clients.Add(newClient);

        await _db.SaveChangesAsync();
        return newClient.Adapt<ClientDto>();
    }

    public async Task<ClientDto?> UpdateClientAsync(int id, ClientDto inputClient)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null)
            return null;

        client.Name = inputClient.Name;
        client.Email = inputClient.Email;
        client.Balance = inputClient.Balance;

        await _db.SaveChangesAsync();
        return client.Adapt<ClientDto>();
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _db.Clients
        .Include(c => c.Tags)
        .FirstOrDefaultAsync(c => c.Id == id);

        if (client == null)
            return false;

        client.Tags?.Clear();

        _db.Clients.Remove(client);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<TagDto>> GetClientTagsAsync(int id)
    {
        var client = await _db.Clients
            .AsNoTracking()
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == id);

        var clientTags = client?.Tags?.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList();
        return clientTags.Adapt<List<TagDto>>();
    }

    public async Task<bool> AddTagToClientAsync(int clientId, int tagId)
    {
        var client = await _db.Clients.Include(c => c.Tags).FirstOrDefaultAsync(c => c.Id == clientId);
        var tag = await _db.Tags.FindAsync(tagId);
        if (client == null || tag == null)
            return false;

        client.Tags!.Add(tag);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveTagFromClientAsync(int clientId, int tagId)
    {
        var client = await _db.Clients.Include(c => c.Tags).FirstOrDefaultAsync(c => c.Id == clientId);
        var tag = client?.Tags?.FirstOrDefault(t => t.Id == tagId);
        if (tag == null)
            return false;

        client!.Tags!.Remove(tag);
        await _db.SaveChangesAsync();
        return true;
    }
}
