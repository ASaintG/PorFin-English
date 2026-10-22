using InglesApp.Application.Interfaces;
using InglesApp.Application.Services;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using MongoDB.Bson;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace InglesApp.Infrastructure.Seed;

public class ContentSeeder
{
    private readonly IGrammarRepository _grammarRepository;
    private readonly IExerciseRepository _exerciseRepository;
    private readonly ILessonRepository _lessonRepository;
    private readonly IVocabularyRepository _vocabularyRepository;
    private readonly IReadingRepository _readingRepository;
    private readonly IListeningRepository _listeningRepository;
    private readonly ISpeakingRepository _speakingRepository;
    private readonly IWritingRepository _writingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ContentSeeder(
        IGrammarRepository grammarRepository,
        IExerciseRepository exerciseRepository,
        ILessonRepository lessonRepository,
        IVocabularyRepository vocabularyRepository,
        IReadingRepository readingRepository,
        IListeningRepository listeningRepository,
        ISpeakingRepository speakingRepository,
        IWritingRepository writingRepository,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _grammarRepository = grammarRepository;
        _exerciseRepository = exerciseRepository;
        _lessonRepository = lessonRepository;
        _vocabularyRepository = vocabularyRepository;
        _readingRepository = readingRepository;
        _listeningRepository = listeningRepository;
        _speakingRepository = speakingRepository;
        _writingRepository = writingRepository;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAllAsync(CancellationToken ct = default)
    {
        await SeedDemoUserAsync(ct);
        await SeedA2VocabularyAsync(ct);
        await SeedLevelVocabularyAsync(CefrLevel.B1, "InglesApp.Infrastructure.Seed.b1-vocabulary.json", ct);
        await SeedLevelVocabularyAsync(CefrLevel.B2, "InglesApp.Infrastructure.Seed.b2-vocabulary.json", ct);
        await SeedThematicVocabularyAsync(ct);
        await SeedGrammarAndLessonsAsync(ct);
        await SeedReadingAsync(ct);
        if (!await _listeningRepository.AnyAsync(ct))
            await SeedListeningAsync(ct);
        await SeedSpeakingAsync(ct);
        await SeedWritingAsync(ct);
    }

    private async Task SeedDemoUserAsync(CancellationToken ct)
    {
        const string email = "demo@inglesapp.dev";
        if (await _userRepository.GetByEmailAsync(email, ct) is not null) return;
        await _userRepository.CreateAsync(new User
        {
            Email = email,
            PasswordHash = _passwordHasher.Hash("demo123"),
            CurrentSubLevel = "A1.1",
            PlacementCompleted = true,
            LastActive = DateTime.UtcNow
        }, ct);
    }

    private async Task SeedLevelVocabularyAsync(CefrLevel level, string resource, CancellationToken ct)
    {
        var items = LoadJson<VocabSeedItem>(resource);
        await SeedVocabularyAsync(level, items, ct);
    }

    private async Task SeedA2VocabularyAsync(CancellationToken ct)
    {
        var items = LoadJson<VocabSeedItem>("InglesApp.Infrastructure.Seed.a2-vocabulary.json");
        if (items.Count == 0) items = GetInlineA2Vocabulary();
        await SeedVocabularyAsync(CefrLevel.A2, items, ct);
    }

    private async Task SeedVocabularyAsync(CefrLevel level, IReadOnlyList<VocabSeedItem> items, CancellationToken ct)
    {
        if (items.Count == 0) return;

        var existing = await _vocabularyRepository.GetByLevelAsync(level, 0, 100000, ct);
        var existingByWord = new Dictionary<string, VocabularyItem>(StringComparer.OrdinalIgnoreCase);
        var toDelete = new List<string>();
        foreach (var item in existing)
        {
            if (IsSyntheticWord(item.Word, item.Category))
            {
                toDelete.Add(item.Id);
                continue;
            }
            existingByWord[item.Word] = item;
        }

        if (toDelete.Count > 0)
            await _vocabularyRepository.DeleteManyAsync(toDelete, ct);

        var toInsert = new List<VocabularyItem>();
        var toUpdate = new List<VocabularyItem>();
        foreach (var seed in items)
        {
            if (IsSyntheticWord(seed.Word, seed.Category)) continue;

            if (existingByWord.TryGetValue(seed.Word, out var current))
            {
                var changed = current.Translation != seed.Translation
                    || current.PartOfSpeech != seed.PartOfSpeech
                    || current.Category != seed.Category
                    || !current.ExampleSentences.SequenceEqual(seed.ExampleSentences, StringComparer.Ordinal);
                if (!changed) continue;

                current.Translation = seed.Translation;
                current.PartOfSpeech = seed.PartOfSpeech;
                current.Category = seed.Category;
                current.ExampleSentences = seed.ExampleSentences;
                toUpdate.Add(current);
            }
            else
            {
                toInsert.Add(new VocabularyItem
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Word = seed.Word,
                    Translation = seed.Translation,
                    PartOfSpeech = seed.PartOfSpeech,
                    CefrLevel = level,
                    Category = seed.Category,
                    ExampleSentences = seed.ExampleSentences
                });
            }
        }

        if (toInsert.Count > 0)
            await _vocabularyRepository.InsertManyAsync(toInsert, ct);
        if (toUpdate.Count > 0)
            await _vocabularyRepository.UpdateManyAsync(toUpdate, ct);
    }

