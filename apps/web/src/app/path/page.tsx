"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type LessonSummary } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function PathPage() {
  const [lessons, setLessons] = useState<LessonSummary[]>([]);

  useEffect(() => {
    Promise.all([
      api.get<LessonSummary[]>("/lessons?cefrLevel=A1"),
      api.get<LessonSummary[]>("/lessons?cefrLevel=A2"),
    ]).then(([a1, a2]) => setLessons([...a1.data, ...a2.data]));
  }, []);

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-3xl">
          <h1 className="mb-6 text-2xl font-semibold">Ruta de aprendizaje</h1>
          <div className="relative space-y-0">
            {lessons.map((l, i) => (
              <div key={l.id} className="flex gap-4 pb-8">
                <div className="flex flex-col items-center">
                  <div className={`flex h-8 w-8 items-center justify-center rounded-full text-sm ${l.isLocked ? "bg-muted text-muted-foreground" : "bg-primary text-primary-foreground"}`}>
                    {l.sequence}
                  </div>
                  {i < lessons.length - 1 && <div className="mt-1 w-0.5 flex-1 bg-border" />}
                </div>
                <div className="flex-1 pb-2">
                  {l.isLocked ? (
                    <p className="font-medium text-muted-foreground">{l.title}</p>
                  ) : (
                    <Link href={`/lessons/${l.id}`} className="font-medium transition-colors hover:text-primary">{l.title}</Link>
                  )}
                  <p className="text-sm text-muted-foreground">{l.cefrLevel} · {l.theme}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </PageContainer>
    </>
  );
}
