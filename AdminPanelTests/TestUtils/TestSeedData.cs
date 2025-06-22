using AdminPanel.Models.Entities;
using System.Collections.Generic;

namespace AdminPanel.Tests.TestData;

public static class TestSeedData
{
    public static List<Tag> Tags() =>
    [
        new() { Id = 1, Name = "Tag1" },
        new() { Id = 2, Name = "Tag2" }
    ];
}