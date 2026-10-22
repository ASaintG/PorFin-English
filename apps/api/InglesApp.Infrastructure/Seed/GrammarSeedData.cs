using InglesApp.Domain.Enums;

namespace InglesApp.Infrastructure.Seed;

internal sealed record GrammarTopicSeed(
    string Title,
    CefrLevel Level,
    string Category,
    string Explanation,
    string[] SubRules,
    string[] CommonErrors,
    string[] Examples,
    string[] WrongExamples,
    string[] Distractors,
    string ProductionPrompt,
    string ProductionModel,
    Skill ProductionSkill,
    int ProductionWordLimit,
    string[] ProductionKeyPhrases);

internal static class GrammarSeedData
{
    public static readonly GrammarTopicSeed[] Topics =
    [
        // ── A1 ──────────────────────────────────────────────────────────────
        new(
            "To be",
            CefrLevel.A1,
            "greetings",
            "Usamos am/is/are para identidad, edad, nacionalidad, estado y ubicación. La forma depende del sujeto.",
            [
                "I + am: I am a student.",
                "He/She/It + is: She is a nurse.",
                "You/We/They + are: They are at home.",
                "Negativa: am not, isn't, aren't."
            ],
            [
                "Usar 'is' con I: I is tired → I am tired.",
                "Usar 'are' con he/she/it: She are happy → She is happy.",
                "Olvidar el verbo: I from Mexico → I am from Mexico."
            ],
            [
                "I {{am}} from Spain.",
                "She {{is}} a nurse.",
                "They are at home.",
                "You are a good student.",
                "My parents are retired."
            ],
            [
                "I is from Spain.",
                "She are a nurse.",
                "They is at home."
            ],
            ["am", "is", "are", "be"],
            "Preséntate en 30 segundos: nombre, edad, nacionalidad, ciudad y profesión. Usa am/is/are.",
            "Hi, I am Ana. I am 28 years old. I am from Mexico, but I live in Madrid. I am a teacher. My parents are in Mexico.",
            Skill.Speaking,
            45,
            ["I am", "I am from", "My ... is/are"]
        ),
        new(
            "Subject pronouns and possessive adjectives",
            CefrLevel.A1,
            "common",
            "Los subject pronouns (I, you, he, she, it, we, they) hacen de sujeto. Los possessive adjectives (my, your, his, her, its, our, their) van antes del sustantivo.",
            [
                "I → my; you → your; he → his; she → her.",
                "it → its; we → our; they → their.",
                "No confundas 'his' con 'her'.",
                "El adjetivo posesivo no cambia con el plural: my books."
            ],
            [
                "Her is my sister → She is my sister.",
                "They house is big → Their house is big.",
                "I call he every day → I call him every day."
            ],
            [
                "{{My}} name is Ana.",
                "{{Their}} house is big.",
                "She is my sister.",
                "We visit our grandparents.",
                "His car is new."
            ],
            [
                "Her is my sister.",
                "They house is big.",
                "I call he every day."
            ],
            ["His", "Her", "Our", "Your"],
            "Describe a tu familia en 45 segundos usando possessive adjectives.",
            "My mother is a doctor. Her name is Laura. My father is a teacher. His name is Carlos. Our house is in Madrid. Their dog is very friendly.",
            Skill.Speaking,
            50,
            ["my", "his", "her", "our", "their"]
        ),
        new(
            "Articles and plural nouns",
            CefrLevel.A1,
            "common",
            "Usamos 'a' antes de sonido consonántico, 'an' antes de sonido vocálico y 'the' para algo específico. Los plurales regulares añaden -s/-es; algunos son irregulares.",
            [
                "a + consonant sound: a book, a university.",
                "an + vowel sound: an apple, an hour.",
                "the + specific noun: the book on the table.",
                "Plural: -s (books), -es (boxes), irregular (children, people)."
            ],
            [
                "a apple → an apple.",
                "an teacher → a teacher.",
                "two childs → two children."
            ],
            [
                "I have {{an}} apple.",
                "She is {{a}} teacher.",
                "The children are playing.",
                "There are two boxes on the table.",
                "I need a new computer."
            ],
            [
                "I have a apple.",
                "She is an teacher.",
                "The childs are playing."
            ],
            ["a", "an", "the", "some"],
            "Escribe 5 frases sobre cosas que tienes y ves. Usa a/an/the y al menos dos plurales.",
            "I have a laptop and an umbrella. The laptop is new. There are two books on my desk. The children in my building play outside. I need an hour to finish.",
            Skill.Writing,
            60,
            ["a", "an", "the", "plural -s"]
        ),
        new(
            "Possessive 's and family",
            CefrLevel.A1,
            "family",
            "Usamos 's para indicar posesión o relación: Ana's brother. Para plurales terminados en -s, solo añadimos el apóstrofo: my parents' house.",
            [
                "Singular: person + 's + noun.",
                "Plural regular: people + ' + noun.",
                "Irregular plural: children's toys.",
                "También se usa para relaciones familiares: my sister's husband."
            ],
            [
                "Anas brother → Ana's brother.",
                "my parents's house → my parents' house.",
                "the childrens toys → the children's toys."
            ],
            [
                "This is {{Ana's}} brother.",
                "My {{parents'}} house is near the park.",
                "Her sister's name is Laura.",
                "The children's toys are in the box.",
                "Carlos's office is on the second floor."
            ],
            [
                "This is Anas brother.",
                "My parents's house is near the park.",
                "The childrens toys are in the box."
            ],
            ["Ana's", "Anas", "parents'", "parents's"],
            "Escribe un párrafo corto sobre tu familia y sus cosas usando 's y plurales posesivos.",
            "This is my family. My sister's name is Laura. My parents' house is in the city. My brother's car is blue. The children's room is upstairs.",
            Skill.Writing,
            70,
            ["'s", "parents'", "children's"]
        ),
        new(
            "Demonstratives",
            CefrLevel.A1,
            "shopping",
            "This/these para cosas cerca; that/those para cosas lejos. This/that son singulares; these/those son plurales.",
            [
                "this + singular near.",
                "these + plural near.",
                "that + singular far.",
                "those + plural far."
            ],
            [
                "This shoes are new → These shoes are new.",
                "Those car is old → That car is old.",
                "These is my bag → This is my bag."
            ],
            [
                "{{This}} shirt is too small.",
                "{{Those}} shoes are expensive.",
                "That bag is on sale.",
                "These jeans are comfortable.",
                "This is my receipt."
            ],
            [
                "This shoes are new.",
                "Those car is old.",
                "These is my bag."
            ],
            ["This", "These", "That", "Those"],
            "Estás en una tienda. Describe 5 productos usando this/that/these/those.",
            "This jacket is nice, but it is expensive. Those shoes are on sale. I like these jeans. That shirt is too small. I will take this one.",
            Skill.Speaking,
            45,
            ["this", "that", "these", "those"]
        ),
        new(
            "Present simple",
            CefrLevel.A1,
            "routine",
            "Usamos el present simple para rutinas, hábitos y verdades generales. Con he/she/it añadimos -s o -es.",
            [
                "I/you/we/they + base verb.",
                "he/she/it + verb-s/es.",
                "Spelling: watch → watches; study → studies.",
                "Expresiones: every day, usually, never."
            ],
            [
                "She work in a bank → She works in a bank.",
                "He go to school → He goes to school.",
                "They works together → They work together."
            ],
            [
                "She {{works}} in a bank.",
                "They {{live}} in Madrid.",
                "I get up at seven every day.",
                "He studies English at night.",
                "We usually eat dinner at eight."
            ],
            [
                "She work in a bank.",
                "He go to school.",
                "They works together."
            ],
            ["work", "works", "working", "lived"],
            "Describe tu rutina diaria en 60 segundos usando present simple.",
            "I get up at seven. I have breakfast and go to work. I work in an office. In the evening, I study English. I usually go to bed at eleven.",
            Skill.Speaking,
            60,
            ["I get up", "I work", "usually", "every day"]
        ),
        new(
            "Present simple questions and negatives",
            CefrLevel.A1,
            "routine",
            "Para preguntas y negativas usamos do/does + verbo base. Con he/she/it usamos does y el verbo pierde la -s.",
            [
                "Do I/you/we/they + verb?",
                "Does he/she/it + verb?",
                "don't/doesn't + verb base.",
                "Respuestas cortas: Yes, I do. / No, she doesn't."
            ],
            [
                "Does she works here? → Does she work here?",
                "She don't like coffee → She doesn't like coffee.",
                "Do he live here? → Does he live here?"
            ],
            [
                "{{Do}} you speak English?",
                "She {{doesn't}} like coffee.",
                "Does he work here?",
                "They don't live in London.",
                "Do you have any questions?"
            ],
            [
                "Does she works here?",
                "She don't like coffee.",
                "Do he live here?"
            ],
            ["do", "does", "don't", "doesn't"],
            "Haz 5 preguntas a un compañero sobre su rutina y responde con negativas.",
            "Do you work on Saturdays? No, I don't. Does your sister study English? Yes, she does. Do they live near here? No, they don't. Does he drive to work? No, he doesn't.",
            Skill.Speaking,
            60,
            ["Do you...?", "Does he/she...?", "don't", "doesn't"]
        ),
        new(
            "Adverbs of frequency and time prepositions",
            CefrLevel.A1,
            "routine",
            "Los adverbios de frecuencia (always, usually, often, sometimes, never) van antes del verbo principal y después de 'to be'. Usamos in/on/at para tiempo.",
            [
                "always > usually > often > sometimes > never.",
                "Before main verb: I usually walk.",
                "After to be: She is always late.",
                "in + month/year/part of day; on + day/date; at + time/night."
            ],
            [
                "I go always to work by bus → I always go to work by bus.",
                "We have class in Monday → We have class on Monday.",
                "I wake up at seven on the morning → I wake up at seven in the morning."
            ],
            [
                "I {{usually}} walk to work.",
                "She is {{always}} late.",
                "We have class on Monday.",
                "I wake up at seven in the morning.",
                "They never eat meat."
            ],
            [
                "I go always to work by bus.",
                "We have class in Monday.",
                "I wake up at seven on the morning."
            ],
            ["always", "usually", "sometimes", "never"],
            "Escribe 6 frases sobre tu semana usando adverbios de frecuencia y in/on/at.",
            "I usually get up at seven. I always have coffee in the morning. On Mondays, I go to the gym. I sometimes work late. I never study on Sundays. I visit my family at Christmas.",
            Skill.Writing,
            80,
            ["usually", "always", "on Monday", "in the morning", "at night"]
        ),
        new(
            "Can/can't",
            CefrLevel.A1,
            "hobbies",
            "Can expresa habilidad o posibilidad. La forma es igual para todas las personas: can + verbo base. La negativa es can't/cannot.",
            [
                "I/you/he/she/it/we/they + can + verb.",
                "Negative: can't / cannot + verb.",
                "Question: Can you...? Yes, I can. / No, I can't.",
                "No 'to' after can: I can swim, not I can to swim."
            ],
            [
                "She can to swim → She can swim.",
                "He cans play guitar → He can play guitar.",
                "I can't to come → I can't come."
            ],
            [
                "I {{can}} swim very well.",
                "She {{can't}} come to the party.",
                "Can you play the guitar?",
                "We can meet at six.",
                "He can't drive."
            ],
            [
                "She can to swim.",
                "He cans play guitar.",
                "I can't to come."
            ],
            ["can", "can't", "could", "cans"],
            "Habla de 5 cosas que puedes y no puedes hacer, y haz 2 preguntas.",
            "I can cook Italian food. I can't play the piano. I can swim, but I can't dive. Can you speak French? Can you drive?",
            Skill.Speaking,
            50,
            ["I can", "I can't", "Can you...?"]
        ),
        new(
            "Prepositions of place and there is/are",
            CefrLevel.A1,
            "places",
            "Usamos in/on/at para lugar y there is/there are para existencia. There is + singular; there are + plural.",
            [
                "in + city/country/enclosed space.",
                "on + surface.",
                "at + specific point (at the door, at work).",
                "There is a... / There are some...",
                "Question: Is there...? Are there...?"
            ],
            [
                "There is two banks → There are two banks.",
                "The book is in the table → The book is on the table.",
                "I am at Spain → I am in Spain."
            ],
            [
                "There {{is}} a bank near here.",
                "There {{are}} two supermarkets on this street.",
                "The keys are on the table.",
                "I am at home.",
                "She lives in Madrid."
            ],
            [
                "There is two banks near here.",
                "The book is in the table.",
                "I am at Spain."
            ],
            ["is", "are", "in", "on", "at"],
            "Describe tu barrio en 60 segundos usando there is/are y preposiciones de lugar.",
            "There is a park near my house. There are two cafés on my street. The supermarket is next to the bank. I am usually at home in the evening.",
            Skill.Speaking,
            60,
            ["there is", "there are", "next to", "on", "in"]
        ),
        new(
            "Countable/uncountable, some/any, much/many",
            CefrLevel.A1,
            "food",
            "Los contables tienen plural (apple/apples); los incontables no (water, bread). Usamos some en afirmativas, any en preguntas/negativas, much con incontables y many con contables.",
            [
                "some + plural/uncountable in affirmative.",
                "any + plural/uncountable in questions/negatives.",
                "much + uncountable; many + countable.",
                "a lot of in all types.",
                "a few + countable; a little + uncountable."
            ],
            [
                "I have any bread → I have some bread.",
                "How much apples? → How many apples?",
                "There isn't many water → There isn't much water."
            ],
            [
                "I need {{some}} water.",
                "Do you have {{any}} bread?",
                "How many apples do you need?",
                "There isn't much milk in the fridge.",
                "She has a lot of friends."
            ],
            [
                "I have any bread.",
                "How much apples do you need?",
                "There isn't many water in the fridge."
            ],
            ["some", "any", "much", "many"],
            "Escribe una lista de la compra y explica qué necesitas usando some/any/much/many.",
            "I need some rice and some vegetables. I don't need any bread. How many eggs do we have? There isn't much milk. We have a lot of fruit.",
            Skill.Writing,
            70,
            ["some", "any", "much", "many", "a lot of"]
        ),
        new(
            "Imperatives and directions",
            CefrLevel.A1,
            "directions",
            "El imperativo usa el verbo base para instrucciones. La negativa es Don't + verb. Es común para dar direcciones.",
            [
                "Open the door. / Turn left.",
                "Negative: Don't turn right.",
                "Polite: Please + imperative.",
                "Directions: go straight, turn left/right, take the first street."
            ],
            [
                "You turn left → Turn left.",
                "Don't to run → Don't run.",
                "Go straightly → Go straight."
            ],
            [
                "{{Turn}} left at the bank.",
                "{{Don't}} cross the street here.",
                "Go straight for two blocks.",
                "Take the first street on the right.",
                "Please wait at the corner."
            ],
            [
                "You turn left at the bank.",
                "Don't to cross the street here.",
                "Go straightly for two blocks."
            ],
            ["Turn", "Turns", "Don't", "Doesn't"],
            "Da direcciones desde tu casa a la estación más cercana en 45 segundos.",
            "Go straight for two blocks. Turn left at the bank. Take the first street on the right. The station is next to the supermarket. Don't cross at the red light.",
            Skill.Speaking,
            60,
            ["go straight", "turn left", "take the first", "don't"]
        ),
        new(
            "Object pronouns and like + -ing",
            CefrLevel.A1,
            "hobbies",
            "Los object pronouns (me, you, him, her, it, us, them) reciben la acción. Con like/love/hate usamos el verbo en -ing o to + infinitive.",
            [
                "I → me; you → you; he → him; she → her; it → it.",
                "we → us; they → them.",
                "like/love/hate + -ing: I like reading.",
                "like + to + infinitive is also possible: I like to read."
            ],
            [
                "I call he → I call him.",
                "She likes read → She likes reading.",
                "They help we → They help us."
            ],
            [
                "I call {{him}} every day.",
                "She likes {{reading}}.",
                "They help us with the project.",
                "We love cooking together.",
                "He hates waiting in line."
            ],
            [
                "I call he every day.",
                "She likes read books.",
                "They help we with the project."
            ],
            ["him", "her", "reading", "to read"],
            "Habla de tus hobbies usando like/love/hate + -ing y object pronouns.",
            "I love reading and cooking. I like listening to music. I hate waiting in traffic. My friends help me with English, and I help them with Spanish.",
            Skill.Speaking,
            60,
            ["I like", "I love", "I hate", "me", "them"]
        ),
        new(
            "Wh-questions",
            CefrLevel.A1,
            "common",
            "Las preguntas con wh- (what, where, when, who, why, how) piden información. El orden es Wh- + auxiliar + sujeto + verbo.",
            [
                "What = cosa; Where = lugar; When = tiempo.",
                "Who = persona; Why = razón; How = manera.",
                "With do/does: Where do you live?",
                "With to be: Where are you from?"
            ],
            [
                "Where you live? → Where do you live?",
                "What means this? → What does this mean?",
                "Why you are late? → Why are you late?"
            ],
            [
                "Where {{do}} you live?",
                "What {{does}} she do?",
                "When do you start work?",
                "Why are you late?",
                "How do you spell your name?"
            ],
            [
                "Where you live?",
                "What means this word?",
                "Why you are late?"
            ],
            ["do", "does", "are", "is"],
            "Entrevista a un compañero con 6 wh-questions sobre su vida.",
            "Where do you live? What do you do? When do you start work? Why are you learning English? How do you practice? Who do you practice with?",
            Skill.Speaking,
            60,
            ["Where do", "What does", "When do", "Why are", "How do"]
        ),

        // ── A2 ──────────────────────────────────────────────────────────────
        new(
            "Present continuous",
            CefrLevel.A2,
            "verbs",
            "Usamos am/is/are + verb-ing para acciones que ocurren ahora, situaciones temporales y planes ya organizados.",
            [
                "Affirmative: I am working.",
                "Negative: She isn't working.",
                "Question: Are you working?",
                "Time expressions: now, right now, at the moment, these days."
            ],
            [
                "She working now → She is working now.",
                "I am work → I am working.",
                "They is studying → They are studying."
            ],
            [
                "I {{am}} working from home today.",
                "She {{is}} studying for an exam.",
                "They are waiting for the bus.",
                "We are meeting Ana at six.",
                "He isn't listening to you."
            ],
            [
                "She working now.",
                "I am work today.",
                "They is studying."
            ],
            ["am", "is", "are", "be"],
            "Describe lo que están haciendo las personas de tu casa ahora mismo.",
            "I am sitting at my desk. My sister is cooking lunch. My parents are watching TV. The neighbours are working in the garden. I am not listening to music.",
            Skill.Writing,
            70,
            ["am/is/are + -ing", "now", "at the moment"]
        ),
        new(
            "Past simple regular",
            CefrLevel.A2,
            "past_events",
            "El past simple regular se forma con verb + -ed. Se usa para acciones terminadas en un momento concreto del pasado.",
            [
                "Regular: work → worked; live → lived; study → studied.",
                "Negative: didn't + base verb.",
                "Question: Did you + base verb?",
                "Time expressions: yesterday, last week, in 2020."
            ],
            [
                "I studyed → I studied.",
                "She didn't worked → She didn't work.",
                "Did you watched? → Did you watch?"
            ],
            [
                "I {{visited}} my family last weekend.",
                "She {{worked}} late yesterday.",
                "They watched a film on Saturday.",
                "We didn't travel last summer.",
                "Did you finish the report?"
            ],
            [
                "I studyed for the exam.",
                "She didn't worked late.",
                "Did you watched the film?"
            ],
            ["visited", "visit", "worked", "work"],
            "Escribe un correo a un amigo sobre lo que hiciste el fin de semana pasado.",
            "Hi! Last weekend I visited my family. We watched a film and cooked dinner. I didn't work on Sunday. I studied English for an hour. Did you do anything special?",
            Skill.Writing,
            80,
            ["last weekend", "visited", "didn't", "Did you...?"]
        ),
        new(
            "Past simple irregular",
            CefrLevel.A2,
            "past_events",
            "Muchos verbos tienen pasado irregular: go → went, have → had, see → saw. No añaden -ed.",
            [
                "go → went; have → had; see → saw.",
                "do → did; make → made; take → took.",
                "Negative: didn't + base verb.",
                "Question: Did + subject + base verb?"
            ],
            [
                "I goed to Madrid → I went to Madrid.",
                "She didn't went → She didn't go.",
                "Did you saw him? → Did you see him?"
            ],
            [
                "I {{went}} to Madrid last month.",
                "She {{had}} a great time.",
                "We saw an exhibition at the museum.",
                "They made dinner together.",
                "Did you take the train?"
            ],
            [
                "I goed to Madrid last month.",
                "She didn't went to the party.",
                "Did you saw him yesterday?"
            ],
            ["went", "go", "had", "have"],
            "Cuenta una experiencia pasada usando al menos 5 verbos irregulares.",
            "Last year I went to Italy. I saw Rome and Florence. I had a lot of pasta. I took hundreds of photos. I made new friends and I didn't want to come home.",
            Skill.Speaking,
            70,
            ["went", "saw", "had", "took", "made"]
        ),
        new(
            "Past continuous",
            CefrLevel.A2,
            "past_events",
            "Usamos was/were + verb-ing para una acción que estaba en progreso en un momento del pasado.",
            [
                "I/He/She/It + was + -ing.",
                "You/We/They + were + -ing.",
                "Negative: wasn't / weren't + -ing.",
                "Often used with while and at that moment."
            ],
            [
                "I were working → I was working.",
                "They was waiting → They were waiting.",
                "She wasn't work → She wasn't working."
            ],
            [
                "I {{was}} working when you called.",
                "They {{were}} waiting outside.",
                "She was cooking dinner at eight.",
                "We weren't listening to the news.",
                "What were you doing at ten?"
            ],
            [
                "I were working when you called.",
                "They was waiting outside.",
                "She wasn't work at eight."
            ],
            ["was", "were", "is", "are"],
            "Describe una situación que estaba pasando cuando ocurrió algo.",
            "I was walking home when I saw an old friend. It was raining and people were waiting for the bus. We were talking when my phone rang.",
            Skill.Writing,
            70,
            ["was/were + -ing", "when", "while"]
        ),
        new(
            "Past simple vs past continuous",
            CefrLevel.A2,
            "past_events",
            "Usamos el past simple para la acción terminada y el past continuous para la acción en progreso que sirve de contexto.",
            [
                "Past continuous: longer background action.",
                "Past simple: shorter interrupting action.",
                "While + past continuous; when + past simple.",
                "Both can appear in the same sentence."
            ],
            [
                "I was watch TV when she called → I was watching TV when she called.",
                "While I cooked, she called → While I was cooking, she called.",
                "When I was seeing him, he was running → When I saw him, he was running."
            ],
            [
                "I {{was}} watching TV when she called.",
                "He {{called}} while I was cooking.",
                "They were walking home when it started to rain.",
                "She was studying while he was working.",
                "We saw the accident while we were driving."
            ],
            [
                "I was watch TV when she called.",
                "While I cooked, she called.",
                "When I was seeing him, he was running."
            ],
            ["was", "were", "called", "saw"],
            "Escribe una anécdota corta combinando past simple y past continuous.",
            "I was walking to work when I saw a dog in the street. The dog was running and a man was calling it. I stopped and helped the man. Then we found the owner.",
            Skill.Writing,
            90,
            ["was/were + -ing", "when", "while", "past simple"]
        ),
        new(
            "Comparatives",
            CefrLevel.A2,
            "shopping",
            "Comparamos dos cosas con adjetivo + -er o more + adjetivo, seguido de than.",
            [
                "Short adjectives: cheap → cheaper than.",
                "Long adjectives: expensive → more expensive than.",
                "Irregular: good → better; bad → worse.",
                "Use than after the comparative."
            ],
            [
                "This is more cheap → This is cheaper.",
                "She is taller that me → She is taller than me.",
                "It is more better → It is better."
            ],
            [
                "This shirt is {{cheaper}} than that one.",
                "The blue car is {{more}} expensive than the red one.",
                "My sister is taller than me.",
                "This book is better than the film.",
                "The new phone is faster than my old one."
            ],
            [
                "This shirt is more cheap than that one.",
                "The blue car is expensiver than the red one.",
                "My sister is taller that me."
            ],
            ["cheaper", "more", "most", "expensiver"],
            "Compara dos productos que quieras comprar y explica cuál prefieres.",
            "I want to buy a laptop. The small one is cheaper, but the large one is more powerful. The large one is also heavier. I think the small one is better for travelling.",
            Skill.Writing,
            80,
            ["-er than", "more ... than", "better", "worse"]
        ),
        new(
            "Superlatives",
            CefrLevel.A2,
            "travel",
            "Usamos the + adjetivo + -est o the most + adjetivo para comparar una cosa con todo un grupo.",
            [
                "Short adjectives: the cheapest.",
                "Long adjectives: the most expensive.",
                "Irregular: the best, the worst.",
                "Use in or of after the superlative."
            ],
            [
                "It is the most cheap → It is the cheapest.",
                "She is the taller in the class → She is the tallest in the class.",
                "It is the best of all (correct) vs It is the better of all."
            ],
            [
                "This is the {{cheapest}} hotel in the city.",
                "It is the {{most}} beautiful beach in Spain.",
                "Tokyo is one of the busiest cities in the world.",
                "That was the best meal of my trip.",
                "This is the worst weather of the year."
            ],
            [
                "This is the most cheap hotel in the city.",
                "She is the taller in the class.",
                "It is the better restaurant of all."
            ],
            ["cheapest", "cheaper", "most", "more"],
            "Describe el mejor y el peor lugar que has visitado.",
            "The best place I have visited is Rome. It is one of the most beautiful cities in Europe. The worst place was a very crowded beach. The food there was the most expensive of my trip.",
            Skill.Speaking,
            80,
            ["the -est", "the most", "the best", "the worst"]
        ),
        new(
            "Future with going to",
            CefrLevel.A2,
            "plans",
            "Usamos am/is/are going to + verbo para planes e intenciones ya decididas.",
            [
                "I am going to travel next month.",
                "She is going to start a course.",
                "Negative: isn't/aren't going to.",
                "Question: Are you going to...?"
            ],
            [
                "I going to travel → I am going to travel.",
                "She is go to study → She is going to study.",
                "They are going to moved → They are going to move."
            ],
            [
                "I {{am}} going to visit my family.",
                "She {{is}} going to start a new job.",
                "We are going to travel in July.",
                "They aren't going to buy a car.",
                "Are you going to study tonight?"
            ],
            [
                "I going to visit my family.",
                "She is go to start a new job.",
                "They are going to moved."
            ],
            ["am", "is", "are", "be"],
            "Escribe tus planes para el próximo mes usando going to.",
            "Next month I am going to start a new English course. I am going to study three times a week. My friend is going to join me. We are going to practise speaking online. I am not going to miss any class.",
            Skill.Writing,
            80,
            ["am/is/are going to", "next month", "plan"]
        ),
        new(
            "Future with will",
            CefrLevel.A2,
            "predictions",
            "Usamos will + verbo base para predicciones, decisiones espontáneas, promesas y ofertas.",
            [
                "Affirmative: will + base verb.",
                "Negative: won't + base verb.",
                "Question: Will you...?",
                "Not used for fixed arrangements; use going to or present continuous."
            ],
            [
                "I will to help → I will help.",
                "She will comes → She will come.",
                "I am will go → I will go."
            ],
            [
                "I {{will}} help you with the report.",
                "She {{won't}} be late tomorrow.",
                "It will rain this afternoon.",
                "I promise I will call you.",
                "Will you open the window?"
            ],
            [
                "I will to help you.",
                "She will comes tomorrow.",
                "I am will go to the party."
            ],
            ["will", "won't", "would", "will to"],
            "Haz 5 predicciones sobre tu futuro y 2 promesas.",
            "I will speak English fluently one day. I will travel to Canada. I won't stop learning. I will call my family every week. Will you help me practise?",
            Skill.Speaking,
            70,
            ["will", "won't", "I promise", "I think..."]
        ),
        new(
            "Present perfect for experience",
            CefrLevel.A2,
            "work",
            "Usamos have/has + past participle para experiencias de vida sin decir cuándo ocurrieron.",
            [
                "I/You/We/They + have + past participle.",
                "He/She/It + has + past participle.",
                "Negative: haven't/hasn't + past participle.",
                "Ever/never are common: Have you ever...?"
            ],
            [
                "I have went → I have gone.",
                "She have visited → She has visited.",
                "Did you ever been? → Have you ever been?"
            ],
            [
                "I {{have}} visited London twice.",
                "She {{has}} worked in three countries.",
                "Have you ever eaten sushi?",
                "They haven't finished the project.",
                "He has never flown before."
            ],
            [
                "I have went to London.",
                "She have visited three countries.",
                "Did you ever been to Japan?"
            ],
            ["have", "has", "had", "having"],
            "Habla de 5 experiencias que has tenido o no has tenido.",
            "I have visited Paris and Rome. I have never been to Asia. I have tried Japanese food. My sister has lived in Canada. Have you ever travelled alone?",
            Skill.Speaking,
            70,
            ["have/has + past participle", "ever", "never"]
        ),
        new(
            "First conditional",
            CefrLevel.A2,
            "work",
            "Usamos if + present simple, will + verbo base para hablar de una condición probable y su resultado.",
            [
                "If + present simple, will + base verb.",
                "The if-clause can come first or second.",
                "No 'will' in the if-clause.",
                "Use unless for 'if not'."
            ],
            [
                "If I will study, I pass → If I study, I will pass.",
                "If it rains, we stay → If it rains, we will stay.",
                "I will call you if I will arrive early → I will call you if I arrive early."
            ],
            [
                "If I {{study}}, I will pass the exam.",
                "If it {{rains}}, we will stay at home.",
                "I will call you if I finish early.",
                "She will miss the train if she doesn't hurry.",
                "Unless you practise, you won't improve."
            ],
            [
                "If I will study, I pass the exam.",
                "If it rains, we stay at home.",
                "I will call you if I will finish early."
            ],
            ["study", "studies", "rains", "rain"],
            "Escribe 5 frases con first conditional sobre tus estudios o trabajo.",
            "If I study every day, I will improve my English. If I finish work early, I will go to the gym. I will call you if I need help. If it rains tomorrow, we will study online.",
            Skill.Writing,
            80,
            ["If + present", "will", "unless"]
        ),
        new(
            "Zero conditional",
            CefrLevel.A2,
            "health",
            "Usamos if + present simple, present simple para verdades generales y rutinas.",
            [
                "If + present simple, present simple.",
                "Both clauses use the present.",
                "Use when for repeated situations.",
                "Do not use will in either clause."
            ],
            [
                "If you heat water, it will boil → If you heat water, it boils.",
                "If I don't sleep, I am tired (correct) vs If I won't sleep, I am tired.",
                "When you press this button, the machine will starts → When you press this button, the machine starts."
            ],
            [
                "If you {{heat}} ice, it melts.",
                "If I {{don't}} sleep, I feel tired.",
                "When you press this button, the machine starts.",
                "Plants die if they don't get water.",
                "If you eat too much sugar, you gain weight."
            ],
            [
                "If you heat ice, it will melts.",
                "If I won't sleep, I feel tired.",
                "When you press this button, the machine will starts."
            ],
            ["heat", "heats", "don't", "doesn't"],
            "Escribe 5 verdades generales usando zero conditional.",
            "If you exercise every day, you feel better. If you don't drink water, you get tired. When I eat breakfast, I have more energy. If you sleep well, you concentrate better.",
            Skill.Writing,
            80,
            ["If + present", "present", "when"]
        ),
        new(
            "Modals and have to",
            CefrLevel.A2,
            "common",
            "Can expresa habilidad/permiso; must y have to expresan obligación; should da consejos. Have to cambia con el sujeto.",
            [
                "can + base verb.",
                "must + base verb (strong obligation).",
                "have to / has to + base verb.",
                "should + base verb (advice).",
                "don't have to = no obligation; mustn't = prohibition."
            ],
            [
                "She must to go → She must go.",
                "He have to work → He has to work.",
                "You should to rest → You should rest."
            ],
            [
                "I {{have}} to work on Saturdays.",
                "She {{should}} rest more.",
                "You must wear a seatbelt.",
                "We don't have to wear a uniform.",
                "You mustn't smoke here."
            ],
            [
                "She must to go now.",
                "He have to work today.",
                "You should to rest."
            ],
            ["have", "has", "should", "must"],
            "Habla de tus obligaciones, consejos y cosas que no tienes que hacer.",
            "I have to work from Monday to Friday. I should practise English every day. I don't have to wear a suit at work. I mustn't use my phone in meetings.",
            Skill.Speaking,
            70,
            ["have to", "has to", "should", "must", "don't have to"]
        ),
        new(
            "Too/enough and quantifiers",
            CefrLevel.A2,
            "shopping",
            "Too + adjetivo significa 'demasiado'; enough significa 'suficiente'. Usamos a few/a little, a lot of, some/any para cantidades.",
            [
                "too + adjective: too expensive.",
                "adjective + enough: cheap enough.",
                "a few + countable; a little + uncountable.",
                "a lot of + both; not enough + noun."
            ],
            [
                "This is too much expensive → This is too expensive.",
                "It isn't enough cheap → It isn't cheap enough.",
                "I have a little books → I have a few books."
            ],
            [
                "This jacket is too {{expensive}}.",
                "The shoes aren't cheap {{enough}}.",
                "I have a few questions.",
                "There is a little milk left.",
                "We don't have enough time."
            ],
            [
                "This jacket is too much expensive.",
                "The shoes aren't enough cheap.",
                "I have a little books."
            ],
            ["expensive", "enough", "much", "many"],
            "Describe una compra que no hiciste por el precio o la talla.",
            "I liked a jacket, but it was too expensive. The shoes were nice, but they weren't comfortable enough. I only had a few euros, so I didn't buy anything.",
            Skill.Writing,
            70,
            ["too + adjective", "adjective + enough", "a few", "a little"]
        ),
        new(
            "Infinitive vs gerund",
            CefrLevel.A2,
            "hobbies",
            "Algunos verbos van seguidos de -ing (enjoy, avoid, finish); otros de to + infinitive (want, decide, hope). Algunos aceptan ambos.",
            [
                "enjoy/avoid/finish + -ing.",
                "want/decide/hope/plan + to + infinitive.",
                "like/love/hate + -ing or to + infinitive.",
                "Prepositions are followed by -ing: interested in learning."
            ],
            [
                "I enjoy to read → I enjoy reading.",
                "She wants going → She wants to go.",
                "I am interested in learn → I am interested in learning."
            ],
            [
                "I enjoy {{reading}} before bed.",
                "She wants {{to}} start a new course.",
                "We decided to travel in June.",
                "He avoids driving at night.",
                "They are interested in learning Spanish."
            ],
            [
                "I enjoy to read before bed.",
                "She wants going to the party.",
                "I am interested in learn Spanish."
            ],
            ["reading", "to", "read", "learning"],
            "Escribe sobre tus hobbies y planes usando gerundios e infinitivos.",
            "I enjoy reading and listening to podcasts. I want to learn Italian next year. I avoid studying late at night. I am interested in cooking healthy food. I hope to travel more.",
            Skill.Writing,
            80,
            ["enjoy + -ing", "want + to", "interested in + -ing"]
        ),
        new(
            "Adverbs of manner and question tags",
            CefrLevel.A2,
            "common",
            "Los adverbios de manera describen cómo se hace algo (quickly, carefully). Las question tags confirman información con un mini-auxiliar al final.",
            [
                "Adjective + -ly: quick → quickly; careful → carefully.",
                "Irregular: good → well; fast → fast.",
                "Positive statement → negative tag: You are tired, aren't you?",
                "Negative statement → positive tag: She isn't here, is she?"
            ],
            [
                "She sings beautiful → She sings beautifully.",
                "He works careful → He works carefully.",
                "You are tired, aren't you? (correct) vs You are tired, are you?"
            ],
            [
                "She speaks English {{fluently}}.",
                "He drives very {{carefully}}.",
                "You are tired, aren't you?",
                "She isn't here, is she?",
                "They don't live here, do they?"
            ],
            [
                "She speaks English fluent.",
                "He drives very careful.",
                "You are tired, are you?"
            ],
            ["fluently", "fluent", "carefully", "careful"],
            "Describe cómo haces tres actividades y añade dos question tags.",
            "I speak English slowly but carefully. I cook quickly when I am hungry. My brother drives very carefully. You are learning a lot, aren't you? This lesson isn't difficult, is it?",
            Skill.Speaking,
            70,
            ["-ly", "well", "aren't you?", "isn't it?"]
        ),

        // ── B1 ──────────────────────────────────────────────────────────────
        new(
            "Present perfect vs past simple",
            CefrLevel.B1,
            "work",
            "Usamos present perfect para experiencias y resultados presentes sin tiempo concreto; past simple para acciones terminadas con un tiempo concreto.",
            [
                "Present perfect: have/has + past participle.",
                "Past simple: verb + -ed / irregular form.",
                "Use past simple with yesterday, last year, in 2020.",
                "Use present perfect with ever, never, just, already, yet, since, for."
            ],
            [
                "I have seen him yesterday → I saw him yesterday.",
                "I saw him since 2020 → I have seen him since 2020.",
                "She has worked there in 2019 → She worked there in 2019."
            ],
            [
                "I {{have}} worked here for three years.",
                "She {{saw}} the film last night.",
                "Have you ever lived abroad?",
                "We have just finished the project.",
                "They didn't come to the meeting yesterday."
            ],
            [
                "I have seen him yesterday.",
                "She has worked there in 2019.",
                "We have finished the project yesterday."
            ],
            ["have", "has", "saw", "did"],
            "Escribe sobre tu experiencia laboral distinguiendo experiencias de fechas concretas.",
            "I have worked in three companies. I started my first job in 2018. I have learned a lot since then. Last year I changed departments. I have just finished an important project.",
            Skill.Writing,
            90,
            ["have/has + past participle", "since", "for", "last year"]
        ),
        new(
            "Present perfect continuous",
            CefrLevel.B1,
            "work",
            "Usamos have/has been + verb-ing para acciones que empezaron en el pasado y continúan, o que acaban de terminar y dejan un resultado visible.",
            [
                "have/has been + -ing.",
                "Focus on duration or recent activity.",
                "Used with for, since, all day, recently.",
                "Not usually used with state verbs."
            ],
            [
                "I have been work → I have been working.",
                "She has been study → She has been studying.",
                "I have been knowing him → I have known him."
            ],
            [
                "I {{have}} been working all day.",
                "She {{has}} been studying for two hours.",
                "They have been waiting since nine.",
                "It has been raining all morning.",
                "We have been planning the trip for weeks."
            ],
            [
                "I have been work all day.",
                "She has been study for two hours.",
                "I have been knowing him for years."
            ],
            ["have", "has", "had", "having"],
            "Explica qué has estado haciendo últimamente y durante cuánto tiempo.",
            "I have been learning English for six months. I have been working on a new project since Monday. I have been exercising more recently. My sister has been living in Berlin for a year.",
            Skill.Speaking,
            80,
            ["have/has been + -ing", "for", "since", "recently"]
        ),
        new(
            "Past perfect",
            CefrLevel.B1,
            "past_events",
            "Usamos had + past participle para una acción pasada anterior a otra acción pasada.",
            [
                "had + past participle for the earlier action.",
                "Past simple for the later action.",
                "Common with before, after, already, by the time.",
                "Negative: hadn't + past participle."
            ],
            [
                "When I arrived, the film started → When I arrived, the film had started.",
                "She had went → She had gone.",
                "I didn't had time → I hadn't had time."
            ],
            [
                "When I arrived, the film {{had}} started.",
                "She {{hadn't}} finished the report before the meeting.",
                "By the time we got there, they had left.",
                "I had already eaten when he called.",
                "He had never seen the sea before that trip."
            ],
            [
                "When I arrived, the film started.",
                "She had went home before I called.",
                "I didn't had time to finish."
            ],
            ["had", "hadn't", "has", "didn't"],
            "Cuenta una anécdota donde una acción ocurrió antes que otra.",
            "Last year I arrived late to the airport. The plane had already left. I had forgotten my passport at home. By the time I returned, the next flight had been cancelled.",
            Skill.Writing,
            90,
            ["had + past participle", "by the time", "already"]
        ),
        new(
            "Future continuous and future perfect",
            CefrLevel.B1,
            "travel",
            "Usamos will be + -ing para una acción en progreso en el futuro y will have + past participle para una acción completada antes de un momento futuro.",
            [
                "Future continuous: will be + -ing.",
                "Future perfect: will have + past participle.",
                "Common with by, at, this time next week.",
                "Negative: won't be / won't have."
            ],
            [
                "At 8, I will working → At 8, I will be working.",
                "By Friday, I will finished → By Friday, I will have finished.",
                "This time next week, we will be lie on the beach → This time next week, we will be lying on the beach."
            ],
            [
                "At eight, I {{will}} be working.",
                "By Friday, she {{will}} have finished the course.",
                "This time next week, we will be flying to Lisbon.",
                "They won't have arrived by noon.",
                "Will you be using the car tonight?"
            ],
            [
                "At eight, I will working.",
                "By Friday, she will finished the course.",
                "This time next week, we will be lie on the beach."
            ],
            ["will", "won't", "would", "have"],
            "Escribe tus planes para la próxima semana usando future continuous y future perfect.",
            "This time next week, I will be travelling to Portugal. By Friday, I will have finished my report. I will be staying in a small hotel. I won't have visited Lisbon before.",
            Skill.Writing,
            90,
            ["will be + -ing", "will have + past participle", "by"]
        ),
        new(
            "Second conditional",
            CefrLevel.B1,
            "opinions",
            "Usamos if + past simple, would + verbo base para situaciones hipotéticas o improbables en el presente/futuro.",
            [
                "If + past simple, would + base verb.",
                "No 'would' in the if-clause.",
                "Use were with I/he/she/it in formal English.",
                "Would can be contracted to 'd."
            ],
            [
                "If I would have money, I would travel → If I had money, I would travel.",
                "If she was here, she will help → If she were here, she would help.",
                "I would travel if I will have time → I would travel if I had time."
            ],
            [
                "If I {{had}} more time, I would learn Italian.",
                "She {{would}} travel more if she could.",
                "If I were you, I would apply for the job.",
                "We would move abroad if we found a good opportunity.",
                "What would you do if you won the lottery?"
            ],
            [
                "If I would have more time, I would learn Italian.",
                "If she was here, she will help us.",
                "I would travel if I will have time."
            ],
            ["had", "would", "have", "will"],
            "Responde qué harías en tres situaciones hipotéticas.",
            "If I had more free time, I would learn to play the guitar. If I lived near the sea, I would swim every day. If I were you, I would start practising English now.",
            Skill.Speaking,
            80,
            ["If + past simple", "would + verb", "If I were you"]
        ),
        new(
            "Third conditional",
            CefrLevel.B1,
            "opinions",
            "Usamos if + past perfect, would have + past participle para imaginar un pasado diferente.",
            [
                "If + had + past participle, would have + past participle.",
                "It describes an unreal past situation.",
                "No 'would have' in the if-clause.",
                "Could have / might have are also possible."
            ],
            [
                "If I would have studied, I would have passed → If I had studied, I would have passed.",
                "If she had known, she would told me → If she had known, she would have told me.",
                "I would have came if you invited me → I would have come if you had invited me."
            ],
            [
                "If I {{had}} studied, I would have passed.",
                "She {{would}} have come if you had invited her.",
                "If we had left earlier, we wouldn't have missed the train.",
                "I could have helped if you had asked me.",
                "They would have bought the house if it had been cheaper."
            ],
            [
                "If I would have studied, I would have passed.",
                "If she had known, she would told me.",
                "I would have came if you invited me."
            ],
            ["had", "would", "have", "will"],
            "Imagina cómo habría sido diferente una situación de tu pasado.",
            "If I had started learning English earlier, I would have had more opportunities. If I hadn't moved to Madrid, I wouldn't have met my best friend. I could have studied abroad if I had applied.",
            Skill.Speaking,
            80,
            ["If + had + past participle", "would have + past participle", "could have"]
        ),
        new(
            "Passive voice",
            CefrLevel.B1,
            "work",
            "Usamos be + past participle cuando el foco está en la acción o el resultado, no en quién la realiza.",
            [
                "Present simple passive: am/is/are + past participle.",
                "Past simple passive: was/were + past participle.",
                "Agent with by when necessary.",
                "Questions: Was the report sent?"
            ],
            [
                "The email was send → The email was sent.",
                "The reports are wrote → The reports are written.",
                "It was did yesterday → It was done yesterday."
            ],
            [
                "The email {{was}} sent yesterday.",
                "The reports {{are}} written every month.",
                "The meeting was cancelled because of the storm.",
                "This software is used by thousands of people.",
                "The project will be finished next week."
            ],
            [
                "The email was send yesterday.",
                "The reports are wrote every month.",
                "It was did yesterday."
            ],
            ["was", "were", "are", "is"],
            "Describe un proceso o noticia usando la voz pasiva.",
            "The report was written by the team. The results were checked by the manager. New employees are trained online. The new office will be opened next month.",
            Skill.Writing,
            90,
            ["be + past participle", "was/were", "is/are", "by"]
        ),
        new(
            "Reported speech and questions",
            CefrLevel.B1,
            "work",
            "Al reportar palabras, cambiamos pronombres y tiempos hacia atrás. Las preguntas reportadas usan el orden de una afirmación.",
            [
                "say + that clause; tell + person + that clause.",
                "Backshift: present → past; will → would; can → could.",
                "Reported question: asked + if/whether + subject + verb.",
                "No auxiliary inversion in reported questions."
            ],
            [
                "She said she is busy → She said she was busy.",
                "He asked where was I → He asked where I was.",
                "She told that she would come → She told me that she would come."
            ],
            [
                "She {{said}} she was busy.",
                "He {{asked}} if I could help.",
                "They told us they would arrive late.",
                "She asked where I lived.",
                "He said he had finished the report."
            ],
            [
                "She said she is busy.",
                "He asked where was I.",
                "She told that she would come."
            ],
            ["said", "asked", "told", "if"],
            "Reporta una conversación que tuviste en el trabajo.",
            "My manager said the project was urgent. She asked if I could work late. I told her I had another meeting. She said she would send the details by email.",
            Skill.Writing,
            90,
            ["said (that)", "told me (that)", "asked if", "asked where"]
        ),
        new(
            "Relative clauses",
            CefrLevel.B1,
            "common",
            "Usamos who para personas, which para cosas y that para ambos. Defining clauses identifican; non-defining clauses añaden información extra entre comas.",
            [
                "who + person; which + thing; that + person/thing.",
                "Defining: The book that I bought is good.",
                "Non-defining: The book, which I bought, is good.",
                "Do not use that in non-defining clauses."
            ],
            [
                "The man which called is my boss → The man who called is my boss.",
                "The email who I sent was long → The email that I sent was long.",
                "My brother, that lives in Paris, is a doctor → My brother, who lives in Paris, is a doctor."
            ],
            [
                "The man {{who}} called is my boss.",
                "The book {{which}} I bought is excellent.",
                "The email that I sent was very long.",
                "My sister, who lives in Berlin, is a designer.",
                "This is the restaurant where we met."
            ],
            [
                "The man which called is my boss.",
                "The email who I sent was long.",
                "My brother, that lives in Paris, is a doctor."
            ],
            ["who", "which", "that", "where"],
            "Describe a persona, un lugar y una cosa importantes para ti usando relative clauses.",
            "The person who helped me most is my mother. The city where I grew up is small. The book that changed my life is a novel. My friend, who lives in London, is learning Spanish.",
            Skill.Writing,
            90,
            ["who", "which", "that", "where", "whose"]
        ),
        new(
            "Modals of deduction",
            CefrLevel.B1,
            "opinions",
            "Usamos must, might/may/could y can't para deducir o especular sobre el presente.",
            [
                "must + base verb = very certain.",
                "might/may/could + base verb = possible.",
                "can't + base verb = impossible.",
                "mustn't does not mean 'not possible'; it means prohibition."
            ],
            [
                "He must be tired (correct) vs He musts be tired.",
                "She can't be at home, she just left (correct).",
                "It might raining → It might be raining."
            ],
            [
                "She {{must}} be tired after the trip.",
                "He {{might}} be at the office.",
                "They can't be at home because their car is gone.",
                "This could be the wrong address.",
                "You must be joking!"
            ],
            [
                "She musts be tired after the trip.",
                "He might be at the office.",
                "It might raining outside."
            ],
            ["must", "might", "can't", "could"],
            "Haz deducciones sobre una situación cotidiana.",
            "My neighbour's lights are off, so she must be away. Her car is still outside, so she might be at home. The dog is barking, so someone could be at the door. It can't be the postman because it's Sunday.",
            Skill.Speaking,
            80,
            ["must be", "might be", "could be", "can't be"]
        ),
        new(
            "Gerunds, infinitives and purpose",
            CefrLevel.B1,
            "hobbies",
            "Usamos -ing después de preposiciones y ciertos verbos; to + infinitive para propósito y después de otros verbos.",
            [
                "Preposition + -ing: interested in learning.",
                "Purpose: to / in order to / so as to + infinitive.",
                "Negative purpose: so as not to / in order not to.",
                "Some verbs change meaning: stop doing vs stop to do."
            ],
            [
                "I am interested in learn → I am interested in learning.",
                "I went to the shop for buy milk → I went to the shop to buy milk.",
                "I study for pass → I study to pass."
            ],
            [
                "I am interested in {{learning}} English.",
                "She went out {{to}} buy some bread.",
                "We practise speaking every day to improve.",
                "He took a course in order to change careers.",
                "I left early so as not to miss the train."
            ],
            [
                "I am interested in learn English.",
                "I went to the shop for buy milk.",
                "I study every day for pass the exam."
            ],
            ["learning", "to", "for", "so as"],
            "Explica por qué haces tres actividades y qué te interesa aprender.",
            "I study English to improve my career. I am interested in learning about other cultures. I practise listening in order to understand films. I take notes so as not to forget new words.",
            Skill.Writing,
            90,
            ["to + infinitive", "in order to", "so as to", "interested in + -ing"]
        ),
        new(
            "Contrast clauses",
            CefrLevel.B1,
            "opinions",
            "Usamos although/even though + clause, despite/in spite of + noun/-ing, y however para contrastar ideas.",
            [
                "although / even though + subject + verb.",
                "despite / in spite of + noun or -ing.",
                "however + comma, at the start of a sentence.",
                "while can contrast two clauses."
            ],
            [
                "Although the rain, we went out → Although it was raining, we went out.",
                "Despite of the rain → Despite the rain.",
                "Although he studied, but he failed → Although he studied, he failed."
            ],
            [
                "{{Although}} it was raining, we went for a walk.",
                "Despite {{being}} tired, she finished the report.",
                "Even though the price was high, they bought the car.",
                "In spite of the traffic, we arrived on time.",
                "I like the job. However, the hours are long."
            ],
            [
                "Although the rain, we went for a walk.",
                "Despite of being tired, she finished the report.",
                "Although he studied, but he failed."
            ],
            ["Although", "Despite", "Even though", "However"],
            "Escribe sobre una decisión con ventajas y desventajas usando contraste.",
            "Although the job is difficult, I enjoy it. Despite the long hours, I have learned a lot. Even though the salary is not perfect, the team is great. However, I would like more holidays.",
            Skill.Writing,
            90,
            ["although", "despite", "even though", "however"]
        ),
        new(
            "Causatives",
            CefrLevel.B1,
            "work",
            "Usamos have/get + object + past participle cuando otra persona hace algo por nosotros.",
            [
                "have + object + past participle.",
                "get + object + past participle (more informal).",
                "Also: have someone do something.",
                "Questions: Where did you have your hair cut?"
            ],
            [
                "I cut my hair yesterday (you did it yourself) vs I had my hair cut.",
                "I had my car repair → I had my car repaired.",
                "She got her house paint → She got her house painted."
            ],
            [
                "I {{had}} my car repaired last week.",
                "She {{got}} her hair cut yesterday.",
                "We had the kitchen painted last month.",
                "He gets his suits made in Italy.",
                "Where did you have your photos printed?"
            ],
            [
                "I had my car repair last week.",
                "She got her hair cut yesterday.",
                "We had the kitchen paint last month."
            ],
            ["had", "got", "have", "get"],
            "Explica tres cosas que hiciste hacer por otra persona.",
            "Last month I had my car repaired. I got my hair cut on Friday. We had our kitchen painted last year. I also had my laptop fixed at a local shop.",
            Skill.Writing,
            80,
            ["have/get + object + past participle", "had", "got"]
        ),
        new(
            "Wish/if only",
            CefrLevel.B1,
            "opinions",
            "Usamos wish/if only + past simple para lamentar el presente y wish + past perfect para lamentar el pasado.",
            [
                "wish + past simple: present regret.",
                "wish + past perfect: past regret.",
                "wish + would: annoyance or desired change.",
                "if only adds emphasis."
            ],
            [
                "I wish I have more time → I wish I had more time.",
                "I wish I studied yesterday → I wish I had studied yesterday.",
                "I wish he stops calling → I wish he would stop calling."
            ],
            [
                "I {{wish}} I had more free time.",
                "If only I {{had}} studied harder.",
                "I wish I could speak French.",
                "She wishes she hadn't left her job.",
                "I wish it would stop raining."
            ],
            [
                "I wish I have more free time.",
                "I wish I studied harder yesterday.",
                "I wish he stops calling me."
            ],
            ["wish", "had", "could", "would"],
            "Escribe tres deseos sobre tu presente y dos lamentos sobre el pasado.",
            "I wish I had more time to travel. I wish I could play the piano. If only I had started learning English earlier. I wish I hadn't missed that opportunity.",
            Skill.Writing,
            80,
            ["wish + past", "wish + past perfect", "if only", "would"]
        ),
        new(
            "Used to and would",
            CefrLevel.B1,
            "past_events",
            "Usamos used to + infinitive para hábitos y estados pasados que ya no son verdad. Would + infinitive describe hábitos pasados, pero no estados.",
            [
                "used to + base verb: past habit or state.",
                "Negative: didn't use to + base verb.",
                "Question: Did you use to...?",
                "would + base verb for repeated past actions, not states."
            ],
            [
                "I use to play tennis → I used to play tennis.",
                "I didn't used to like coffee → I didn't use to like coffee.",
                "I would have a dog when I was a child → I used to have a dog."
            ],
            [
                "I {{used}} to play football every weekend.",
                "She {{didn't}} use to like coffee.",
                "We would visit our grandparents every summer.",
                "Did you use to live in the city?",
                "He used to be a teacher."
            ],
            [
                "I use to play football every weekend.",
                "She didn't used to like coffee.",
                "I would have a dog when I was a child."
            ],
            ["used", "didn't", "would", "use"],
            "Compara tu vida de niño con tu vida ahora usando used to/would.",
            "I used to live in a small town. I would play football every afternoon. I didn't use to study much. Now I live in a big city and I study English every day.",
            Skill.Speaking,
            80,
            ["used to", "didn't use to", "would", "Did you use to...?"]
        )
    ];
}
