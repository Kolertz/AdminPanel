using AdminPanel.Models;

namespace AdminPanel.Interfaces;

public interface ITagService
{
    Task<List<Tag>> GetAllTagsAsync();
    Task<Tag> CreateTagAsync(Tag tag);
    Task<bool> DeleteTagAsync(int id);
}
