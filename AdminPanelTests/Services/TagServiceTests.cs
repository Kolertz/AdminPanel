using AdminPanel.Models;
using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;
using AdminPanel.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminPanel.Tests.Services;

public class TagServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TagService _service;
    private readonly List<Tag> _initialTags;

    public TagServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TagServiceTests_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        _service = new TagService(_context);

        // Initialize test data
        _initialTags =
        [
            new Tag { Id = 1, Name = "Tag1" },
            new Tag { Id = 2, Name = "Tag2" }
        ];

        // Seed initial data
        _context.Tags.AddRange(_initialTags);
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
        Assert.Equal(_initialTags.Count, result.Count);
        Assert.All(_initialTags, t =>
            Assert.Contains(result, r => r.Name == t.Name && r.Id == t.Id));
    }

    [Fact]
    public async Task CreateTagAsync_AddsNewTag()
    {
        // Arrange
        var newTag = new TagDto { Name = "NewTag" };
        var initialCount = _initialTags.Count;

        // Act
        var result = await _service.CreateTagAsync(newTag);

        // Assert
        Assert.Multiple(
            async () => Assert.Equal(initialCount + 1, await _context.Tags.CountAsync()),
            () => Assert.Equal(newTag.Name, result.Name),
            () => Assert.True(result.Id > 0)
        );
    }

    [Fact]
    public async Task DeleteTagAsync_RemovesTag_WhenExists()
    {
        // Arrange
        var tagIdToDelete = _initialTags[0].Id;
        var initialCount = _initialTags.Count;

        // Act
        var result = await _service.DeleteTagAsync(tagIdToDelete);

        // Assert
        Assert.Multiple(
            () => Assert.True(result),
            async () => Assert.Equal(initialCount - 1, await _context.Tags.CountAsync()),
            async() => Assert.Null(await _context.Tags.FindAsync(tagIdToDelete))
        );
    }

    [Fact]
    public async Task DeleteTagAsync_ReturnsFalse_WhenNotExists()
    {
        // Arrange
        var nonExistentId = 999;
        var initialCount = _initialTags.Count;

        // Act
        var result = await _service.DeleteTagAsync(nonExistentId);

        // Assert
        Assert.Multiple(
            () => Assert.False(result),
            async () => Assert.Equal(initialCount, await _context.Tags.CountAsync())
        );
    }

    [Fact]
    public async Task CreateTagAsync_Throws_WhenDuplicateName()
    {
        // Arrange
        var duplicateTag = new TagDto { Name = _initialTags[0].Name };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateTagAsync(duplicateTag));
    }
}