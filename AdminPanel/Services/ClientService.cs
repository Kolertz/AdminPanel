using AdminPanel.Interfaces;
using AdminPanel.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class ClientService(AppDbContext db) : IClientService
{
    private readonly AppDbContext _db = db;

    public async Task<List<Client>> GetAllClientsAsync() =>
        await _db.Clients.AsNoTracking().ToListAsync();

    public async Task<Client?> GetClientByIdAsync(int id) =>
        await _db.Clients.FindAsync(id);

    public async Task<Client> CreateClientAsync(Client client)
    {
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return client;
    }

    public async Task<Client?> UpdateClientAsync(int id, Client inputClient)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null)
            return null;

        client.Name = inputClient.Name;
        client.Email = inputClient.Email;
        client.Balance = inputClient.Balance;

        await _db.SaveChangesAsync();
        return client;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null)
            return false;

        _db.Clients.Remove(client);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Tag>> GetClientTagsAsync(int id)
    {
        var client = await _db.Clients
            .AsNoTracking()
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == id);
        return client?.Tags ?? [];
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