    private static bool IsSyntheticWord(string? word, string? category)
    {
        if (string.IsNullOrWhiteSpace(word)) return true;
        if (string.Equals(category, "generated", StringComparison.OrdinalIgnoreCase)) return true;
        return Regex.IsMatch(word.Trim(), @"\s\d+$");
    }

    private async Task SeedThematicVocabularyAsync(CancellationToken ct)
    {
        foreach (var pack in GetVocabularyPacks())
        {
            var existing = await _vocabularyRepository.GetByLevelAndCategoryAsync(
                pack.Level, pack.Category, 0, 10000, ct);
            var existingWords = existing.Select(item => item.Word).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var missing = pack.Items.Where(item => !existingWords.Contains(item.Word)).ToList();
            if (missing.Count == 0) continue;

            var vocab = missing.Select(s => new VocabularyItem
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Word = s.Word,
                Translation = s.Translation,
                PartOfSpeech = s.PartOfSpeech,
                CefrLevel = pack.Level,
                Category = pack.Category,
                ExampleSentences = s.ExampleSentences
            }).ToList();

            await _vocabularyRepository.InsertManyAsync(vocab, ct);
        }
    }

    private async Task SeedGrammarAndLessonsAsync(CancellationToken ct)
    {
        var existingRules = await _grammarRepository.GetAllAsync(ct);
        var byKey = existingRules.ToDictionary(
            r => $"{r.CefrLevel}|{r.Title}",
            StringComparer.OrdinalIgnoreCase);

        var desiredKeys = GrammarSeedData.Topics
            .Select(topic => $"{topic.Level}|{topic.Title}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var obsolete = existingRules
            .Where(rule => rule.CefrLevel is CefrLevel.A1 or CefrLevel.A2 or CefrLevel.B1)
            .Where(rule => !desiredKeys.Contains($"{rule.CefrLevel}|{rule.Title}"))
            .ToList();

        if (obsolete.Count > 0)
        {
            await _exerciseRepository.DeleteByGrammarRuleIdsAsync(obsolete.Select(rule => rule.Id), ct);
            await _grammarRepository.DeleteByIdsAsync(obsolete.Select(rule => rule.Id), ct);
        }

        var newRules = new List<GrammarRule>();
        var orderedRules = new List<GrammarRule>();
        var changedRuleIds = new HashSet<string>();
        var updatedCount = 0;
        var exercisesChanged = false;

        foreach (var topic in GrammarSeedData.Topics)
        {
            var key = $"{topic.Level}|{topic.Title}";
            var examples = topic.Examples.Select(GrammarExerciseFactory.CleanExample).ToList();
            var related = GrammarExerciseFactory.BuildRelatedStructures(topic);

            if (byKey.TryGetValue(key, out var rule))
            {
                if (rule.Explanation != topic.Explanation
                    || rule.Category != topic.Category
                    || !rule.Examples.SequenceEqual(examples)
                    || !rule.RelatedStructures.SequenceEqual(related))
                {
                    rule.Explanation = topic.Explanation;
                    rule.Category = topic.Category;
                    rule.Examples = examples;
                    rule.RelatedStructures = related;
                    await _grammarRepository.UpdateAsync(rule, ct);
                    changedRuleIds.Add(rule.Id);
                    updatedCount++;
                }
            }
            else
            {
                rule = new GrammarRule
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Title = topic.Title,
                    Explanation = topic.Explanation,
                    CefrLevel = topic.Level,
                    Category = topic.Category,
                    Examples = examples,
                    RelatedStructures = related
                };
                newRules.Add(rule);
                byKey[key] = rule;
            }

            orderedRules.Add(rule);
        }

        if (newRules.Count > 0)
            await _grammarRepository.InsertManyAsync(newRules, ct);

        var allExercises = new List<Exercise>();
        for (var i = 0; i < GrammarSeedData.Topics.Length; i++)
        {
            var topic = GrammarSeedData.Topics[i];
            var rule = orderedRules[i];
            var existingExercises = await _exerciseRepository.GetByGrammarRuleIdAsync(rule.Id, ct);
            var needsRegeneration = existingExercises.Count < 5
                || existingExercises.All(e => e.Type == ExerciseType.FillBlank)
                || changedRuleIds.Contains(rule.Id);

            if (existingExercises.Count > 0 && needsRegeneration)
            {
                await _exerciseRepository.DeleteByGrammarRuleIdsAsync([rule.Id], ct);
                existingExercises = [];
                exercisesChanged = true;
            }

            if (existingExercises.Count > 0)
            {
                allExercises.AddRange(existingExercises);
                continue;
            }

            var generated = GrammarExerciseFactory.BuildExercises(rule, topic);
            foreach (var exercise in generated)
                exercise.Id = ObjectId.GenerateNewId().ToString();

            await _exerciseRepository.InsertManyAsync(generated, ct);
            allExercises.AddRange(generated);
            exercisesChanged = true;
        }

        if (newRules.Count > 0 || obsolete.Count > 0 || updatedCount > 0 || exercisesChanged)
        {
            await _lessonRepository.DeleteAllAsync(ct);
            var lessons = BuildLessons(orderedRules, allExercises);
            await _lessonRepository.InsertManyAsync(lessons, ct);
        }
        else if (!await _lessonRepository.AnyAsync(ct))
        {
            var lessons = BuildLessons(orderedRules, allExercises);
            await _lessonRepository.InsertManyAsync(lessons, ct);
        }
    }

    private async Task SeedReadingAsync(CancellationToken ct)
    {
        var existingTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var level in new[] { CefrLevel.A1, CefrLevel.A2, CefrLevel.B1 })
        {
            var existing = await _readingRepository.GetByLevelAsync(level, 0, 1000, ct);
            foreach (var text in existing)
                existingTitles.Add(text.Title);
        }

        var missing = GetReadingTexts().Where(t => !existingTitles.Contains(t.Title)).ToList();
        await _readingRepository.InsertManyAsync(missing, ct);
    }

    private async Task SeedListeningAsync(CancellationToken ct)
    {
        var items = new List<ListeningExercise>
        {
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = "Ordering at a Café",
                Transcript = "Waiter: Good morning. What would you like? Customer: A coffee and a sandwich, please. Waiter: Anything else? Customer: No, thank you. How much is it? Waiter: That is five euros.",
                CefrLevel = CefrLevel.A1,
                Category = "restaurant",
                Questions = Questions(
                    ("What does the customer order?", ["Tea and cake", "Coffee and sandwich", "Water", "Pizza"], 1))
            },
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = "Asking for Directions",
                Transcript = "Excuse me, where is the train station? Go straight for two blocks, then turn left. The station is next to the bank. Thank you very much. You are welcome.",
                CefrLevel = CefrLevel.A2,
                Category = "directions",
                Questions = Questions(
                    ("What is next to the station?", ["A park", "A bank", "A school", "A hotel"], 1))
            }
        };
        await _listeningRepository.InsertManyAsync(items, ct);
    }

    private async Task SeedSpeakingAsync(CancellationToken ct)
    {
        var existing = await GetAllSpeakingPromptsAsync(ct);
        var prompts = new List<SpeakingPrompt>();
        var seen = existing.Select(p => p.Prompt).ToHashSet(StringComparer.OrdinalIgnoreCase);

        void Add(SpeakingPrompt prompt)
        {
            if (seen.Add(prompt.Prompt))
                prompts.Add(prompt);
        }

        Add(Sp("Introduce yourself", "Tell me about yourself. Include your name, where you live, and what you do.", CefrLevel.A1, "greetings",
            "My name is Ana. I live in Madrid. I am a student. I like music and reading.",
            ["my name is", "I live in", "I am a", "I like"]));
        Add(Sp("Order food", "You are at a restaurant. Order a meal and a drink politely.", CefrLevel.A1, "restaurant",
            "I would like the chicken with rice, please. Can I have a glass of water? Thank you.",
            ["I would like", "please", "thank you"]));
        Add(Sp("Tell me about yourself", "Describe your background, skills, and why you are looking for a job.", CefrLevel.B1, "job_interview",
            "I am a software developer with three years of experience. I enjoy solving problems and working in teams. I am looking for new challenges in an international company.",
            ["experience", "I enjoy", "I am looking for", "team"]));
        Add(Sp("Describe your experience", "Talk about a project you are proud of and what you learned.", CefrLevel.B1, "job_interview",
            "I built a web application for language learning. I learned about databases and user experience. The project helped me improve my English.",
            ["I built", "I learned", "project", "improve"]));
        Add(Sp("Why this company", "Explain why you want to work at this company.", CefrLevel.B1, "job_interview",
            "I admire your company's mission. My skills match the role. I want to grow professionally here.",
            ["I admire", "my skills", "grow professionally"]));

        foreach (var topic in GrammarSeedData.Topics.Where(t => t.ProductionSkill == Skill.Speaking))
        {
            Add(new SpeakingPrompt
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Prompt = topic.ProductionPrompt,
                CefrLevel = topic.Level,
                Category = topic.Category,
                ModelAnswer = topic.ProductionModel,
                KeyPhrases = topic.ProductionKeyPhrases.ToList()
            });
        }

        await _speakingRepository.InsertManyAsync(prompts, ct);
    }

    private async Task SeedWritingAsync(CancellationToken ct)
    {
        var existing = await GetAllWritingPromptsAsync(ct);
        var prompts = new List<WritingPrompt>();
        var seen = existing.Select(p => p.Prompt).ToHashSet(StringComparer.OrdinalIgnoreCase);

        void Add(WritingPrompt prompt)
        {
            if (seen.Add(prompt.Prompt))
                prompts.Add(prompt);
        }

        Add(Wp("Describe your daily routine", "Write about what you do on a typical day.", CefrLevel.A1, "common",
            "I wake up at seven. I have breakfast and go to work. In the evening I study English.", 50));
        Add(Wp("A letter to a friend", "Write a short email to a friend about your weekend.", CefrLevel.A2, "common",
            "Hi! How are you? I had a great weekend. I visited my family and watched a film. Write soon!", 60));
        Add(Wp("Cover letter paragraph", "Write a paragraph for a job application explaining your interest.", CefrLevel.B1, "job_interview",
            "I am writing to apply for the developer position. I have experience with web technologies and I am eager to contribute to your team.", 80));
        Add(Wp("Professional email", "Write a polite email requesting a meeting with your manager.", CefrLevel.B1, "job_interview",
            "Dear Manager, I would like to schedule a meeting to discuss my progress. I am available next week. Best regards.", 70));

        foreach (var topic in GrammarSeedData.Topics.Where(t => t.ProductionSkill == Skill.Writing))
        {
            Add(new WritingPrompt
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Prompt = topic.ProductionPrompt,
                CefrLevel = topic.Level,
                Category = topic.Category,
                ModelAnswer = topic.ProductionModel,
                WordLimit = topic.ProductionWordLimit
            });
        }

        await _writingRepository.InsertManyAsync(prompts, ct);
    }

    private async Task<List<SpeakingPrompt>> GetAllSpeakingPromptsAsync(CancellationToken ct)
    {
        var all = new List<SpeakingPrompt>();
        foreach (var level in Enum.GetValues<CefrLevel>())
            all.AddRange(await _speakingRepository.GetByLevelAndCategoryAsync(level, null, ct));
        return all;
    }

    private async Task<List<WritingPrompt>> GetAllWritingPromptsAsync(CancellationToken ct)
    {
        var all = new List<WritingPrompt>();
        foreach (var level in Enum.GetValues<CefrLevel>())
            all.AddRange(await _writingRepository.GetByLevelAndCategoryAsync(level, null, ct));
        return all;
    }

    private static SpeakingPrompt Sp(string title, string prompt, CefrLevel level, string cat, string model, List<string> keys) => new()
    {
        Id = ObjectId.GenerateNewId().ToString(),
        Prompt = prompt,
        CefrLevel = level,
        Category = cat,
        ModelAnswer = model,
        KeyPhrases = keys
    };

    private static WritingPrompt Wp(string title, string prompt, CefrLevel level, string cat, string model, int limit) => new()
    {
        Id = ObjectId.GenerateNewId().ToString(),
        Prompt = prompt,
        CefrLevel = level,
        Category = cat,
        ModelAnswer = model,
        WordLimit = limit
    };

    private static List<ComprehensionQuestion> Questions(
        params (string Question, string[] Options, int CorrectIndex)[] items) =>
        items.Select(item => new ComprehensionQuestion
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Question = item.Question,
            Options = item.Options.ToList(),
            CorrectIndex = item.CorrectIndex
        }).ToList();

    private static List<Lesson> BuildLessons(List<GrammarRule> rules, List<Exercise> exercises)
    {
        var themes = new[]
        {
            ("Introducing yourself", "greetings", "greetings", 1),
            ("At the restaurant", "food", "restaurant", 2),
            ("Asking for directions", "directions", "directions", 3),
            ("Shopping", "shopping", "shopping", 4),
            ("Daily routine", "routine", "routine", 5),
            ("Past experiences", "past_events", "travel", 6),
            ("At work", "work", "job_interview", 7),
            ("Health and body", "health", "health", 8),
            ("Business meetings", "work", "meetings", 9),
            ("Professional emails", "work", "emails", 10),
            ("Negotiations", "work", "negotiations", 11),
            ("Presentations", "work", "presentations", 12),
            ("Phone calls at work", "work", "phone", 13),
            ("Job applications", "work", "job_interview", 14),
            ("Team collaboration", "work", "teamwork", 15),
            ("Customer service", "work", "customer", 16),
            ("Travel for business", "travel", "business_travel", 17),
            ("Housing and rent", "places", "housing", 18),
            ("Education and courses", "common", "education", 19),
            ("Technology at work", "technology", "technology", 20)
        };

        var lessons = new List<Lesson>();
        foreach (var (title, cat, theme, seq) in themes)
        {
            var level = seq <= 4 ? CefrLevel.A1 : seq <= 8 ? CefrLevel.A2 : CefrLevel.B1;
            var ruleIds = rules.Where(r =>
                    r.Category == cat
                    || (seq <= 4 && r.CefrLevel == CefrLevel.A1)
                    || (seq > 4 && seq <= 8 && r.CefrLevel == CefrLevel.A2)
                    || (seq > 8 && r.CefrLevel <= CefrLevel.B1))
                .Take(2).Select(r => r.Id).ToList();
            var exIds = exercises.Where(e => ruleIds.Contains(e.GrammarRuleId ?? "")).Select(e => e.Id).Take(4).ToList();
            lessons.Add(new Lesson
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = title,
                Description = $"Practice {title.ToLower()} with grammar and exercises.",
                CefrLevel = level,
                Category = cat,
                Theme = theme,
                Sequence = seq,
                GrammarRuleIds = ruleIds,
                ExerciseIds = exIds,
                IsLocked = seq > 1
            });
        }
        if (lessons.Count > 0) lessons[0].IsLocked = false;
        return lessons;
    }

    private static List<ReadingText> GetReadingTexts() =>
    [
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "A Day at the Market",
            Body = "Maria goes to the market every Saturday. She buys fresh fruit and bread. The market is near her house. She likes to talk to the sellers. They are very friendly. Maria pays with cash. She walks home with two bags.",
            CefrLevel = CefrLevel.A1,
            WordCount = 45,
            Source = "InglesApp Original",
            Attribution = "Original simplified text for learning.",
            Questions = Questions(
                ("Where does Maria go?", ["To school", "To the market", "To work", "To the park"], 1),
                ("When does she go?", ["Every day", "Every Saturday", "Every month", "Never"], 1),
                ("How does she pay?", ["By card", "With cash", "By phone", "With a cheque"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "My Morning Routine",
            Body = "I wake up at six thirty every day. First, I drink a glass of water. Then I get dressed and have breakfast. I usually eat bread, fruit, and yoghurt. I leave home at seven fifteen and walk to the bus stop. I start work at eight. I like my morning routine because it is calm.",
            CefrLevel = CefrLevel.A1,
            WordCount = 62,
            Source = "InglesApp Original",
            Attribution = "Original text for A1 reading practice.",
            Questions = Questions(
                ("What time does the writer wake up?", ["At six", "At six thirty", "At seven", "At eight"], 1),
                ("How does the writer go to work?", ["By car", "By train", "On foot and by bus", "By bike"], 2),
                ("Why does the writer like the routine?", ["It is fast", "It is calm", "It is cheap", "It is new"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "A New Neighbour",
            Body = "A new family moved into the house next to mine. The parents are from Colombia, and they have two children. The children go to the local school. On Saturday, I made a cake and took it to their house. They were very happy. Now we often have coffee together. I am learning some Spanish words, and they are practising English with me.",
            CefrLevel = CefrLevel.A1,
            WordCount = 75,
            Source = "InglesApp Original",
            Attribution = "Original text for A1 reading practice.",
            Questions = Questions(
                ("Where is the new family from?", ["Mexico", "Spain", "Colombia", "Peru"], 2),
                ("What did the writer take to their house?", ["Bread", "A cake", "Coffee", "Fruit"], 1),
                ("What are they practising together?", ["Cooking", "English and Spanish", "Football", "Music"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "At the Supermarket",
            Body = "On Friday evening, I go to the supermarket. I need milk, eggs, rice, and some vegetables. The supermarket is busy, but the queues are not very long. I compare prices because I want to save money. I buy a large bag of rice because it is cheaper. I pay by card and put everything in my bag. Then I walk home and make dinner.",
            CefrLevel = CefrLevel.A1,
            WordCount = 70,
            Source = "InglesApp Original",
            Attribution = "Original text for A1 reading practice.",
            Questions = Questions(
                ("When does the writer go shopping?", ["Friday evening", "Saturday morning", "Sunday", "Monday"], 0),
                ("Why does the writer compare prices?", ["To save money", "To meet friends", "To buy fruit", "To find the exit"], 0),
                ("How does the writer pay?", ["In cash", "By card", "By phone", "With a cheque"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "The Little Prince (adapted excerpt)",
            Body = "Once when I was six years old I saw a picture in a book. It showed a boa constrictor swallowing an animal. I drew my first drawing. Grown-ups did not understand it. They told me to study geography and history. I learned that grown-ups often need explanations. Today, I still remember that drawing and the way adults see the world.",
            CefrLevel = CefrLevel.A2,
            WordCount = 68,
            Source = "Project Gutenberg - The Little Prince by Antoine de Saint-Exupéry",
            Attribution = "Public domain. Adapted for A2 learners.",
            Questions = Questions(
                ("How old was the narrator?", ["Four", "Five", "Six", "Ten"], 2),
                ("What did grown-ups suggest?", ["Art", "Geography and history", "Music", "Sports"], 1),
                ("What does the narrator remember today?", ["The bookshop", "The drawing and adults' view", "A school teacher", "A friend"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "A Weekend in Lisbon",
            Body = "Last spring, my partner and I spent a weekend in Lisbon. We stayed in a small hotel near the river. On Saturday morning, we visited the old streets and took a tram to the castle. The views were amazing. In the afternoon, we ate fresh fish in a local restaurant. On Sunday, it rained, so we visited a museum instead of going to the beach. We would love to go back next year.",
            CefrLevel = CefrLevel.A2,
            WordCount = 80,
            Source = "InglesApp Original",
            Attribution = "Original text for A2 reading practice.",
            Questions = Questions(
                ("Where did they stay?", ["Near the airport", "Near the river", "In the castle", "At the beach"], 1),
                ("What did they do on Saturday afternoon?", ["They visited a museum", "They ate fish", "They went shopping", "They stayed in the hotel"], 1),
                ("Why didn't they go to the beach?", ["It was too far", "It was raining", "It was closed", "They were tired"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "Learning to Cook",
            Body = "When I moved into my first flat, I could not cook. I ate sandwiches and frozen pizza almost every day. After a few months, I decided to learn. I watched short videos and started with simple recipes: pasta, soup, and rice with vegetables. I made many mistakes, but I slowly improved. Now I can prepare dinner for my friends, and I enjoy trying new dishes. Cooking has become one of my favourite hobbies.",
            CefrLevel = CefrLevel.A2,
            WordCount = 82,
            Source = "InglesApp Original",
            Attribution = "Original text for A2 reading practice.",
            Questions = Questions(
                ("What did the writer eat at first?", ["Pasta and soup", "Sandwiches and frozen pizza", "Rice and vegetables", "Fish"], 1),
                ("How did the writer learn?", ["At a cooking school", "From videos and simple recipes", "From a friend", "From a book"], 1),
                ("How does the writer feel about cooking now?", ["It is boring", "It is difficult", "It is a favourite hobby", "It is too expensive"], 2))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "The Lost Phone",
            Body = "On Tuesday, Sara lost her phone on the bus. She realized it was missing when she arrived at work. She used her computer to call the bus company and asked if anyone had found it. In the afternoon, a driver called her. A passenger had given him the phone. Sara went to the station to collect it. She was very relieved and thanked the driver. Now she always checks her bag before she gets off the bus.",
            CefrLevel = CefrLevel.A2,
            WordCount = 82,
            Source = "InglesApp Original",
            Attribution = "Original text for A2 reading practice.",
            Questions = Questions(
                ("Where did Sara lose her phone?", ["At work", "On the bus", "At home", "In a shop"], 1),
                ("Who called her in the afternoon?", ["A passenger", "A driver", "Her manager", "A friend"], 1),
                ("What does Sara do now before getting off the bus?", ["She calls the driver", "She checks her bag", "She takes a photo", "She writes a note"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "My First Job Interview",
            Body = "Last week I had my first job interview. I arrived ten minutes early. The manager asked about my experience and skills. I explained my projects calmly. She said they would call me soon. I felt nervous but prepared. The next day, I received an email with an offer. I was surprised and very happy. I learned that preparation and practice really make a difference.",
            CefrLevel = CefrLevel.B1,
            WordCount = 72,
            Source = "InglesApp Original",
            Attribution = "Original text for professional English practice.",
            Questions = Questions(
                ("When did the interview happen?", ["Yesterday", "Last week", "Last month", "Last year"], 1),
                ("How did the writer feel during the interview?", ["Bored", "Nervous but prepared", "Angry", "Confused"], 1),
                ("What happened the next day?", ["The writer started work", "The writer received an offer", "The writer had another interview", "The writer called the manager"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "Remote Work: A New Routine",
            Body = "Remote work has changed the way many people organize their day. Some employees save time because they no longer commute. They can start work earlier and spend more time with their families. However, remote work also has disadvantages. Some people feel isolated, and it can be difficult to separate work from personal life. Experts suggest creating a fixed schedule, taking regular breaks, and staying in contact with colleagues. A good routine can make remote work healthy and productive.",
            CefrLevel = CefrLevel.B1,
            WordCount = 88,
            Source = "InglesApp Original",
            Attribution = "Original text for B1 reading practice.",
            Questions = Questions(
                ("Why do some employees save time?", ["They work less", "They don't commute", "They earn more", "They have shorter meetings"], 1),
                ("What is one disadvantage mentioned?", ["Higher salaries", "Feeling isolated", "Longer holidays", "Better technology"], 1),
                ("What do experts recommend?", ["Working without breaks", "A fixed schedule and regular breaks", "Avoiding colleagues", "Changing jobs often"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "A City Without Cars",
            Body = "Some cities are trying to reduce the number of cars in their centres. They build bike lanes, improve public transport, and create pedestrian areas. Supporters say these changes reduce pollution and make streets safer. Local businesses often worry that customers will stop visiting, but studies in several cities show that sales can increase when streets become more attractive. The transition is not always easy, but many residents enjoy cleaner air and quieter neighbourhoods.",
            CefrLevel = CefrLevel.B1,
            WordCount = 86,
            Source = "InglesApp Original",
            Attribution = "Original text for B1 reading practice.",
            Questions = Questions(
                ("What are some cities trying to reduce?", ["Public transport", "Cars in city centres", "Bike lanes", "Pedestrian areas"], 1),
                ("What do supporters say about the changes?", ["They increase pollution", "They make streets safer", "They reduce tourism", "They are too expensive"], 1),
                ("What can happen to sales when streets are more attractive?", ["They always fall", "They can increase", "They stay the same", "They are not measured"], 1))
        },
        new()
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = "The Volunteer Project",
            Body = "Last summer, I joined a volunteer project at a community centre. We organized English classes for older people and helped with a small garden. At first, I was nervous because I had never taught before. The coordinator gave us a short training session and simple materials. Over six weeks, I learned to explain things clearly and to listen patiently. The participants were grateful, but I think I learned more than they did. The experience made me more confident and encouraged me to continue volunteering.",
            CefrLevel = CefrLevel.B1,
            WordCount = 98,
            Source = "InglesApp Original",
            Attribution = "Original text for B1 reading practice.",
            Questions = Questions(
                ("What did the volunteer project organize?", ["A sports competition", "English classes and a garden", "A music festival", "A cooking course"], 1),
                ("Why was the writer nervous at first?", ["The garden was too big", "They had never taught before", "They didn't speak English", "The centre was closed"], 1),
                ("What did the writer learn from the experience?", ["How to cook", "How to be more confident", "How to drive", "How to manage money"], 1))
        }
    ];

    private static List<VocabularyPack> GetVocabularyPacks() =>
    [
        new(CefrLevel.A1, "routine",
        [
            V("wake up", "despertarse", "phrasal verb", "I wake up at seven every day."),
            V("get dressed", "vestirse", "phrasal verb", "She gets dressed before breakfast."),
            V("have breakfast", "desayunar", "collocation", "We have breakfast at eight."),
            V("commute", "desplazarse al trabajo", "verb", "I commute by bus."),
            V("run errands", "hacer recados", "collocation", "I run errands on Saturdays."),
            V("go to bed", "acostarse", "collocation", "They go to bed early.")
        ]),
        new(CefrLevel.A1, "food",
        [
            V("groceries", "comestibles", "noun", "I buy groceries every Friday."),
            V("receipt", "recibo", "noun", "Keep the receipt, please."),
            V("fresh", "fresco", "adjective", "The market sells fresh fruit."),
            V("cheap", "barato", "adjective", "Vegetables are cheap here."),
            V("loaf of bread", "barra de pan", "noun phrase", "I need a loaf of bread."),
            V("cash", "efectivo", "noun", "Can I pay in cash?")
        ]),
        new(CefrLevel.A1, "places",
        [
            V("neighbourhood", "barrio", "noun", "My neighbourhood is quiet."),
            V("corner", "esquina", "noun", "The café is on the corner."),
            V("crossroads", "cruce", "noun", "Turn left at the crossroads."),
            V("bus stop", "parada de autobús", "noun phrase", "The bus stop is near my house."),
            V("pharmacy", "farmacia", "noun", "There is a pharmacy on this street."),
            V("library", "biblioteca", "noun", "The library opens at nine.")
        ]),
        new(CefrLevel.A2, "travel",
        [
            V("booking", "reserva", "noun", "I made a booking online."),
            V("departure", "salida", "noun", "Our departure is at six."),
            V("luggage", "equipaje", "noun", "My luggage is very heavy."),
            V("sightseeing", "hacer turismo", "noun", "We went sightseeing in the old town."),
            V("accommodation", "alojamiento", "noun", "The accommodation was clean and cheap."),
            V("delay", "retraso", "noun", "The flight had a two-hour delay.")
        ]),
        new(CefrLevel.A2, "work",
        [
            V("deadline", "fecha límite", "noun", "The deadline is next Friday."),
            V("colleague", "compañero de trabajo", "noun", "My colleague helped me with the report."),
            V("schedule", "horario", "noun", "My schedule is full today."),
            V("update", "actualización", "noun", "I sent an update to the team."),
            V("attend", "asistir", "verb", "I need to attend a meeting."),
            V("task", "tarea", "noun", "This task is urgent.")
        ]),
        new(CefrLevel.A2, "health",
        [
            V("appointment", "cita", "noun", "I have a doctor's appointment at four."),
            V("symptom", "síntoma", "noun", "A headache is a common symptom."),
            V("prescription", "receta", "noun", "The doctor gave me a prescription."),
            V("recover", "recuperarse", "verb", "She is recovering from a cold."),
            V("pain", "dolor", "noun", "I have a pain in my back."),
            V("healthy", "saludable", "adjective", "I try to eat healthy food.")
        ]),
        new(CefrLevel.B1, "opinions",
        [
            V("point of view", "punto de vista", "noun phrase", "From my point of view, the plan is risky."),
            V("argue", "argumentar", "verb", "She argued that the cost was too high."),
            V("agree", "estar de acuerdo", "verb", "I agree with your opinion."),
            V("concern", "preocupación", "noun", "My main concern is safety."),
            V("evidence", "evidencia", "noun", "There is no evidence for that claim."),
            V("reasonable", "razonable", "adjective", "That seems like a reasonable solution.")
        ]),
        new(CefrLevel.B1, "environment",
        [
            V("climate change", "cambio climático", "noun phrase", "Climate change affects everyone."),
            V("pollution", "contaminación", "noun", "Air pollution is a serious problem."),
            V("sustainable", "sostenible", "adjective", "We need sustainable energy."),
            V("waste", "residuos", "noun", "Reduce waste at home."),
            V("renewable", "renovable", "adjective", "Solar power is renewable."),
            V("carbon footprint", "huella de carbono", "noun phrase", "Cycling reduces your carbon footprint.")
        ]),
        new(CefrLevel.B1, "technology",
        [
            V("device", "dispositivo", "noun", "This device connects to Wi-Fi."),
            V("update", "actualizar", "verb", "Please update the app."),
            V("password", "contraseña", "noun", "Use a strong password."),
            V("storage", "almacenamiento", "noun", "My phone has enough storage."),
            V("privacy", "privacidad", "noun", "Privacy is important online."),
            V("backup", "copia de seguridad", "noun", "Make a backup of your files.")
        ])
    ];

    private static VocabSeedItem V(string word, string translation, string partOfSpeech, params string[] examples) => new()
    {
        Word = word,
        Translation = translation,
        PartOfSpeech = partOfSpeech,
        Category = string.Empty,
        ExampleSentences = examples.ToList()
    };

    private static List<VocabSeedItem> GetInlineA2Vocabulary() => [];

    private static List<T> LoadJson<T>(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null) return [];
        using var reader = new StreamReader(stream);
        return JsonSerializer.Deserialize<List<T>>(reader.ReadToEnd(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
    }

    private sealed class VocabSeedItem
    {
        public string Word { get; set; } = "";
        public string Translation { get; set; } = "";
        public string PartOfSpeech { get; set; } = "";
        public string Category { get; set; } = "";
        public List<string> ExampleSentences { get; set; } = [];
    }

    private sealed record VocabularyPack(CefrLevel Level, string Category, List<VocabSeedItem> Items);
}
