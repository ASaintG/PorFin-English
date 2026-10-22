"use client";

import Link from "next/link";
import { useCallback, useEffect, useMemo, useState } from "react";
import api, { type LessonSummary } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";
import { useAuthStore } from "@/stores/authStore";

const CEFR_LEVELS = ["A1", "A2", "B1"] as const;

type Level = (typeof CEFR_LEVELS)[number];
type Tab = "route" | "all";

const LEVEL_LABELS: Record<Level, string> = {
  A1: "Principiante",
  A2: "Elemental",
  B1: "Intermedio",
};

interface IconProps {
  className?: string;
}

function BookIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20" />
      <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2Z" />
    </svg>
  );
}

function HeadphonesIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M3 18v-6a9 9 0 0 1 18 0v6" />
      <path d="M21 19a2 2 0 0 1-2 2h-1a2 2 0 0 1-2-2v-3a2 2 0 0 1 2-2h3Z" />
      <path d="M3 19a2 2 0 0 0 2 2h1a2 2 0 0 0 2-2v-3a2 2 0 0 0-2-2H3Z" />
    </svg>
  );
}

function MicIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M12 2a3 3 0 0 0-3 3v7a3 3 0 0 0 6 0V5a3 3 0 0 0-3-3Z" />
      <path d="M19 10v2a7 7 0 0 1-14 0v-2" />
      <path d="M12 19v3" />
    </svg>
  );
}

function PenIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M12 20h9" />
      <path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4Z" />
    </svg>
  );
}

function ClockIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <circle cx="12" cy="12" r="9" />
      <path d="M12 7v5l3 2" />
    </svg>
  );
}

function CheckIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <circle cx="12" cy="12" r="9" />
      <path d="m8.5 12 2.5 2.5 4.5-5" />
    </svg>
  );
}

function ArrowRightIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M5 12h14" />
      <path d="m13 6 6 6-6 6" />
    </svg>
  );
}

const SKILLS = {
  grammar: { label: "Gramática", Icon: BookIcon },
  listening: { label: "Listening", Icon: HeadphonesIcon },
  speaking: { label: "Speaking", Icon: MicIcon },
  writing: { label: "Writing", Icon: PenIcon },
} as const;

type Skill = keyof typeof SKILLS;

function normalizeLevel(level: string | null): Level {
  const candidate = level?.slice(0, 2).toUpperCase();
  return CEFR_LEVELS.find((item) => item === candidate) ?? "A1";
}

function levelRank(level: string) {
  const index = CEFR_LEVELS.indexOf(level as Level);
  return index === -1 ? 0 : index;
}

function getLessonSkills(lesson: LessonSummary): Skill[] {
  const theme = lesson.theme.toLowerCase();
  const category = lesson.category.toLowerCase();
  const skills: Skill[] = ["grammar"];
  const add = (skill: Skill) => {
    if (!skills.includes(skill)) skills.push(skill);
  };

  if (["restaurant", "directions", "health", "meetings", "phone", "customer", "business_travel", "travel"].some((item) => theme === item)) {
    add("listening");
  }
  if (["greetings", "restaurant", "directions", "shopping", "job_interview", "meetings", "negotiations", "presentations", "phone", "teamwork", "customer"].some((item) => theme === item)) {
    add("speaking");
  }
  if (["emails", "job_interview", "presentations", "education", "technology", "housing"].some((item) => theme === item)) {
    add("writing");
  }
  if (["work", "technology", "education", "places"].includes(category)) {
    add("writing");
  }

  return skills;
}

