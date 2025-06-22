using AdminPanel.Interfaces;
using AdminPanel.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class TagService(AppDbContext db) : ITagService
{
    private readonly AppDbContext _db = db;

    public async Task<List<Tag>> GetAllTagsAsync() =>
        await _db.Tags.AsNoTracking().ToListAsync();

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();
        return tag;
    }

    public async Task<bool> DeleteTagAsync(int id)
    {
        var tag = await _db.Tags.FindAsync(id);
        if (tag == null)
            return false;

        _db.Tags.Remove(tag);
        await _db.SaveChangesAsync();
        return true;
    }
}
