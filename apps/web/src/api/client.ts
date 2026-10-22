import axios from "axios";
import { getAuthToken } from "@/lib/authToken";

const api = axios.create({
  baseURL: "/api",
  headers: { "Content-Type": "application/json" },
});

api.interceptors.request.use((config) => {
  const token = getAuthToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

export default api;

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  currentLevel: string;
  currentSubLevel: string;
  placementCompleted: boolean;
}

export interface SkillProgress {
  skill: string;
  mastered: number;
  total: number;
  percent: number;
}

export interface ProgressStats {
  wordsMastered: number;
  wordsDueToday: number;
  totalWordsStudied: number;
  streak: number;
  currentLevel: string;
  currentSubLevel: string;
  a1MasteryPercent: number;
  totalPracticeMinutes: number;
  grammarMastered: number;
  readingCompleted: number;
  listeningCompleted: number;
  speakingCompleted: number;
  writingCompleted: number;
  skills: SkillProgress[];
}

export interface ReviewItem {
  itemId: string;
  word: string;
  translation: string;
  partOfSpeech: string;
  category: string;
  exampleSentences: string[];
  repetitions: number;
  nextReviewDate: string;
}

export interface FillBlankContent {
  sentence: string;
  blank: string;
  options: string[];
  correctIndex: number;
}

export interface MultipleChoiceContent {
  question: string;
  options: string[];
  correctIndex: number;
}

export interface WordOrderContent {
  words: string[];
  correctOrder: string[];
}

export type ExerciseContent = FillBlankContent | MultipleChoiceContent | WordOrderContent;

export interface ExerciseDue {
  id: string;
  type: string;
  content: ExerciseContent;
  cefrLevel: string;
  skill: string;
  category: string;
  repetitions: number;
  nextReviewDate: string;
}

export interface LessonSummary {
  id: string;
  title: string;
  description: string;
  cefrLevel: string;
  category: string;
  theme: string;
  sequence: number;
  exerciseCount: number;
  isLocked: boolean;
}

export interface GrammarRule {
  id: string;
  title: string;
  explanation: string;
  cefrLevel: string;
  category: string;
  examples: string[];
  relatedStructures: string[];
}

export interface LessonDetail {
  id: string;
  title: string;
  description: string;
  cefrLevel: string;
  category: string;
  theme: string;
  sequence: number;
  grammarRules: GrammarRule[];
  exercises: { id: string; type: string; content: ExerciseContent; category: string }[];
}

export interface ReadingSummary {
  id: string;
  title: string;
  cefrLevel: string;
  wordCount: number;
  attribution: string;
}

export interface ComprehensionQuestion {
  id: string;
  question: string;
  options: string[];
}

export interface ReadingDetail extends ReadingSummary {
  body: string;
  source: string;
  questions: ComprehensionQuestion[];
}

export interface ListeningSummary {
  id: string;
  title: string;
  cefrLevel: string;
  category: string;
}

export interface ListeningDetail {
  id: string;
  title: string;
  transcript: string;
  cefrLevel: string;
  category: string;
  questions: ComprehensionQuestion[];
}

export interface SpeakingPrompt {
  id: string;
  prompt: string;
  cefrLevel: string;
  category: string;
  modelAnswer: string;
  keyPhrases: string[];
}

export interface WritingPrompt {
  id: string;
  prompt: string;
  cefrLevel: string;
  category: string;
  wordLimit: number;
}

export interface PlacementQuestion {
  exerciseId: string;
  type: string;
  content: ExerciseContent;
  cefrLevel: string;
}

export interface DailySession {
  vocabularyReviews: ReviewItem[];
  grammarReviews: ExerciseDue[];
  recommendedLessonId: string | null;
  recommendedLessonTitle: string | null;
  estimatedMinutes: number;
}
