using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface IExerciseRepository
{
    Task<Exercise?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Exercise>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Exercise>> GetByLevelAndSkillAsync(CefrLevel level, Skill skill, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Exercise>> GetByGrammarRuleIdAsync(string grammarRuleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Exercise>> GetByLessonIdAsync(string lessonId, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<Exercise> exercises, CancellationToken cancellationToken = default);
    Task DeleteByGrammarRuleIdsAsync(IEnumerable<string> grammarRuleIds, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
