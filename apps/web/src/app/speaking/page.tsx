"use client";

import { useEffect, useState } from "react";
import api, { type SpeakingPrompt } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";
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
      <PageContainer>
        <div className="mx-auto w-full max-w-4xl">
          <h1 className="mb-2 text-2xl font-semibold">Speaking</h1>
          <p className="mb-6 text-sm text-muted-foreground">Track: English for Job Interviews (B1)</p>
          <div className="grid gap-3 md:grid-cols-2">
            {prompts.map((p) => (
              <button
                key={p.id}
                type="button"
                onClick={() => { setActive(p); setFeedback(null); }}
                className={`block w-full rounded-xl border bg-card p-4 text-left transition-colors ${active?.id === p.id ? "border-primary" : "border-border hover:border-primary/50"}`}
              >
                {p.prompt}
              </button>
            ))}
          </div>
          {active && (
            <div className="mt-6 rounded-xl border border-border bg-card p-4">
              {!supported && <p className="text-warning">Usa Chrome o Edge para reconocimiento de voz.</p>}
              <button type="button" disabled={!supported || listening} onClick={start} className="rounded-lg bg-primary px-4 py-2 text-sm text-primary-foreground transition-colors hover:bg-primary-hover disabled:opacity-40">
                {listening ? "Escuchando..." : "Grabar respuesta"}
              </button>
              {transcript && <p className="mt-3 text-sm">{transcript}</p>}
              {transcript && (
                <button type="button" onClick={submit} className="mt-3 rounded-lg border border-border px-4 py-2 text-sm transition-colors hover:bg-muted">Enviar</button>
              )}
              {feedback && <p className="mt-3 text-sm text-success">{feedback}</p>}
            </div>
          )}
        </div>
      </PageContainer>
    </>
  );
}
