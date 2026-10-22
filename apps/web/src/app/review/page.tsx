"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type DailySession, type ExerciseDue, type ReviewItem } from "@/api/client";
import { FillBlank } from "@/components/exercises/FillBlank";
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

async function fetchReviewSession(): Promise<ReviewSessionResult> {
  try {
    await syncPendingReviews();
    const { data } = await api.get<DailySession>("/daily/session");
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

  const loadReviews = () => {
    setLoading(true);
    void fetchReviewSession().then((result) => {
      setItems(result.session);
      setIndex(0);
      setCompleted(result.session.length === 0);
      setLessonLink(result.lessonLink);
      setError(result.error);
      setLoading(false);
    });
  };

  useEffect(() => {
    let cancelled = false;
    void fetchReviewSession().then((result) => {
      if (cancelled) return;
      setItems(result.session);
      setIndex(0);
      setCompleted(result.session.length === 0);
      setLessonLink(result.lessonLink);
      setError(result.error);
      setLoading(false);
    });
    const handleOnline = () => {
      setLoading(true);
      void fetchReviewSession().then((result) => {
        if (cancelled) return;
        setItems(result.session);
        setIndex(0);
        setCompleted(result.session.length === 0);
        setLessonLink(result.lessonLink);
        setError(result.error);
        setLoading(false);
      });
    };
    window.addEventListener("online", handleOnline);
    return () => {
      cancelled = true;
      window.removeEventListener("online", handleOnline);
    };
  }, []);

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
            <p className="mb-2 text-muted-foreground">
              {items.length > 0
                ? "Buen trabajo. Vocabulario y gramática actualizados con SM-2."
                : "No hay ítems pendientes por ahora."}
            </p>
            {lessonLink && (
              <Link href={`/lessons/${lessonLink.id}`} className="mb-4 text-sm text-primary underline">
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
              <button type="button" onClick={loadReviews} className="rounded-lg border border-border px-5 py-2.5 text-sm transition-colors hover:bg-muted">
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
          <FillBlank
            key={current.data.id}
            content={current.data.content}
            disabled={submitting}
            onSubmit={handleGrammarAnswer}
          />
        )}
      </PageContainer>
    </>
  );
}
