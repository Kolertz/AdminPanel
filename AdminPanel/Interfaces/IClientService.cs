using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;

namespace AdminPanel.Interfaces;

public interface IClientService
{
    Task<List<ClientDto>> GetAllClientsAsync();
    Task<ClientDto?> GetClientByIdAsync(int id);
    Task<ClientDto> CreateClientAsync(ClientDto client);
    Task<ClientDto?> UpdateClientAsync(int id, ClientDto inputClient);
    Task<bool> DeleteClientAsync(int id);
    Task<List<TagDto>> GetClientTagsAsync(int id);
    Task<bool> AddTagToClientAsync(int clientId, int tagId);
    Task<bool> RemoveTagFromClientAsync(int clientId, int tagId);
}
