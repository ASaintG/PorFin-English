"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import api, { type DailySession, type ExerciseDue, type GrammarRule, type ReviewItem } from "@/api/client";
import { ExerciseRenderer } from "@/components/exercises/ExerciseRenderer";
import { Flashcard } from "@/components/exercises/Flashcard";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";
import {
  cacheReviewSession,
  enqueueReview,
  getCachedReviewSession,
  getPendingReviews,
  removePendingReview,
} from "@/lib/offlineDb";

type SessionItem =
  | { kind: "vocab"; data: ReviewItem }
  | { kind: "grammar"; data: ExerciseDue };

const LEVELS = ["A1", "A2", "B1"] as const;

function formatCategory(category: string) {
  return category.replace(/_/g, " ").replace(/\b\w/g, (letter) => letter.toUpperCase());
}

async function submitVocabReview(itemId: string, quality: number): Promise<boolean> {
  try {
    await api.post(`/reviews/${itemId}`, { quality });
    return true;
  } catch {
    await enqueueReview(itemId, quality);
    return false;
  }
}

async function submitGrammarReview(itemId: string, correct: boolean): Promise<void> {
  await api.post(`/exercises/${itemId}/submit`, { quality: correct ? 5 : 2, correct });
}

async function syncPendingReviews(): Promise<void> {
  const pending = await getPendingReviews();
  if (pending.length === 0) return;

  try {
    const { data } = await api.post<{ synced: number; failed: string[] }>("/reviews/batch", {
      reviews: pending.map((e) => ({ itemId: e.itemId, quality: e.quality })),
    });
    for (const entry of pending) {
      if (!data.failed.includes(entry.itemId)) {
        await removePendingReview(entry.id);
      }
    }
  } catch {
    for (const entry of pending) {
      try {
        await api.post(`/reviews/${entry.itemId}`, { quality: entry.quality });
        await removePendingReview(entry.id);
      } catch {
        break;
      }
    }
  }
}

type ReviewSessionResult = {
  session: SessionItem[];
  lessonLink: { id: string; title: string } | null;
  error: string;
};

async function fetchReviewSession(level: string, category: string): Promise<ReviewSessionResult> {
  try {
    await syncPendingReviews();
    const params = new URLSearchParams({ cefrLevel: level });
    if (category) params.set("category", category);
    const { data } = await api.get<DailySession>(`/daily/session?${params.toString()}`);
    const session: SessionItem[] = [
      ...data.vocabularyReviews.map((v) => ({ kind: "vocab" as const, data: v })),
      ...data.grammarReviews.map((g) => ({ kind: "grammar" as const, data: g })),
    ];
    await cacheReviewSession(data.vocabularyReviews);
    return {
      session,
      lessonLink:
        data.recommendedLessonId && data.recommendedLessonTitle
          ? { id: data.recommendedLessonId, title: data.recommendedLessonTitle }
          : null,
      error: "",
    };
  } catch {
    const cached = await getCachedReviewSession();
    if (cached?.items?.length) {
      return {
        session: (cached.items as ReviewItem[]).map((v) => ({ kind: "vocab" as const, data: v })),
        lessonLink: null,
        error: "Sin conexión — usando sesión guardada.",
      };
    }
    return { session: [], lessonLink: null, error: "No se pudo cargar el repaso." };
  }
}

