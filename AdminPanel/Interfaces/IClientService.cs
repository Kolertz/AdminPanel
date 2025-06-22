using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;

namespace AdminPanel.Interfaces;

public interface IClientService
{
    Task<List<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int id);
    Task<Client> CreateClientAsync(Client client);
    Task<Client?> UpdateClientAsync(int id, Client inputClient);
    Task<bool> DeleteClientAsync(int id);
    Task<List<TagDto>> GetClientTagsAsync(int id);
    Task<bool> AddTagToClientAsync(int clientId, int tagId);
    Task<bool> RemoveTagFromClientAsync(int clientId, int tagId);
}
