"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type LessonDetail } from "@/api/client";
import { FillBlank } from "@/components/exercises/FillBlank";
import { AppHeader } from "@/components/layout/AppHeader";

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

  if (!lesson) return <><AppHeader /><main className="p-8">Cargando...</main></>;

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-3xl px-4 py-8">
        <h1 className="mb-2 text-2xl font-semibold">{lesson.title}</h1>
        <p className="mb-6 text-zinc-600">{lesson.description}</p>

        {lesson.grammarRules.map((r) => (
          <div key={r.id} className="mb-4 rounded-xl border border-zinc-200 p-4 dark:border-zinc-800">
            <h3 className="font-medium">{r.title}</h3>
            <p className="mt-1 text-sm text-zinc-600">{r.explanation}</p>
            <ul className="mt-2 list-inside list-disc text-sm italic">
              {r.examples.map((e) => <li key={e}>{e}</li>)}
            </ul>
          </div>
        ))}

        {exercise ? (
          <div>
            <p className="mb-2 text-sm text-zinc-500">Ejercicio {exIndex + 1} de {lesson.exercises.length}</p>
            <FillBlank content={exercise.content} onSubmit={handleSubmit} />
          </div>
        ) : (
          <p className="text-emerald-700">Lección completada.</p>
        )}
      </main>
    </>
  );
}