export default function ReviewPage() {
  const [items, setItems] = useState<SessionItem[]>([]);
  const [lessonLink, setLessonLink] = useState<{ id: string; title: string } | null>(null);
  const [index, setIndex] = useState(0);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [completed, setCompleted] = useState(false);
  const [offlineQueued, setOfflineQueued] = useState(0);
  const [error, setError] = useState("");
  const [level, setLevel] = useState("A1");
  const [category, setCategory] = useState("");
  const [categories, setCategories] = useState<string[]>([]);

  useEffect(() => {
    api.get<GrammarRule[]>(`/grammar?cefrLevel=${level}`)
      .then(({ data }) => {
        const extra = level === "B1" ? ["environment", "technology"] : [];
        const next = Array.from(new Set([...data.map((rule) => rule.category), ...extra])).sort();
        setCategories(next);
        setCategory((current) => (current && !next.includes(current) ? "" : current));
      })
      .catch(() => setCategories([]));
  }, [level]);

  const loadReviews = useCallback(() => {
    void fetchReviewSession(level, category).then((result) => {
      setItems(result.session);
      setIndex(0);
      setCompleted(result.session.length === 0);
      setLessonLink(result.lessonLink);
      setError(result.error);
      setLoading(false);
    });
  }, [level, category]);

  useEffect(() => {
    void loadReviews();
  }, [loadReviews]);

  useEffect(() => {
    const handleOnline = () => {
      setLoading(true);
      void loadReviews();
    };
    window.addEventListener("online", handleOnline);
    return () => window.removeEventListener("online", handleOnline);
  }, [loadReviews]);

  const changeLevel = (value: string) => {
    setLoading(true);
    setLevel(value);
  };

  const changeCategory = (value: string) => {
    setLoading(true);
    setCategory(value);
  };

  const current = items[index];

  const handleVocabAnswer = async (quality: number) => {
    if (!current || current.kind !== "vocab") return;
    setSubmitting(true);
    const synced = await submitVocabReview(current.data.itemId, quality);
    if (!synced) setOfflineQueued((n) => n + 1);
    advance();
    setSubmitting(false);
  };

  const handleGrammarAnswer = async (correct: boolean) => {
    if (!current || current.kind !== "grammar") return;
    setSubmitting(true);
    await submitGrammarReview(current.data.id, correct);
    advance();
    setSubmitting(false);
  };

  const advance = () => {
    const nextIndex = index + 1;
    if (nextIndex >= items.length) setCompleted(true);
    else setIndex(nextIndex);
  };

  const filters = (
    <div className="mb-6 w-full max-w-lg">
      <div className="flex flex-wrap items-center gap-2">
        <div className="flex rounded-lg border border-border p-1">
          {LEVELS.map((value) => (
            <button
              key={value}
              type="button"
              onClick={() => changeLevel(value)}
              className={`rounded-md px-3 py-1 text-sm transition-colors ${
                level === value ? "bg-primary text-primary-foreground" : "text-muted-foreground hover:bg-muted"
              }`}
            >
              {value}
            </button>
          ))}
        </div>
        <select
          value={category}
          onChange={(event) => changeCategory(event.target.value)}
          className="rounded-lg border border-border bg-background px-3 py-1.5 text-sm"
          aria-label="Filtrar por tema"
        >
          <option value="">Todos los temas</option>
          {categories.map((value) => (
            <option key={value} value={value}>
              {formatCategory(value)}
            </option>
          ))}
        </select>
      </div>
    </div>
  );

  if (loading) {
    return (
      <>
        <AppHeader />
        <PageContainer className="flex flex-1 items-center justify-center">
          <p className="text-muted-foreground">Cargando sesión diaria...</p>
        </PageContainer>
      </>
    );
  }

  if (completed || !current) {
    return (
      <>
        <AppHeader />
        <PageContainer className="flex flex-1 flex-col items-center justify-center text-center">
          <div className="mx-auto w-full max-w-lg">
            <h1 className="mb-2 text-2xl font-semibold">¡Sesión completada!</h1>
            <p className="mb-4 text-muted-foreground">
              {items.length > 0
                ? "Buen trabajo. Vocabulario y gramática actualizados con SM-2."
                : "No hay ítems pendientes para este nivel y tema."}
            </p>
            {filters}
            {lessonLink && (
              <Link href={`/lessons/${lessonLink.id}`} className="mb-4 block text-sm text-primary underline">
                Continuar con: {lessonLink.title}
              </Link>
            )}
            {offlineQueued > 0 && (
              <p className="mb-4 text-sm text-warning">
                {offlineQueued} respuesta(s) en cola — se sincronizarán al reconectar.
              </p>
            )}
            <div className="flex justify-center gap-3">
              <Link href="/dashboard" className="rounded-lg bg-primary px-5 py-2.5 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary-hover">
                Dashboard
              </Link>
              <button type="button" onClick={() => { setLoading(true); loadReviews(); }} className="rounded-lg border border-border px-5 py-2.5 text-sm transition-colors hover:bg-muted">
                Otra ronda
              </button>
            </div>
          </div>
        </PageContainer>
      </>
    );
  }

  return (
    <>
      <AppHeader />
      <PageContainer className="flex flex-1 flex-col items-center">
        {filters}
        <div className="mb-6 w-full max-w-lg">
          <div className="mb-2 flex justify-between text-sm text-muted-foreground">
            <span>
              {current.kind === "vocab" ? "Vocabulario" : "Gramática"} · {index + 1} de {items.length}
            </span>
            {error && <span className="text-warning">{error}</span>}
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-progress-track">
            <div className="h-full rounded-full bg-success transition-all" style={{ width: `${((index + 1) / items.length) * 100}%` }} />
          </div>
        </div>

        {current.kind === "vocab" ? (
          <Flashcard
            key={current.data.itemId}
            word={current.data.word}
            translation={current.data.translation}
            partOfSpeech={current.data.partOfSpeech}
            category={current.data.category}
            exampleSentences={current.data.exampleSentences}
            disabled={submitting}
            onKnow={() => handleVocabAnswer(5)}
            onDontKnow={() => handleVocabAnswer(2)}
          />
        ) : (
          <ExerciseRenderer
            key={current.data.id}
            type={current.data.type}
            content={current.data.content}
            disabled={submitting}
            onSubmit={handleGrammarAnswer}
          />
        )}
      </PageContainer>
    </>
  );
}
