using InglesApp.Application.Interfaces;
using InglesApp.Application.Services;
using InglesApp.Domain.Entities;
using InglesApp.Domain.Enums;
using InglesApp.Domain.Interfaces;
using MongoDB.Bson;
using System.Reflection;
using System.Text.Json;

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
    private readonly ExerciseGeneratorService _exerciseGenerator;
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
        ExerciseGeneratorService exerciseGenerator,
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
        _exerciseGenerator = exerciseGenerator;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAllAsync(CancellationToken ct = default)
    {
        await SeedDemoUserAsync(ct);
        await SeedA2VocabularyAsync(ct);
        await SeedLevelVocabularyAsync(CefrLevel.B1, "InglesApp.Infrastructure.Seed.b1-vocabulary.json", ct);
        await SeedLevelVocabularyAsync(CefrLevel.B2, "InglesApp.Infrastructure.Seed.b2-vocabulary.json", ct);
        if (!await _grammarRepository.AnyAsync(ct))
            await SeedGrammarAndLessonsAsync(ct);
        if (!await _readingRepository.AnyAsync(ct))
            await SeedReadingAsync(ct);
        if (!await _listeningRepository.AnyAsync(ct))
            await SeedListeningAsync(ct);
        if (!await _speakingRepository.AnyAsync(ct))
            await SeedSpeakingAsync(ct);
        if (!await _writingRepository.AnyAsync(ct))
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
        if (await _vocabularyRepository.CountByLevelAsync(level, ct) > 0) return;
        var items = LoadJson<VocabSeedItem>(resource);
        if (items.Count == 0) return;
        var vocab = items.Select(s => new VocabularyItem
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Word = s.Word,
            Translation = s.Translation,
            PartOfSpeech = s.PartOfSpeech,
            CefrLevel = level,
            Category = s.Category,
            ExampleSentences = s.ExampleSentences
        }).ToList();
        await _vocabularyRepository.InsertManyAsync(vocab, ct);
    }

    private async Task SeedA2VocabularyAsync(CancellationToken ct)
    {
        if (await _vocabularyRepository.CountByLevelAsync(CefrLevel.A2, ct) > 0) return;
        var items = LoadJson<VocabSeedItem>("InglesApp.Infrastructure.Seed.a2-vocabulary.json");
        if (items.Count == 0) items = GetInlineA2Vocabulary();
        var vocab = items.Select(s => new VocabularyItem
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Word = s.Word,
            Translation = s.Translation,
            PartOfSpeech = s.PartOfSpeech,
            CefrLevel = CefrLevel.A2,
            Category = s.Category,
            ExampleSentences = s.ExampleSentences
        }).ToList();
        await _vocabularyRepository.InsertManyAsync(vocab, ct);
    }

    private async Task SeedGrammarAndLessonsAsync(CancellationToken ct)
    {
        var rules = GetGrammarRules();
        foreach (var r in rules) r.Id = ObjectId.GenerateNewId().ToString();
        await _grammarRepository.InsertManyAsync(rules, ct);

        var allExercises = new List<Exercise>();
        foreach (var rule in rules)
        {
            var generated = _exerciseGenerator.GenerateFromGrammarRule(rule);
            foreach (var ex in generated) ex.Id = ObjectId.GenerateNewId().ToString();
            allExercises.AddRange(generated);
        }
        await _exerciseRepository.InsertManyAsync(allExercises, ct);

        var lessons = BuildLessons(rules, allExercises);
        await _lessonRepository.InsertManyAsync(lessons, ct);
    }

    private async Task SeedReadingAsync(CancellationToken ct)
    {
        var texts = new List<ReadingText>
        {
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = "A Day at the Market",
                Body = "Maria goes to the market every Saturday. She buys fresh fruit and bread. The market is near her house. She likes to talk to the sellers. They are very friendly. Maria pays with cash. She walks home with two bags.",
                CefrLevel = CefrLevel.A1,
                WordCount = 42,
                Source = "InglesApp Original",
                Attribution = "Original simplified text for learning.",
                Questions = Questions("Where does Maria go?", ["To school", "To the market", "To work", "To the park"], 1,
                    "When does she go?", ["Every day", "Every Saturday", "Every month", "Never"], 1)
            },
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = "The Little Prince (adapted excerpt)",
                Body = "Once when I was six years old I saw a picture in a book. It showed a boa constrictor swallowing an animal. I drew my first drawing. Grown-ups did not understand it. They told me to study geography and history.",
                CefrLevel = CefrLevel.A2,
                WordCount = 45,
                Source = "Project Gutenberg - The Little Prince by Antoine de Saint-Exupéry",
                Attribution = "Public domain. Adapted for A2 learners.",
                Questions = Questions("How old was the narrator?", ["Four", "Five", "Six", "Ten"], 2,
                    "What did grown-ups suggest?", ["Art", "Geography and history", "Music", "Sports"], 1)
            },
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = "My First Job Interview",
                Body = "Last week I had my first job interview. I arrived ten minutes early. The manager asked about my experience and skills. I explained my projects calmly. She said they would call me soon. I felt nervous but prepared.",
                CefrLevel = CefrLevel.B1,
                WordCount = 44,
                Source = "InglesApp Original",
                Attribution = "Original text for professional English practice.",
                Questions = Questions("When did the interview happen?", ["Yesterday", "Last week", "Last month", "Last year"], 1)
            }
        };
        await _readingRepository.InsertManyAsync(texts, ct);
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
                Questions = Questions("What does the customer order?", ["Tea and cake", "Coffee and sandwich", "Water", "Pizza"], 1)
            },
            new()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = "Asking for Directions",
                Transcript = "Excuse me, where is the train station? Go straight for two blocks, then turn left. The station is next to the bank. Thank you very much. You are welcome.",
                CefrLevel = CefrLevel.A2,
                Category = "directions",
                Questions = Questions("What is next to the station?", ["A park", "A bank", "A school", "A hotel"], 1)
            }
        };
        await _listeningRepository.InsertManyAsync(items, ct);
    }

    private async Task SeedSpeakingAsync(CancellationToken ct)
    {
        var prompts = new List<SpeakingPrompt>
        {
            Sp("Introduce yourself", "Tell me about yourself. Include your name, where you live, and what you do.", CefrLevel.A1, "greetings",
                "My name is Ana. I live in Madrid. I am a student. I like music and reading.",
                ["my name is", "I live in", "I am a", "I like"]),
            Sp("Order food", "You are at a restaurant. Order a meal and a drink politely.", CefrLevel.A1, "restaurant",
                "I would like the chicken with rice, please. Can I have a glass of water? Thank you.",
                ["I would like", "please", "thank you"]),
            Sp("Tell me about yourself", "Describe your background, skills, and why you are looking for a job.", CefrLevel.B1, "job_interview",
                "I am a software developer with three years of experience. I enjoy solving problems and working in teams. I am looking for new challenges in an international company.",
                ["experience", "I enjoy", "I am looking for", "team"]),
            Sp("Describe your experience", "Talk about a project you are proud of and what you learned.", CefrLevel.B1, "job_interview",
                "I built a web application for language learning. I learned about databases and user experience. The project helped me improve my English.",
                ["I built", "I learned", "project", "improve"]),
            Sp("Why this company", "Explain why you want to work at this company.", CefrLevel.B1, "job_interview",
                "I admire your company's mission. My skills match the role. I want to grow professionally here.",
                ["I admire", "my skills", "grow professionally"])
        };
        for (var i = 0; i < 5; i++)
            prompts.Add(Sp($"B1 topic {i + 1}", "Discuss a topic related to work and daily life.", CefrLevel.B1, "job_interview",
                "In my opinion, communication is essential at work.", ["in my opinion", "communication", "work"]));
        await _speakingRepository.InsertManyAsync(prompts, ct);
    }

    private async Task SeedWritingAsync(CancellationToken ct)
    {
        var prompts = new List<WritingPrompt>
        {
            Wp("Describe your daily routine", "Write about what you do on a typical day.", CefrLevel.A1, "common", "I wake up at seven. I have breakfast and go to work. In the evening I study English.", 50),
            Wp("A letter to a friend", "Write a short email to a friend about your weekend.", CefrLevel.A2, "common", "Hi! How are you? I had a great weekend. I visited my family and watched a film. Write soon!", 60),
            Wp("Cover letter paragraph", "Write a paragraph for a job application explaining your interest.", CefrLevel.B1, "job_interview", "I am writing to apply for the developer position. I have experience with web technologies and I am eager to contribute to your team.", 80),
            Wp("Professional email", "Write a polite email requesting a meeting with your manager.", CefrLevel.B1, "job_interview", "Dear Manager, I would like to schedule a meeting to discuss my progress. I am available next week. Best regards.", 70)
        };
        for (var i = 0; i < 6; i++)
            prompts.Add(Wp($"Work email {i + 1}", "Write a short professional message to a colleague.", CefrLevel.B1, "job_interview", "Hi team, please find the report attached. Let me know if you have questions.", 60));
        await _writingRepository.InsertManyAsync(prompts, ct);
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

    private static List<ComprehensionQuestion> Questions(string q1, string[] o1, int c1, string? q2 = null, string[]? o2 = null, int? c2 = null)
    {
        var list = new List<ComprehensionQuestion>
        {
            new() { Id = ObjectId.GenerateNewId().ToString(), Question = q1, Options = o1.ToList(), CorrectIndex = c1 }
        };
        if (q2 is not null && o2 is not null && c2.HasValue)
            list.Add(new() { Id = ObjectId.GenerateNewId().ToString(), Question = q2, Options = o2.ToList(), CorrectIndex = c2.Value });
        return list;
    }

    private static List<Lesson> BuildLessons(List<GrammarRule> rules, List<Exercise> exercises)
    {
        var themes = new[]
        {
            ("Introducing yourself", "greetings", "greetings", 1),
            ("At the restaurant", "food", "restaurant", 2),
            ("Asking for directions", "places", "directions", 3),
            ("Shopping", "common", "shopping", 4),
            ("Daily routine", "verbs", "routine", 5),
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
            ("Technology at work", "work", "technology", 20)
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

    private static List<GrammarRule> GetGrammarRules() =>
    [
        Rule("Present Simple", "Use for habits and facts. I/You/We/They + verb. He/She/It + verb+s.", CefrLevel.A1, "verbs", ["I work every day.", "She likes coffee.", "They live in London."]),
        Rule("Articles a/an/the", "Use 'a' before consonant sounds, 'an' before vowel sounds, 'the' for specific things.", CefrLevel.A1, "common", ["I have a dog.", "She is an engineer.", "The book is on the table."]),
        Rule("To be", "Am/is/are for identity, age, nationality.", CefrLevel.A1, "greetings", ["I am Maria.", "He is tall.", "We are students."]),
        Rule("Questions with do/does", "Do I/you/we/they? Does he/she/it?", CefrLevel.A1, "common", ["Do you speak English?", "Does she work here?", "Do they live nearby?"]),
        Rule("Prepositions of place", "In, on, at for location.", CefrLevel.A1, "places", ["The keys are on the table.", "I am at home.", "She lives in Spain."]),
        Rule("Countable/uncountable", "Some/any with nouns.", CefrLevel.A1, "food", ["I need some water.", "Do you have any bread?", "There is some milk."]),
        Rule("Present Continuous", "Am/is/are + verb-ing for now.", CefrLevel.A2, "verbs", ["I am studying now.", "She is cooking dinner.", "They are waiting."]),
        Rule("Past Simple", "Regular: verb+ed. Irregular forms must be memorized.", CefrLevel.A2, "past_events", ["I visited Paris last year.", "She went to work.", "We watched a film."]),
        Rule("Comparatives", "Short adj + -er or more + adj.", CefrLevel.A2, "common", ["This is bigger.", "She is more talented.", "It is cheaper than before."]),
        Rule("Future with going to", "Going to + verb for plans.", CefrLevel.A2, "common", ["I am going to travel.", "She is going to study.", "We are going to eat."]),
        Rule("Present Perfect intro", "Have/has + past participle for experience.", CefrLevel.B1, "work", ["I have worked here for two years.", "She has visited London.", "They have finished the project."]),
        Rule("First Conditional", "If + present, will + verb.", CefrLevel.B1, "work", ["If I study, I will pass.", "If it rains, we will stay home."]),
        Rule("Passive voice intro", "Be + past participle.", CefrLevel.B1, "work", ["The email was sent.", "The report is written.", "The meeting was cancelled."]),
        Rule("Modal verbs", "Can, must, should for ability, obligation, advice.", CefrLevel.A2, "common", ["I can swim.", "You must wear a seatbelt.", "You should rest."]),
        Rule("Phrasal verbs basics", "Common combinations: get up, look for.", CefrLevel.B2, "work", ["I get up at seven.", "She is looking for a job.", "Please turn off the light."]),
        Rule("There is/there are", "For existence and quantity.", CefrLevel.A1, "common", ["There is a book.", "There are two cats.", "Is there a problem?"]),
        Rule("Possessive adjectives", "My, your, his, her, our, their.", CefrLevel.A1, "common", ["My name is Ana.", "Their house is big.", "Our teacher is kind."]),
        Rule("Object pronouns", "Me, you, him, her, us, them.", CefrLevel.A1, "common", ["She knows me.", "I called him.", "They helped us."]),
        Rule("Imperatives", "Base verb for instructions.", CefrLevel.A1, "common", ["Open the door.", "Don't run.", "Please sit down."]),
        Rule("Some/any", "Some in affirmatives, any in questions/negatives.", CefrLevel.A1, "food", ["I have some apples.", "Do you have any milk?", "There isn't any sugar."]),
        Rule("Much/many/a lot of", "Quantity expressions.", CefrLevel.A2, "common", ["I don't have much time.", "There are many people.", "She has a lot of friends."]),
        Rule("Used to", "Past habits that are no longer true.", CefrLevel.B1, "common", ["I used to play football.", "She used to live here.", "We didn't use to travel."]),
        Rule("Second Conditional", "If + past, would + verb for hypothetical.", CefrLevel.B1, "common", ["If I had money, I would travel.", "If she studied, she would pass."]),
        Rule("Reported speech intro", "Say/tell + that clause.", CefrLevel.B1, "work", ["She said that she was busy.", "He told me that he would come."]),
        Rule("Relative clauses", "Who, which, that.", CefrLevel.B1, "common", ["The man who called is my boss.", "The email that I sent was long."]),
        Rule("Present Perfect Continuous", "Have been + verb-ing.", CefrLevel.B2, "work", ["I have been working all day.", "She has been waiting for an hour."]),
        Rule("Third Conditional", "If + past perfect, would have + pp.", CefrLevel.B2, "common", ["If I had known, I would have helped.", "If they had left earlier, they would have arrived on time."]),
        Rule("Inversion for emphasis", "Never have I..., Rarely do we...", CefrLevel.B2, "academic", ["Never have I seen such a result.", "Rarely do we agree."]),
        Rule("Mixed conditionals", "Combine time references.", CefrLevel.B2, "common", ["If I had studied medicine, I would be a doctor now."])
    ];

    private static GrammarRule Rule(string title, string explanation, CefrLevel level, string cat, List<string> examples) => new()
    {
        Title = title,
        Explanation = explanation,
        CefrLevel = level,
        Category = cat,
        Examples = examples
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
}
