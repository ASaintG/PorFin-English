using InglesApp.Application.Services;
using InglesApp.Domain.Entities;

namespace InglesApp.Tests;

public class LessonUnlockTests
{
    [Fact]
    public void FirstLesson_IsNeverLocked()
    {
        var lessons = new List<Lesson>
        {
            new() { Id = "1", Sequence = 1 },
            new() { Id = "2", Sequence = 2 },
        };

        var locked = LessonService.IsLessonLocked(lessons[0], lessons, []);
        Assert.False(locked);
    }

    [Fact]
    public void SecondLesson_LockedUntilFirstCompleted()
    {
        var lessons = new List<Lesson>
        {
            new() { Id = "1", Sequence = 1 },
            new() { Id = "2", Sequence = 2 },
        };

        Assert.True(LessonService.IsLessonLocked(lessons[1], lessons, []));
        Assert.False(LessonService.IsLessonLocked(lessons[1], lessons, ["1"]));
    }
}
