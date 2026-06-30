using InglesApp.Domain.Entities;

namespace InglesApp.Domain.Interfaces;

public interface IUserSubmissionRepository
{
    Task<UserSubmission> CreateAsync(UserSubmission submission, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserSubmission>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
}
