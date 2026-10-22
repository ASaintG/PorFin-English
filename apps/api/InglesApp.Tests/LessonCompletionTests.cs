using InglesApp.Application.Services;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Tests;

public class LessonCompletionTests
{
    [Fact]
    public async Task GetByLevelAsync_MarksOnlyCompletedLessons()
    {
        var service = CreateService(
            new User { Id = "user1", CompletedLessonIds = ["1"] },
            BuildLessons());

        var result = await service.GetByLevelAsync("user1", CefrLevel.A1);

        Assert.True(result.Single(l => l.Id == "1").IsCompleted);
        Assert.False(result.Single(l => l.Id == "2").IsCompleted);
    }

    [Fact]
    public async Task GetByLevelAsync_DoesNotLockSequentialLessons()
    {
        var service = CreateService(new User { Id = "user1" }, BuildLessons());

        var result = await service.GetByLevelAsync("user1", CefrLevel.A1);

        Assert.All(result, lesson => Assert.False(lesson.IsCompleted));
    }

    private static LessonService CreateService(User user, List<Lesson> lessons) =>
        new(
            new FakeLessonRepository(lessons),
            new FakeExerciseRepository(),
            new FakeGrammarRepository(),
            new FakeUserRepository(user));

    private static List<Lesson> BuildLessons() =>
    [
        new() { Id = "1", Title = "First", CefrLevel = CefrLevel.A1, Sequence = 1 },
        new() { Id = "2", Title = "Second", CefrLevel = CefrLevel.A1, Sequence = 2 },
    ];

    private sealed class FakeUserRepository(User user) : IUserRepository
    {
        public Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult<User?>(user.Id == id ? user : null);

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult<User?>(user.Email == email ? user : null);

        public Task<User> CreateAsync(User newUser, CancellationToken cancellationToken = default) =>
            Task.FromResult(newUser);

        public Task UpdateAsync(User updated, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class FakeLessonRepository(List<Lesson> lessons) : ILessonRepository
    {
        public Task<Lesson?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult(lessons.FirstOrDefault(l => l.Id == id));

        public Task<IReadOnlyList<Lesson>> GetByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Lesson>>(lessons.Where(l => l.CefrLevel == level).ToList());

        public Task<IReadOnlyList<Lesson>> GetAllSortedAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Lesson>>(lessons.OrderBy(l => l.Sequence).ToList());

        public Task InsertManyAsync(IEnumerable<Lesson> newLessons, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteAllAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(lessons.Count > 0);
    }

    private sealed class FakeExerciseRepository : IExerciseRepository
    {
        public Task<Exercise?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Exercise?>(null);

        public Task<IReadOnlyList<Exercise>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Exercise>>([]);

        public Task<IReadOnlyList<Exercise>> GetByLevelAndSkillAsync(CefrLevel level, Skill skill, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Exercise>>([]);

        public Task<IReadOnlyList<Exercise>> GetByGrammarRuleIdAsync(string grammarRuleId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Exercise>>([]);

        public Task<IReadOnlyList<Exercise>> GetByLessonIdAsync(string lessonId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Exercise>>([]);

        public Task InsertManyAsync(IEnumerable<Exercise> exercises, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteByGrammarRuleIdsAsync(IEnumerable<string> grammarRuleIds, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private sealed class FakeGrammarRepository : IGrammarRepository
    {
        public Task<GrammarRule?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
            Task.FromResult<GrammarRule?>(null);

        public Task<IReadOnlyList<GrammarRule>> GetByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<GrammarRule>>([]);

        public Task<IReadOnlyList<GrammarRule>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<GrammarRule>>([]);

        public Task InsertManyAsync(IEnumerable<GrammarRule> rules, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task UpdateAsync(GrammarRule rule, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task DeleteByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;

        public Task<bool> AnyAsync(CancellationToken cancellationToken = default) => Task.FromResult(false);
    }
}
