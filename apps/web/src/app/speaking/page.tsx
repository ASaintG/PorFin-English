"use client";

import { useEffect, useState } from "react";
import api, { type SpeakingPrompt } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { useSpeechRecognition } from "@/hooks/useSpeechRecognition";

export default function SpeakingPage() {
  const [prompts, setPrompts] = useState<SpeakingPrompt[]>([]);
  const [active, setActive] = useState<SpeakingPrompt | null>(null);
  const [feedback, setFeedback] = useState<string | null>(null);
  const { supported, listening, transcript, start } = useSpeechRecognition();

  useEffect(() => {
    api.get<SpeakingPrompt[]>("/speaking/prompts?cefrLevel=B1&category=job_interview").then((r) => setPrompts(r.data));
  }, []);

  const submit = async () => {
    if (!active) return;
    const { data } = await api.post<{ matchedKeyPhrases: string[]; matchedCount: number; totalKeyPhrases: number; modelAnswer: string }>(
      "/speaking/submit",
      { promptId: active.id, transcript, durationSeconds: 60 },
    );
    setFeedback(`Frases clave: ${data.matchedCount}/${data.totalKeyPhrases}. Modelo: ${data.modelAnswer}`);
  };

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-3xl px-4 py-8">
        <h1 className="mb-2 text-2xl font-semibold">Speaking</h1>
        <p className="mb-6 text-sm text-zinc-500">Track: English for Job Interviews (B1)</p>
        <div className="space-y-3">
          {prompts.map((p) => (
            <button
              key={p.id}
              type="button"
              onClick={() => { setActive(p); setFeedback(null); }}
              className={`block w-full rounded-xl border p-4 text-left ${active?.id === p.id ? "border-emerald-500" : "border-zinc-200 dark:border-zinc-800"}`}
            >
              {p.prompt}
            </button>
          ))}
        </div>
        {active && (
          <div className="mt-6 rounded-xl border border-zinc-200 p-4 dark:border-zinc-800">
            {!supported && <p className="text-amber-600">Usa Chrome o Edge para reconocimiento de voz.</p>}
            <button type="button" disabled={!supported || listening} onClick={start} className="rounded-lg bg-emerald-700 px-4 py-2 text-sm text-white">
              {listening ? "Escuchando..." : "Grabar respuesta"}
            </button>
            {transcript && <p className="mt-3 text-sm">{transcript}</p>}
            {transcript && (
              <button type="button" onClick={submit} className="mt-3 rounded-lg border px-4 py-2 text-sm">Enviar</button>
            )}
            {feedback && <p className="mt-3 text-sm text-emerald-700">{feedback}</p>}
          </div>
        )}
      </main>
    </>
  );
}
