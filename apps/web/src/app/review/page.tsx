"use client";

import Link from "next/link";
import { useCallback, useEffect, useMemo, useState, type ReactNode } from "react";
import api, { type DailySession, type ExerciseDue, type ReviewItem } from "@/api/client";
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

type SkillFilter = "all" | "vocab" | "grammar";

const LEVELS = ["A1", "A2", "B1"] as const;

function VocabIcon({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20" />
      <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2Z" />
      <path d="M9 7h7" />
      <path d="M9 11h7" />
    </svg>
  );
}

function GrammarIcon({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="m3 8 4-4 4 4" />
      <path d="M7 4v16" />
      <path d="m13 16 4 4 4-4" />
      <path d="M17 20V4" />
    </svg>
  );
}

function CheckIcon({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <circle cx="12" cy="12" r="9" />
      <path d="m8.5 12 2.5 2.5 4.5-5" />
    </svg>
  );
}

function ArrowRightIcon({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M5 12h14" />
      <path d="m13 6 6 6-6 6" />
    </svg>
  );
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

async function fetchReviewSession(level: string): Promise<ReviewSessionResult> {
  try {
    await syncPendingReviews();
    const params = new URLSearchParams({ cefrLevel: level });
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
  const [finished, setFinished] = useState(false);
  const [offlineQueued, setOfflineQueued] = useState(0);
  const [error, setError] = useState("");
  const [level, setLevel] = useState("A1");
  const [skillFilter, setSkillFilter] = useState<SkillFilter>("all");
  const [reviewedCount, setReviewedCount] = useState(0);

  const loadReviews = useCallback(() => {
    void fetchReviewSession(level).then((result) => {
      setItems(result.session);
      setIndex(0);
      setReviewedCount(0);
      setFinished(false);
      setLessonLink(result.lessonLink);
      setError(result.error);
      setLoading(false);
    });
  }, [level]);

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

  const counts = useMemo(
    () => ({
      all: items.length,
      vocab: items.filter((item) => item.kind === "vocab").length,
      grammar: items.filter((item) => item.kind === "grammar").length,
    }),
    [items],
  );

  const filteredItems = useMemo(
    () =>
      skillFilter === "all"
        ? items
        : items.filter((item) => item.kind === skillFilter),
    [items, skillFilter],
  );

  const current = filteredItems[index];

  const changeLevel = (value: string) => {
    setLoading(true);
    setLevel(value);
  };

  const changeSkillFilter = (value: SkillFilter) => {
    setIndex(0);
    setReviewedCount(0);
    setFinished(false);
    setSkillFilter(value);
  };

  const advance = () => {
    const nextIndex = index + 1;
    if (nextIndex >= filteredItems.length) setFinished(true);
    else setIndex(nextIndex);
  };

  const handleVocabAnswer = async (quality: number) => {
    if (!current || current.kind !== "vocab") return;
    setSubmitting(true);
    const synced = await submitVocabReview(current.data.itemId, quality);
    if (!synced) setOfflineQueued((n) => n + 1);
    setReviewedCount((n) => n + 1);
    advance();
    setSubmitting(false);
  };

  const handleGrammarAnswer = async (correct: boolean) => {
    if (!current || current.kind !== "grammar") return;
    setSubmitting(true);
    await submitGrammarReview(current.data.id, correct);
    setReviewedCount((n) => n + 1);
    advance();
    setSubmitting(false);
  };

  const filters = (
    <div className="mb-8 w-full max-w-3xl space-y-3">
      <div className="flex flex-wrap items-center gap-2" role="group" aria-label="Filtrar por nivel">
        {LEVELS.map((value) => (
          <button
            key={value}
            type="button"
            onClick={() => changeLevel(value)}
            aria-pressed={level === value}
            className={`rounded-full px-5 py-2 text-sm font-semibold transition-all ${
              level === value
                ? "bg-primary text-primary-foreground shadow-md shadow-primary/20"
                : "bg-muted text-muted-foreground hover:bg-muted/80 hover:text-foreground"
            }`}
          >
            {value}
          </button>
        ))}
      </div>

      <div className="flex flex-wrap items-center gap-2" role="group" aria-label="Filtrar por habilidad">
        <SkillPill
          label="Todos"
          count={counts.all}
          active={skillFilter === "all"}
          onClick={() => changeSkillFilter("all")}
          tone="neutral"
        />
        <SkillPill
          label="Vocabulario"
          count={counts.vocab}
          active={skillFilter === "vocab"}
          onClick={() => changeSkillFilter("vocab")}
          tone="vocab"
          icon={<VocabIcon className="h-4 w-4" />}
        />
        <SkillPill
          label="Gramática"
          count={counts.grammar}
          active={skillFilter === "grammar"}
          onClick={() => changeSkillFilter("grammar")}
          tone="grammar"
          icon={<GrammarIcon className="h-4 w-4" />}
        />
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

  if (finished) {
    return (
      <>
        <AppHeader />
        <PageContainer className="flex flex-1 flex-col items-center justify-center">
          <div className="mx-auto w-full max-w-2xl">
            <div className="rounded-3xl border border-border bg-card p-6 text-center shadow-sm sm:p-8">
              <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-success/10 text-success">
                <CheckIcon className="h-9 w-9" />
              </div>
              <h1 className="text-2xl font-semibold">¡Buen trabajo!</h1>
              <p className="mt-2 text-muted-foreground">
                Has repasado {reviewedCount} {reviewedCount === 1 ? "tarjeta" : "tarjetas"}.
              </p>

              {offlineQueued > 0 && (
                <p className="mt-3 text-sm text-warning">
                  {offlineQueued} respuesta(s) en cola — se sincronizarán al reconectar.
                </p>
              )}

              <div className="mt-8 rounded-2xl border border-primary/30 bg-primary/5 p-5 text-left sm:p-6">
                <p className="text-xs font-semibold uppercase tracking-wide text-primary">
                  Siguiente sesión
                </p>
                {lessonLink ? (
                  <>
                    <h2 className="mt-2 text-xl font-semibold">{lessonLink.title}</h2>
                    <p className="mt-1 text-sm text-muted-foreground">
                      Continúa con la siguiente lección recomendada para tu nivel.
                    </p>
                    <Link
                      href={`/lessons/${lessonLink.id}`}
                      className="mt-5 inline-flex items-center gap-2 rounded-xl bg-primary px-5 py-3 text-sm font-semibold text-primary-foreground transition-colors hover:bg-primary-hover"
                    >
                      Continuar aprendiendo
                      <ArrowRightIcon className="h-4 w-4" />
                    </Link>
                  </>
                ) : (
                  <>
                    <h2 className="mt-2 text-xl font-semibold">Explora la ruta de aprendizaje</h2>
                    <p className="mt-1 text-sm text-muted-foreground">
                      Encuentra tu siguiente tema y sigue practicando.
                    </p>
                    <Link
                      href="/path"
                      className="mt-5 inline-flex items-center gap-2 rounded-xl bg-primary px-5 py-3 text-sm font-semibold text-primary-foreground transition-colors hover:bg-primary-hover"
                    >
                      Ver ruta de aprendizaje
                      <ArrowRightIcon className="h-4 w-4" />
                    </Link>
                  </>
                )}
              </div>

              <div className="mt-6 flex flex-col gap-3 sm:flex-row sm:justify-center">
                <button
                  type="button"
                  onClick={() => {
                    setLoading(true);
                    loadReviews();
                  }}
                  className="rounded-xl border border-border bg-transparent px-5 py-2.5 text-sm font-medium transition-colors hover:bg-muted"
                >
                  Otra ronda
                </button>
                <Link
                  href="/dashboard"
                  className="rounded-xl border border-border bg-transparent px-5 py-2.5 text-sm font-medium transition-colors hover:bg-muted"
                >
                  Volver al Dashboard
                </Link>
              </div>
            </div>
          </div>
        </PageContainer>
      </>
    );
  }

  if (!current) {
    return (
      <>
        <AppHeader />
        <PageContainer className="flex flex-1 flex-col items-center">
          {filters}
          <div className="w-full max-w-3xl rounded-3xl border border-dashed border-border p-8 text-center">
            <h1 className="text-xl font-semibold">No hay tarjetas pendientes</h1>
            <p className="mt-2 text-sm text-muted-foreground">
              {skillFilter === "all"
                ? `No hay repasos para ${level} en este momento.`
                : `No hay repasos de ${skillFilter === "vocab" ? "vocabulario" : "gramática"} para ${level}.`}
            </p>
            {skillFilter !== "all" && counts.all > 0 && (
              <button
                type="button"
                onClick={() => changeSkillFilter("all")}
                className="mt-5 rounded-xl border border-border px-4 py-2 text-sm font-medium transition-colors hover:bg-muted"
              >
                Ver todos los repasos
              </button>
            )}
          </div>
        </PageContainer>
      </>
    );
  }

  const isVocab = current.kind === "vocab";

  return (
    <>
      <AppHeader />
      <PageContainer className="flex flex-1 flex-col items-center">
        {filters}
        <div className="mb-6 w-full max-w-lg">
          <div className="mb-2 flex items-center justify-between gap-3 text-sm text-muted-foreground">
            <span className="inline-flex items-center gap-2">
              <span
                className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-medium ${
                  isVocab
                    ? "bg-success/10 text-success"
                    : "bg-sky-500/10 text-sky-400"
                }`}
              >
                {isVocab ? <VocabIcon className="h-3.5 w-3.5" /> : <GrammarIcon className="h-3.5 w-3.5" />}
                {isVocab ? "Vocabulario" : "Gramática"}
              </span>
              <span>{index + 1} de {filteredItems.length}</span>
            </span>
            {error && <span className="text-warning">{error}</span>}
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-progress-track">
            <div
              className={`h-full rounded-full transition-all ${isVocab ? "bg-success" : "bg-sky-500"}`}
              style={{ width: `${((index + 1) / filteredItems.length) * 100}%` }}
            />
          </div>
        </div>

        {isVocab ? (
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

function SkillPill({
  label,
  count,
  active,
  onClick,
  tone,
  icon,
}: {
  label: string;
  count: number;
  active: boolean;
  onClick: () => void;
  tone: "neutral" | "vocab" | "grammar";
  icon?: ReactNode;
}) {
  const activeStyles = {
    neutral: "border-slate-600 bg-slate-700 text-white shadow-sm",
    vocab: "border-success/40 bg-success/15 text-success",
    grammar: "border-sky-500/40 bg-sky-500/15 text-sky-400",
  }[tone];

  return (
    <button
      type="button"
      onClick={onClick}
      aria-pressed={active}
      className={`inline-flex items-center gap-2 rounded-full border px-4 py-2 text-sm font-medium transition-all ${
        active
          ? activeStyles
          : "border-border bg-muted/60 text-muted-foreground hover:bg-muted hover:text-foreground"
      }`}
    >
      {icon}
      {label}
      <span className={`rounded-full px-1.5 py-0.5 text-[11px] ${active ? "bg-black/15" : "bg-background/50"}`}>
        {count}
      </span>
    </button>
  );
}
