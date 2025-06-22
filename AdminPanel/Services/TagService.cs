using AdminPanel.Interfaces;
using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class TagService(AppDbContext db) : ITagService
{
    private readonly AppDbContext _db = db;

    public async Task<List<TagDto>> GetAllTagsAsync() =>
        (await _db.Tags.AsNoTracking().ToListAsync())
        .Adapt<List<TagDto>>();

    public async Task<TagDto> CreateTagAsync(TagDto tag)
    {
        var newTag = tag.Adapt<Tag>();

        _db.Tags.Add(newTag);

        await _db.SaveChangesAsync();
        return newTag.Adapt<TagDto>();
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
