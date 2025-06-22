using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;

namespace AdminPanel.Interfaces;

public interface ITagService
{
    Task<List<TagDto>> GetAllTagsAsync();
    Task<TagDto> CreateTagAsync(TagDto tag);
    Task<bool> DeleteTagAsync(int id);
}
