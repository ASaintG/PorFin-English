"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type LessonSummary } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function LessonsPage() {
  const [lessons, setLessons] = useState<LessonSummary[]>([]);

  useEffect(() => {
    api.get<LessonSummary[]>("/lessons?cefrLevel=A1").then((r) => setLessons(r.data));
  }, []);

  return (
    <>
      <AppHeader />
      <PageContainer>
        <h1 className="mb-6 text-2xl font-semibold">Lecciones temáticas</h1>
        <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">
          {lessons.map((l) => (
            <Link
              key={l.id}
              href={`/lessons/${l.id}`}
              className="block rounded-xl border border-border bg-card p-4 transition-colors hover:border-primary/50"
            >
              <div className="flex justify-between">
                <h2 className="font-medium">{l.sequence}. {l.title}</h2>
                {l.isCompleted && <span className="text-xs text-success">Completada</span>}
              </div>
              <p className="text-sm text-muted-foreground">{l.description}</p>
              <p className="mt-1 text-xs text-muted-foreground">{l.exerciseCount} ejercicios · {l.theme}</p>
            </Link>
          ))}
        </div>
      </PageContainer>
    </>
  );
}