function LessonCard({ lesson, isRecommended }: { lesson: LessonSummary; isRecommended: boolean }) {
  const completed = lesson.isCompleted;
  const skills = getLessonSkills(lesson);
  const minutes = Math.max(10, lesson.exerciseCount * 3 + 3);
  const border = isRecommended
    ? "border-primary ring-1 ring-primary/30"
    : completed
      ? "border-success/45"
      : "border-slate-700/80";

  return (
    <Link
      href={`/lessons/${lesson.id}`}
      className="group block h-full rounded-2xl focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary focus-visible:ring-offset-2 focus-visible:ring-offset-background"
    >
      <article
        className={`flex h-full min-h-[238px] flex-col rounded-2xl border bg-[#1E293B] p-5 text-slate-100 shadow-sm transition-all duration-200 group-hover:-translate-y-0.5 group-hover:shadow-lg ${border}`}
      >
        <div className="flex items-start justify-between gap-3">
          <div className="flex flex-wrap items-center gap-2">
            <span className="rounded-full bg-slate-800 px-2.5 py-0.5 text-xs font-semibold text-slate-300">
              {lesson.cefrLevel}
            </span>
            {isRecommended && (
              <span className="rounded-full bg-primary/20 px-2.5 py-0.5 text-xs font-semibold text-primary-hover">
                Siguiente
              </span>
            )}
          </div>
          {completed && (
            <span className="text-success" aria-label="Completado" title="Completado">
              <CheckIcon className="h-5 w-5" />
            </span>
          )}
        </div>

        <h3 className="mt-4 text-lg font-semibold leading-snug text-slate-100">
          {lesson.title}
        </h3>
        <p className="mt-1 line-clamp-2 text-sm leading-relaxed text-slate-400">
          {lesson.description}
        </p>

        <div className="mt-4 flex flex-wrap items-center gap-2" aria-label="Habilidades cubiertas">
          {skills.map((skill) => {
            const { label, Icon } = SKILLS[skill];
            return (
              <span
                key={skill}
                className="inline-flex h-7 w-7 items-center justify-center rounded-md bg-slate-800/80 text-slate-400"
                title={label}
                aria-label={label}
              >
                <Icon className="h-4 w-4" />
              </span>
            );
          })}
        </div>

        <div className="mt-auto flex items-center justify-between gap-3 pt-5">
          <span className="inline-flex items-center gap-1.5 text-xs text-slate-400">
            <ClockIcon className="h-3.5 w-3.5" />
            ~{minutes} min
          </span>
          <span
            className={`inline-flex items-center gap-1.5 rounded-lg px-3 py-1.5 text-sm font-medium transition-colors ${
              isRecommended
                ? "bg-primary text-primary-foreground group-hover:bg-primary-hover"
                : "border border-slate-600 text-slate-200 group-hover:border-slate-400 group-hover:text-white"
            }`}
          >
            {isRecommended ? "Empezar" : "Repasar"}
            {isRecommended && <ArrowRightIcon className="h-4 w-4" />}
          </span>
        </div>
      </article>
    </Link>
  );
}

