using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Domain.Interfaces;

public interface IListeningRepository
{
    Task<ListeningExercise?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ListeningExercise>> GetByLevelAsync(CefrLevel level, CancellationToken cancellationToken = default);
    Task InsertManyAsync(IEnumerable<ListeningExercise> exercises, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}
