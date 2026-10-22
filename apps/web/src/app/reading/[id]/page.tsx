"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type ReadingDetail } from "@/api/client";
import { ComprehensionQuestions } from "@/components/exercises/ComprehensionQuestions";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";
import { useSpeech } from "@/hooks/useSpeech";
import { cacheReadingText, getCachedReadingText } from "@/lib/offlineDb";

export default function ReadingDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [text, setText] = useState<ReadingDetail | null>(null);
  const [highlight, setHighlight] = useState(-1);
  const [result, setResult] = useState<string | null>(null);
  const { speakWithHighlight, speak, supported } = useSpeech();
  const words = text?.body.split(/\s+/) ?? [];

  useEffect(() => {
    api.get<ReadingDetail>(`/reading/${id}`)
      .then((r) => {
        setText(r.data);
        cacheReadingText(id, r.data);
      })
      .catch(async () => {
        const cached = await getCachedReadingText(id);
        if (cached) setText(cached as ReadingDetail);
      });
  }, [id]);

  const handleSubmit = async (answers: number[]) => {
    const { data } = await api.post<{ correct: number; total: number; scorePercent: number }>(
      `/reading/${id}/submit`,
      { answers },
    );
    setResult(`Puntuación: ${data.scorePercent}% (${data.correct}/${data.total})`);
  };

  if (!text) return <><AppHeader /><PageContainer>Cargando...</PageContainer></>;

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-3xl">
          <h1 className="mb-4 text-2xl font-semibold">{text.title}</h1>
          {supported && (
            <div className="mb-4 flex gap-2">
              <button type="button" onClick={() => speak(text.body)} className="rounded-lg border border-border px-3 py-1 text-sm transition-colors hover:bg-muted">Escuchar</button>
              <button type="button" onClick={() => speakWithHighlight(text.body, setHighlight)} className="rounded-lg border border-border px-3 py-1 text-sm transition-colors hover:bg-muted">Con resaltado</button>
            </div>
          )}
          <p className="mb-6 leading-relaxed">
            {words.map((w, i) => (
              <span key={i} className={i === highlight ? "rounded bg-warning/30 text-foreground" : ""}>{w} </span>
            ))}
          </p>
          <p className="mb-6 text-xs text-muted-foreground">{text.attribution}</p>
          <ComprehensionQuestions questions={text.questions} onSubmit={handleSubmit} />
          {result && <p className="mt-4 font-medium text-success">{result}</p>}
        </div>
      </PageContainer>
    </>
  );
}
