using InglesApp.Application.Interfaces;
using InglesApp.Application.Services;
using InglesApp.Domain.Interfaces;
using InglesApp.Infrastructure.Auth;
using InglesApp.Infrastructure.Data;
using InglesApp.Infrastructure.Repositories;
using InglesApp.Infrastructure.Seed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InglesApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<MongoDbContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVocabularyRepository, VocabularyRepository>();
        services.AddScoped<IUserProgressRepository, UserProgressRepository>();
        services.AddScoped<IGrammarRepository, GrammarRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IReadingRepository, ReadingRepository>();
        services.AddScoped<IListeningRepository, ListeningRepository>();
        services.AddScoped<ISpeakingRepository, SpeakingRepository>();
        services.AddScoped<IWritingRepository, WritingRepository>();
        services.AddScoped<IUserSubmissionRepository, UserSubmissionRepository>();
        services.AddScoped<IContentSuggestionRepository, ContentSuggestionRepository>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<VocabularySeeder>();
        services.AddScoped<ContentSeeder>();

        return services;
    }
}
