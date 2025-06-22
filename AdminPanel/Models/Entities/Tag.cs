using AdminPanel.Models.Dtos;

namespace AdminPanel.Models.Entities;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Client>? Clients { get; set; }
}
