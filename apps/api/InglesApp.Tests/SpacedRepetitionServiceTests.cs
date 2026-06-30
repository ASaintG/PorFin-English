using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Services;

namespace InglesApp.Tests;

public class SpacedRepetitionServiceTests
{
    private readonly SpacedRepetitionService _srs = new();

    [Fact]
    public void ApplyReview_WithHighQuality_IncreasesInterval()
    {
        var progress = _srs.CreateInitialProgress("user1", "item1");
        var reviewed = _srs.ApplyReview(progress, 5);

        Assert.Equal(1, reviewed.Repetitions);
        Assert.Equal(1, reviewed.Interval);
        Assert.True(reviewed.NextReviewDate > DateTime.UtcNow);
    }

    [Fact]
    public void ApplyReview_WithLowQuality_ResetsRepetitions()
    {
        var progress = _srs.CreateInitialProgress("user1", "item1");
        progress.Repetitions = 3;
        progress.Interval = 10;

        var reviewed = _srs.ApplyReview(progress, 2);

        Assert.Equal(0, reviewed.Repetitions);
        Assert.Equal(1, reviewed.Interval);
    }

    [Fact]
    public void CreateInitialProgress_SetsGrammarItemType()
    {
        var progress = _srs.CreateInitialProgress("user1", "ex1", ItemType.Grammar);
        Assert.Equal(ItemType.Grammar, progress.ItemType);
    }
}
