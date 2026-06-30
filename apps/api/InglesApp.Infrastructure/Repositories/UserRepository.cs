using InglesApp.Domain.Entities;
using InglesApp.Infrastructure.Data;
using InglesApp.Domain.Interfaces;
using MongoDB.Driver;

namespace InglesApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository(MongoDbContext context)
    {
        _collection = context.Users;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(u => u.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(u => u.Email == email).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(user.Id))
            user.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

        await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);
        return user;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _collection.ReplaceOneAsync(u => u.Id == user.Id, user, cancellationToken: cancellationToken);
    }
}