export default function PathPage() {
  const currentLevel = useAuthStore((state) => state.currentLevel);
  const normalizedLevel = normalizeLevel(currentLevel);
  const [tab, setTab] = useState<Tab>("route");
  const [lessons, setLessons] = useState<LessonSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);

  const fetchLessons = useCallback(
    () =>
      Promise.all(CEFR_LEVELS.map((level) => api.get<LessonSummary[]>(`/lessons?cefrLevel=${level}`)))
        .then((responses) => responses.flatMap((response) => response.data)),
    [],
  );

  useEffect(() => {
    let cancelled = false;

    fetchLessons()
      .then((data) => {
        if (!cancelled) setLessons(data);
      })
      .catch(() => {
        if (!cancelled) setError(true);
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => {
      cancelled = true;
    };
  }, [fetchLessons]);

  const retry = () => {
    setLoading(true);
    setError(false);
    fetchLessons()
      .then(setLessons)
      .catch(() => setError(true))
      .finally(() => setLoading(false));
  };

  const groupedLessons = useMemo(
    () =>
      CEFR_LEVELS.map((level) => ({
        level,
        lessons: lessons
          .filter((lesson) => lesson.cefrLevel === level)
          .sort((a, b) => a.sequence - b.sequence),
      })).filter((group) => group.lessons.length > 0),
    [lessons],
  );

  const userRank = levelRank(normalizedLevel);
  const routeGroups = useMemo(
    () => groupedLessons.filter((group) => levelRank(group.level) >= userRank),
    [groupedLessons, userRank],
  );
  const visibleGroups = tab === "route" ? routeGroups : groupedLessons;
  const routeLessons = useMemo(
    () => routeGroups.flatMap((group) => group.lessons),
    [routeGroups],
  );
  const recommendedId = routeLessons.find((lesson) => !lesson.isCompleted)?.id ?? null;

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-6xl">
          <header className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
            <div>
              <h1 className="text-2xl font-semibold">Ruta de aprendizaje</h1>
              <p className="mt-1 max-w-2xl text-sm text-muted-foreground">
                Tu test inicial marca un punto de partida recomendado. Puedes explorar y practicar cualquier nivel.
              </p>
            </div>
            <div className="flex w-fit items-center gap-3 rounded-2xl border border-border bg-card px-4 py-3">
              <span className="text-xs uppercase tracking-wide text-muted-foreground">Nivel actual</span>
              <span className="rounded-lg bg-primary/10 px-2.5 py-1 text-sm font-semibold text-primary">
                {normalizedLevel}
              </span>
            </div>
          </header>

          <div className="mb-8 flex w-fit rounded-xl border border-border bg-card p-1" role="tablist" aria-label="Vista de la ruta de aprendizaje">
            <button
              type="button"
              role="tab"
              aria-selected={tab === "route"}
              onClick={() => setTab("route")}
              className={`rounded-lg px-4 py-2 text-sm font-medium transition-colors ${
                tab === "route"
                  ? "bg-primary text-primary-foreground shadow-sm"
                  : "text-muted-foreground hover:text-foreground"
              }`}
            >
              Mi Ruta
            </button>
            <button
              type="button"
              role="tab"
              aria-selected={tab === "all"}
              onClick={() => setTab("all")}
              className={`rounded-lg px-4 py-2 text-sm font-medium transition-colors ${
                tab === "all"
                  ? "bg-primary text-primary-foreground shadow-sm"
                  : "text-muted-foreground hover:text-foreground"
              }`}
            >
              Explorar Todo
            </button>
          </div>

          {loading && (
            <div className="rounded-2xl border border-dashed border-border p-8 text-center text-sm text-muted-foreground">
              Cargando ruta...
            </div>
          )}

          {!loading && error && (
            <div className="rounded-2xl border border-destructive/30 bg-destructive/5 p-8 text-center">
              <p className="text-sm text-muted-foreground">No se pudo cargar la ruta de aprendizaje.</p>
              <button
                type="button"
                onClick={retry}
                className="mt-4 rounded-lg bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition-colors hover:bg-primary-hover"
              >
                Reintentar
              </button>
            </div>
          )}

          {!loading && !error && visibleGroups.length === 0 && (
            <div className="rounded-2xl border border-dashed border-border p-8 text-center">
              <p className="text-sm text-muted-foreground">
                No hay temas disponibles para este nivel todavía.
              </p>
              {tab === "route" && (
                <button
                  type="button"
                  onClick={() => setTab("all")}
                  className="mt-4 rounded-lg border border-border px-4 py-2 text-sm font-medium transition-colors hover:bg-muted"
                >
                  Explorar todo
                </button>
              )}
            </div>
          )}

          {!loading && !error && visibleGroups.length > 0 && (
            <div className="space-y-10" role="tabpanel">
              {visibleGroups.map((group) => {
                const completed = group.lessons.filter((lesson) => lesson.isCompleted).length;
                return (
                  <section key={group.level} aria-labelledby={`level-${group.level}`}>
                    <div className="mb-4 flex flex-wrap items-center justify-between gap-3">
                      <div className="flex items-center gap-3">
                        <span className="rounded-lg bg-primary/15 px-3 py-1 text-sm font-bold text-primary">
                          {group.level}
                        </span>
                        <div>
                          <h2 id={`level-${group.level}`} className="font-semibold">
                            {LEVEL_LABELS[group.level as Level]}
                          </h2>
                          <p className="text-sm text-muted-foreground">
                            {completed} de {group.lessons.length} completadas
                          </p>
                        </div>
                      </div>
                      {group.level === normalizedLevel && (
                        <span className="rounded-full border border-primary/30 bg-primary/10 px-3 py-1 text-xs font-medium text-primary">
                          Tu nivel
                        </span>
                      )}
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
                      {group.lessons.map((lesson) => (
                        <LessonCard
                          key={lesson.id}
                          lesson={lesson}
                          isRecommended={lesson.id === recommendedId}
                        />
                      ))}
                    </div>
                  </section>
                );
              })}
            </div>
          )}
        </div>
      </PageContainer>
    </>
  );
}
