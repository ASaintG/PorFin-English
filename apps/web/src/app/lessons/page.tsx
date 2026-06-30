"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type LessonSummary } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";

export default function LessonsPage() {
  const [lessons, setLessons] = useState<LessonSummary[]>([]);

  useEffect(() => {
    api.get<LessonSummary[]>("/lessons?cefrLevel=A1").then((r) => setLessons(r.data));
  }, []);

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-3xl px-4 py-8">
        <h1 className="mb-6 text-2xl font-semibold">Lecciones temáticas</h1>
        <div className="space-y-3">
          {lessons.map((l) => (
            <Link
              key={l.id}
              href={l.isLocked ? "#" : `/lessons/${l.id}`}
              className={`block rounded-xl border p-4 ${l.isLocked ? "opacity-50" : "hover:border-emerald-300"} border-zinc-200 dark:border-zinc-800`}
            >
              <div className="flex justify-between">
                <h2 className="font-medium">{l.sequence}. {l.title}</h2>
                {l.isLocked && <span className="text-xs">Bloqueada</span>}
              </div>
              <p className="text-sm text-zinc-500">{l.description}</p>
              <p className="mt-1 text-xs text-zinc-400">{l.exerciseCount} ejercicios · {l.theme}</p>
            </Link>
          ))}
        </div>
      </main>
    </>
  );
}
