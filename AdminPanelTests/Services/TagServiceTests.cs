using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPanel.Tests.Services;

public class TagServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TagService _service;

    public TagServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TagServiceTests_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _service = new TagService(_context);

        // Seed initial data
        _context.Tags.AddRange(TestSeedData.Tags());
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllTagsAsync_ReturnsAllTags()
    {
        // Act
        var result = await _service.GetAllTagsAsync();

        // Assert
        Assert.Equal(TestSeedData.Tags().Count, result.Count);
        Assert.Contains(result, t => t.Name == "Tag1");
        Assert.Contains(result, t => t.Name == "Tag2");
    }

    [Fact]
    public async Task CreateTagAsync_AddsNewTag()
    {
        // Arrange
        var newTag = new Tag { Name = "NewTag" };

        // Act
        var result = await _service.CreateTagAsync(newTag);

        // Assert
        Assert.Equal(TestSeedData.Tags().Count + 1, await _context.Tags.CountAsync());
        Assert.Equal(newTag.Name, result.Name);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task DeleteTagAsync_RemovesTag_WhenExists()
    {
        // Arrange
        var tagIdToDelete = TestSeedData.Tags()[0].Id;

        // Act
        var result = await _service.DeleteTagAsync(tagIdToDelete);

        // Assert
        Assert.True(result);
        Assert.Equal(TestSeedData.Tags().Count - 1, await _context.Tags.CountAsync());
        Assert.Null(await _context.Tags.FindAsync(tagIdToDelete));
    }

    [Fact]
    public async Task DeleteTagAsync_ReturnsFalse_WhenNotExists()
    {
        // Arrange
        var nonExistentId = 999;

        // Act
        var result = await _service.DeleteTagAsync(nonExistentId);

        // Assert
        Assert.False(result);
        Assert.Equal(TestSeedData.Tags().Count, await _context.Tags.CountAsync());
    }

    [Fact]
    public async Task CreateTagAsync_Throws_WhenDuplicateName()
    {
        // Arrange
        var duplicateTag = new Tag { Name = "Tag1" }; // Дублируем существующее имя

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateTagAsync(duplicateTag));
    }
}