"use client";

import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type ReadingDetail } from "@/api/client";
import { ComprehensionQuestions } from "@/components/exercises/ComprehensionQuestions";
import { AppHeader } from "@/components/layout/AppHeader";
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

  if (!text) return <><AppHeader /><main className="p-8">Cargando...</main></>;

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-3xl px-4 py-8">
        <h1 className="mb-4 text-2xl font-semibold">{text.title}</h1>
        {supported && (
          <div className="mb-4 flex gap-2">
            <button type="button" onClick={() => speak(text.body)} className="rounded-lg border px-3 py-1 text-sm">Escuchar</button>
            <button type="button" onClick={() => speakWithHighlight(text.body, setHighlight)} className="rounded-lg border px-3 py-1 text-sm">Con resaltado</button>
          </div>
        )}
        <p className="mb-6 leading-relaxed">
          {words.map((w, i) => (
            <span key={i} className={i === highlight ? "rounded bg-yellow-200 dark:bg-yellow-900" : ""}>{w} </span>
          ))}
        </p>
        <p className="mb-6 text-xs text-zinc-400">{text.attribution}</p>
        <ComprehensionQuestions questions={text.questions} onSubmit={handleSubmit} />
        {result && <p className="mt-4 font-medium text-emerald-700">{result}</p>}
      </main>
    </>
  );
}
