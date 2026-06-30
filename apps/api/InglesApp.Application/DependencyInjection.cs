using InglesApp.Application.Interfaces;
using InglesApp.Application.Services;
using InglesApp.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InglesApp.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<SpacedRepetitionService>();
        services.AddScoped<ExerciseGeneratorService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IVocabularyService, VocabularyService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<IGrammarService, GrammarService>();
        services.AddScoped<ILessonService, LessonService>();
        services.AddScoped<IExerciseService, ExerciseService>();
        services.AddScoped<IReadingService, ReadingService>();
        services.AddScoped<IListeningService, ListeningService>();
        services.AddScoped<ISpeakingService, SpeakingService>();
        services.AddScoped<IWritingService, WritingService>();
        services.AddScoped<IPlacementService, PlacementService>();
        services.AddScoped<IDailySessionService, DailySessionService>();
        services.AddScoped<IExamSimulatorService, ExamSimulatorService>();
        services.AddScoped<ISuggestionService, SuggestionService>();
        return services;
    }
}
