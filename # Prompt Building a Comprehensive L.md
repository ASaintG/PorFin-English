# Prompt: Building a Comprehensive Language Learning App (A1 to B2)

You are an expert full‑stack developer and language‑learning product designer. Your task is to build a complete, free web application that teaches English from A1 to B2 according to the CEFR framework, using **React (frontend), .NET Core Web API (backend), and MongoDB (database).** The app must offer real progress tracking, avoid gamification emptiness, and rely only on free, legally usable content. Below is the full specification, methodology, and technical architecture to achieve it.

## 1. Learning Methodology
The app must implement proven pedagogical principles:

- **Comprehensible Input (Krashen):** expose the user to texts/audio they understand ~90%, with 10% new material.
- **Spaced Repetition (SRS):** implement SM‑2 algorithm for vocabulary; difficult items appear more frequently.
- **Four skills:** reading, writing, listening, speaking – each CEFR “can‑do” descriptor becomes concrete exercises.
- **Grammar in context:** rules introduced through real examples, not isolated theory.
- **Measurable progress:** break CEFR levels into sub‑levels (A1.1, A1.2 … B2.2), each tied to specific vocabulary, grammar structures, and communicative situations.

Example A1 goal: “I can introduce myself, order food, understand simple directions.” Exercises must cover exactly those scenarios.

## 2. Free & Legal Content Sources
All content must be free and legally obtained:

- **Vocabulary & example sentences:**
  - CEFR‑graded frequency lists (e.g., open‑source CEFR Vocabulary Project).
  - Tatoeba (tatoeba.org) – CC BY 2.0 sentence pairs English‑Spanish; filter by length/vocabulary for difficulty.
- **Audio:**
  - Use browser’s Web Speech API (SpeechSynthesis) – Google voices are high quality (Chrome/Safari). No pre‑recorded files needed.
  - For speaking exercises: use SpeechRecognition API (Chrome/Edge).
- **Reading texts:**
  - Public domain books from Project Gutenberg and Librivox (adapt short excerpts by level).
  - Create original simplified texts using procedural generation or free AI (if budget permits later).
- **Exercises:** auto‑generate from templates (fill‑in‑blanks, multiple choice, ordering words, matching words to emojis/icons). Generate distractors from same‑category vocabulary.

Always attribute CC‑BY content appropriately.

## 3. Technical Architecture

### Frontend (React)
- SPA with React Router.
- State management: Context API or Zustand.
- Reusable exercise components: `<MultipleChoice />`, `<FillBlank />`, `<SpeechPractice />`, etc.
- Communicate with backend via REST API (fetch/axios).
- Store offline progress in `localStorage` / IndexedDB; sync on connection.
- Use Web Speech API for TTS and STT.

### Backend (.NET Core Web API)
- Services: UserService, LessonService, ProgressService, SpacedRepetitionService.
- RESTful controllers.
- Authentication: JWT (ASP.NET Core Identity or custom).
- SM‑2 algorithm for scheduling reviews.

### Database (MongoDB)
Collections (flexible document structure):

- `users`: profile, current_level, streak, last_active.
- `vocabularyItems`: word, translation, partOfSpeech, cefrLevel, exampleSentences, audioUrl (optional).
- `grammarRules`: title, explanation, cefrLevel, examples.
- `exercises`: type, content (JSON), cefrLevel, skill (reading/writing/…), difficulty.
- `lessons`: title, description, cefrLevel, sequence, array_of_exercise_ids.
- `userProgress`: userId, itemId, itemType, easeFactor, interval, nextReviewDate, repetitions, correctCount.

## 4. Core Features & UX Design
The app must feel like a personal tutor, not a game. Include light motivational elements (streaks, daily goals, milestone celebrations) only.

- **Placement test:** adaptive test (A1–B2 exercises) to set the starting point.
- **Learning Path:** CEFR‑based journey divided into thematic units (e.g., “At the restaurant” for A1). Each unit blends vocabulary, grammar, listening, reading, speaking.
- **Daily Challenge:** 15‑minute session mixing spaced repetition reviews and new content, prioritizing items due for review.
- **Progress Dashboard:**
  - Percentage mastery per skill and level.
  - Vocabulary count mastered per level.
  - Streak & hours practiced.
  - Certification exam simulator (Cambridge‑style) so user feels they are progressing towards a concrete goal.
- **Minimal community (optional):** users report errors or suggest sentences; moderated enrichment.

## 5. Development Roadmap (Incremental but Fully Functional)

### Phase 1 – Smart Vocabulary (2‑3 months)
- User auth.
- Seed 500 A1 words (with translation, category, example).
- Implement SM‑2 algorithm.
- Flashcard interface with TTS + “know/don’t know” buttons.
- Basic stats of learned words.

### Phase 2 – Grammar Exercises (2 months)
- Add grammar rules with examples.
- Auto‑generate fill‑in‑blank exercises from rules.
- Expand to A2 content.

### Phase 3 – Reading & Listening Comprehension (2‑3 months)
- Short readings with comprehension questions.
- TTS‑generated audio with synchronized text (highlighting).
- Listening exercises (answer questions after audio).

### Phase 4 – Speaking & Writing (3 months)
- Repetition exercises (speech recognition) with simple feedback.
- Guided writing: complete open‑ended prompts, show model answer.

### Phase 5 – B1 & B2 Expansion (ongoing)
- Continuously add content, refine difficulty.

## 6. Important Considerations
- **Stick to official CEFR descriptors:** convert each “can do statement” into exercise types.
- **Client‑side caching:** use IndexedDB to store exercises for offline practice, sync to MongoDB later.
- **SM‑2 simplicity:** only four variables per user‑item pair (ease, interval, repetitions, next date). No machine learning needed.
- **Adaptive tests:** use item response theory or simple rule “if 3/5 correct, move up sub‑level”.
- **TTS quality:** detect best available browser voices; on mobile, Chrome voices are usually good.
- **Community sourcing:** later, allow supervised contributions to multiply content without manual curation.

## 7. Personal Benefit
While building the app, your own English will improve dramatically because you will be deep‑diving into vocabulary lists, grammar rules, and audio transcription. Document your journey (build in public) for extra motivation.

Use this document as the full prompt to design and implement the application. Start by choosing a specific phase to elaborate further.