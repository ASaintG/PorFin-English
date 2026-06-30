"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import api, { type DailySession, type ExerciseDue, type ReviewItem } from "@/api/client";
import { FillBlank } from "@/components/exercises/FillBlank";
import { Flashcard } from "@/components/exercises/Flashcard";
import { AppHeader } from "@/components/layout/AppHeader";
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

export default function ReviewPage() {
  const [items, setItems] = useState<SessionItem[]>([]);
  const [lessonLink, setLessonLink] = useState<{ id: string; title: string } | null>(null);
  const [index, setIndex] = useState(0);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [completed, setCompleted] = useState(false);
  const [offlineQueued, setOfflineQueued] = useState(0);
  const [error, setError] = useState("");

  const loadReviews = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      await syncPendingReviews();
      const { data } = await api.get<DailySession>("/daily/session");
      const session: SessionItem[] = [
        ...data.vocabularyReviews.map((v) => ({ kind: "vocab" as const, data: v })),
        ...data.grammarReviews.map((g) => ({ kind: "grammar" as const, data: g })),
      ];
      setItems(session);
      setIndex(0);
      setCompleted(session.length === 0);
      if (data.recommendedLessonId && data.recommendedLessonTitle) {
        setLessonLink({ id: data.recommendedLessonId, title: data.recommendedLessonTitle });
      }
      await cacheReviewSession(data.vocabularyReviews);
    } catch {
      const cached = await getCachedReviewSession();
      if (cached?.items?.length) {
        const session = (cached.items as ReviewItem[]).map((v) => ({ kind: "vocab" as const, data: v }));
        setItems(session);
        setIndex(0);
        setError("Sin conexión — usando sesión guardada.");
      } else {
        setError("No se pudo cargar el repaso.");
      }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadReviews();
    const handleOnline = () => syncPendingReviews().then(loadReviews);
    window.addEventListener("online", handleOnline);
    return () => window.removeEventListener("online", handleOnline);
  }, [loadReviews]);

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
        <main className="flex flex-1 items-center justify-center">
          <p className="text-zinc-500">Cargando sesión diaria...</p>
        </main>
      </>
    );
  }

  if (completed || !current) {
    return (
      <>
        <AppHeader />
        <main className="mx-auto flex max-w-lg flex-1 flex-col items-center justify-center px-4 py-12 text-center">
          <h1 className="mb-2 text-2xl font-semibold">¡Sesión completada!</h1>
          <p className="mb-2 text-zinc-600">
            {items.length > 0
              ? "Buen trabajo. Vocabulario y gramática actualizados con SM-2."
              : "No hay ítems pendientes por ahora."}
          </p>
          {lessonLink && (
            <Link href={`/lessons/${lessonLink.id}`} className="mb-4 text-sm text-emerald-700 underline">
              Continuar con: {lessonLink.title}
            </Link>
          )}
          {offlineQueued > 0 && (
            <p className="mb-4 text-sm text-amber-600">
              {offlineQueued} respuesta(s) en cola — se sincronizarán al reconectar.
            </p>
          )}
          <div className="flex gap-3">
            <Link href="/dashboard" className="rounded-lg bg-emerald-700 px-5 py-2.5 text-sm font-medium text-white">
              Dashboard
            </Link>
            <button type="button" onClick={loadReviews} className="rounded-lg border px-5 py-2.5 text-sm">
              Otra ronda
            </button>
          </div>
        </main>
      </>
    );
  }

  return (
    <>
      <AppHeader />
      <main className="mx-auto flex w-full max-w-5xl flex-1 flex-col items-center px-4 py-8">
        <div className="mb-6 w-full max-w-lg">
          <div className="mb-2 flex justify-between text-sm text-zinc-500">
            <span>
              {current.kind === "vocab" ? "Vocabulario" : "Gramática"} · {index + 1} de {items.length}
            </span>
            {error && <span className="text-amber-600">{error}</span>}
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-zinc-200 dark:bg-zinc-800">
            <div className="h-full rounded-full bg-emerald-600 transition-all" style={{ width: `${((index + 1) / items.length) * 100}%` }} />
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
      </main>
    </>
  );
}
