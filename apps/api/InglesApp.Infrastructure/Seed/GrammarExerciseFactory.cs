using System.Text.Json;
using System.Text.RegularExpressions;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;

namespace InglesApp.Infrastructure.Seed;

internal static class GrammarExerciseFactory
{
    private static readonly Regex Marker = new(@"\{\{(.+?)\}\}", RegexOptions.Compiled);

    public static List<string> BuildRelatedStructures(GrammarTopicSeed topic)
    {
        var structures = topic.SubRules.Select(rule => $"Sub-regla: {rule}").ToList();
        structures.AddRange(topic.CommonErrors.Select(error => $"Error común: {error}"));
        structures.Add($"Producción ({topic.ProductionSkill}): {topic.ProductionPrompt}");
        return structures;
    }

    public static List<Exercise> BuildExercises(GrammarRule rule, GrammarTopicSeed topic)
    {
        var exercises = new List<Exercise>();
        var difficulty = topic.Level switch
        {
            CefrLevel.A1 => 1,
            CefrLevel.A2 => 2,
            _ => 3
        };

        for (var i = 0; i < 2 && i < topic.Examples.Length; i++)
        {
            var fill = BuildFillBlank(topic.Examples[i], topic.Distractors, i + StableHash(topic.Title));
            if (fill is not null)
                exercises.Add(Create(rule, topic, ExerciseType.FillBlank, fill, difficulty));
        }

        if (topic.Examples.Length >= 3)
        {
            var correct = Clean(topic.Examples[2]);
            var options = new List<string> { correct };
            options.AddRange(topic.WrongExamples.Take(3));
            options = options.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            Shuffle(options, new Random(StableHash(topic.Title) + 11));
            exercises.Add(Create(rule, topic, ExerciseType.MultipleChoice, new
            {
                question = "Elige la oración correcta.",
                options,
                correctIndex = options.FindIndex(o => o.Equals(correct, StringComparison.OrdinalIgnoreCase))
            }, difficulty));
        }

        if (topic.Examples.Length >= 4)
        {
            var sentence = Clean(topic.Examples[3]);
            var correctOrder = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (correctOrder.Length >= 3)
            {
                var words = correctOrder.ToList();
                Shuffle(words, new Random(StableHash(topic.Title) + 23));
                if (words.SequenceEqual(correctOrder))
                    (words[0], words[1]) = (words[1], words[0]);

                exercises.Add(Create(rule, topic, ExerciseType.WordOrder, new
                {
                    words,
                    correctOrder
                }, difficulty));
            }
        }

        if (topic.WrongExamples.Length >= 3)
        {
            var wrong = topic.WrongExamples[0];
            var correct = Clean(topic.Examples[0]);
            var options = new List<string> { correct };
            options.AddRange(topic.WrongExamples.Take(3));
            options = options.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            Shuffle(options, new Random(StableHash(topic.Title) + 37));
            exercises.Add(Create(rule, topic, ExerciseType.MultipleChoice, new
            {
                question = $"Encuentra la corrección. La frase \"{wrong}\" tiene un error.",
                options,
                correctIndex = options.FindIndex(o => o.Equals(correct, StringComparison.OrdinalIgnoreCase))
            }, difficulty));
        }

        return exercises;
    }

    private static object? BuildFillBlank(string example, string[] distractors, int seed)
    {
        var match = Marker.Match(example);
        if (!match.Success) return null;

        var answer = match.Groups[1].Value;
        var sentence = Marker.Replace(example, "___");

        var options = new List<string> { answer };
        foreach (var distractor in distractors)
        {
            if (options.Count >= 4) break;
            if (!options.Any(o => o.Equals(distractor, StringComparison.OrdinalIgnoreCase)))
                options.Add(distractor);
        }

        while (options.Count < 4)
            options.Add($"opción {options.Count + 1}");

        Shuffle(options, new Random(seed));
        return new
        {
            sentence,
            blank = answer,
            options,
            correctIndex = options.FindIndex(o => o.Equals(answer, StringComparison.OrdinalIgnoreCase))
        };
    }

    private static Exercise Create(GrammarRule rule, GrammarTopicSeed topic, ExerciseType type, object content, int difficulty) => new()
    {
        Type = type,
        ContentJson = JsonSerializer.Serialize(content),
        CefrLevel = rule.CefrLevel,
        Skill = Skill.Grammar,
        GrammarRuleId = rule.Id,
        Category = rule.Category,
        Difficulty = difficulty
    };

    public static string CleanExample(string value) => Marker.Replace(value, "$1");

    private static string Clean(string value) => CleanExample(value);

    private static void Shuffle<T>(IList<T> list, Random random)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static int StableHash(string value)
    {
        unchecked
        {
            var hash = 17;
            foreach (var c in value)
                hash = hash * 31 + c;
            return hash & int.MaxValue;
        }
    }
}
