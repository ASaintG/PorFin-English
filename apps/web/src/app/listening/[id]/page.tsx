"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type ListeningDetail } from "@/api/client";
import { ComprehensionQuestions } from "@/components/exercises/ComprehensionQuestions";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";
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

  if (!item) return <><AppHeader /><PageContainer>Cargando...</PageContainer></>;

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-3xl">
          <h1 className="mb-4 text-2xl font-semibold">{item.title}</h1>
          {supported && (
            <button type="button" onClick={() => speak(item.transcript)} className="mb-4 rounded-lg bg-primary px-4 py-2 text-sm text-primary-foreground transition-colors hover:bg-primary-hover">
              Reproducir audio (TTS)
            </button>
          )}
          <details className="mb-6">
            <summary className="cursor-pointer text-sm text-muted-foreground hover:text-foreground">Ver transcripción</summary>
            <p className="mt-2 text-sm italic">{item.transcript}</p>
          </details>
          <ComprehensionQuestions questions={item.questions} onSubmit={handleSubmit} />
          {result && <p className="mt-4 text-success">{result}</p>}
        </div>
      </PageContainer>
    </>
  );
}
