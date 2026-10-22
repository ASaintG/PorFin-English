"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type LessonDetail } from "@/api/client";
import { FillBlank } from "@/components/exercises/FillBlank";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function LessonDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [lesson, setLesson] = useState<LessonDetail | null>(null);
  const [exIndex, setExIndex] = useState(0);

  useEffect(() => {
    api.get<LessonDetail>(`/lessons/${id}`).then((r) => setLesson(r.data));
  }, [id]);

  const exercise = lesson?.exercises[exIndex];

  const handleSubmit = async (correct: boolean) => {
    if (!exercise) return;
    await api.post(`/exercises/${exercise.id}/submit`, { quality: correct ? 5 : 2, correct });
    if (lesson && exIndex < lesson.exercises.length - 1) setExIndex(exIndex + 1);
    else if (lesson) await api.post(`/lessons/${id}/complete`);
  };

  if (!lesson) return <><AppHeader /><PageContainer>Cargando...</PageContainer></>;

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-4xl">
          <h1 className="mb-2 text-2xl font-semibold">{lesson.title}</h1>
          <p className="mb-6 text-muted-foreground">{lesson.description}</p>

          {lesson.grammarRules.map((r) => (
            <div key={r.id} className="mb-4 rounded-xl border border-border bg-card p-4">
              <h3 className="font-medium">{r.title}</h3>
              <p className="mt-1 text-sm text-muted-foreground">{r.explanation}</p>
              <ul className="mt-2 list-inside list-disc text-sm italic">
                {r.examples.map((e) => <li key={e}>{e}</li>)}
              </ul>
            </div>
          ))}

          {exercise ? (
            <div>
              <p className="mb-2 text-sm text-muted-foreground">Ejercicio {exIndex + 1} de {lesson.exercises.length}</p>
              <FillBlank content={exercise.content} onSubmit={handleSubmit} />
            </div>
          ) : (
            <p className="text-success">Lección completada.</p>
          )}
        </div>
      </PageContainer>
    </>
  );
}
