using AdminPanel.Models.Entities;

namespace AdminPanel.Tests.TestData;

public static class TestSeedData
{
    public static List<Tag> Tags() =>
    [
        new() { Id = 1, Name = "Tag1" },
        new() { Id = 2, Name = "Tag2" }
    ];
}