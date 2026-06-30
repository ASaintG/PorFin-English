using System.Text.Json;
using InglesApp.Application.DTOs.Exercises;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;

namespace InglesApp.Application.Services;

public class ExerciseGeneratorService
{
    private static readonly string[] DistractorPool =
        ["is", "are", "was", "were", "have", "has", "do", "does", "go", "went", "the", "a", "an"];

    public List<Exercise> GenerateFromGrammarRule(GrammarRule rule, string? lessonId = null)
    {
        var exercises = new List<Exercise>();
        foreach (var example in rule.Examples.Take(2))
        {
            var words = example.TrimEnd('.').Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 3) continue;

            var blankIndex = Math.Min(2, words.Length - 1);
            var answer = words[blankIndex].TrimEnd('.', ',', '!', '?');
            var sentence = string.Join(" ", words.Select((w, i) => i == blankIndex ? "___" : w));

            var options = new List<string> { answer };
            options.AddRange(DistractorPool.Where(d => d != answer.ToLower()).Take(3));
            options = options.Distinct(StringComparer.OrdinalIgnoreCase).Take(4).ToList();
            while (options.Count < 4)
                options.Add($"opt{options.Count}");

            var content = new
            {
                sentence = sentence + ".",
                blank = answer,
                options,
                correctIndex = 0
            };

            exercises.Add(new Exercise
            {
                Type = ExerciseType.FillBlank,
                ContentJson = JsonSerializer.Serialize(content),
                CefrLevel = rule.CefrLevel,
                Skill = Skill.Grammar,
                GrammarRuleId = rule.Id,
                LessonId = lessonId,
                Category = rule.Category,
                Difficulty = 1
            });
        }

        return exercises;
    }

    public static object ParseContent(string contentJson) =>
        JsonSerializer.Deserialize<object>(contentJson) ?? new { };
}
