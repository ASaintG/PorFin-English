"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type ListeningDetail } from "@/api/client";
import { ComprehensionQuestions } from "@/components/exercises/ComprehensionQuestions";
import { AppHeader } from "@/components/layout/AppHeader";
import { useSpeech } from "@/hooks/useSpeech";
import { cacheListeningExercise, getCachedListeningExercise } from "@/lib/offlineDb";

export default function ListeningDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [item, setItem] = useState<ListeningDetail | null>(null);
  const [result, setResult] = useState<string | null>(null);
  const { speak, supported } = useSpeech();

  useEffect(() => {
    api.get<ListeningDetail>(`/listening/${id}`)
      .then((r) => {
        setItem(r.data);
        cacheListeningExercise(id, r.data);
      })
      .catch(async () => {
        const cached = await getCachedListeningExercise(id);
        if (cached) setItem(cached as ListeningDetail);
      });
  }, [id]);

  const handleSubmit = async (answers: number[]) => {
    const { data } = await api.post<{ scorePercent: number; correct: number; total: number }>(
      `/listening/${id}/submit`,
      { answers },
    );
    setResult(`Puntuación: ${data.scorePercent}%`);
  };

  if (!item) return <><AppHeader /><main className="p-8">Cargando...</main></>;

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-3xl px-4 py-8">
        <h1 className="mb-4 text-2xl font-semibold">{item.title}</h1>
        {supported && (
          <button type="button" onClick={() => speak(item.transcript)} className="mb-4 rounded-lg bg-emerald-700 px-4 py-2 text-sm text-white">
            Reproducir audio (TTS)
          </button>
        )}
        <details className="mb-6">
          <summary className="cursor-pointer text-sm text-zinc-500">Ver transcripción</summary>
          <p className="mt-2 text-sm italic">{item.transcript}</p>
        </details>
        <ComprehensionQuestions questions={item.questions} onSubmit={handleSubmit} />
        {result && <p className="mt-4 text-emerald-700">{result}</p>}
      </main>
    </>
  );
}
