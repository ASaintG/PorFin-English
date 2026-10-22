"use client";

import { useEffect, useState } from "react";
import api, { type SpeakingPrompt } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";
import { useSpeechRecognition } from "@/hooks/useSpeechRecognition";

const LEVELS = ["A1", "A2", "B1"] as const;

export default function SpeakingPage() {
  const [prompts, setPrompts] = useState<SpeakingPrompt[]>([]);
  const [active, setActive] = useState<SpeakingPrompt | null>(null);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [level, setLevel] = useState("A1");
  const { supported, listening, transcript, start } = useSpeechRecognition();

  useEffect(() => {
    api.get<SpeakingPrompt[]>(`/speaking/prompts?cefrLevel=${level}`).then((r) => {
      setPrompts(r.data);
      setActive(null);
      setFeedback(null);
    });
  }, [level]);

  const aiPrompt = active
    ? `Actúa como examinador de inglés ${level}. Evalúa esta transcripción para la tarea: "${active.prompt}". Transcripción: "${transcript || "[pega aquí tu transcripción]"}". Criterios: fluidez, gramática, vocabulario y cumplimiento de la tarea. Da una puntuación de 0 a 5 y 3 recomendaciones.`
    : "";

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
          <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
            <div>
              <h1 className="text-2xl font-semibold">Speaking</h1>
              <p className="text-sm text-muted-foreground">Practica producción oral por nivel y tema.</p>
            </div>
            <div className="flex rounded-lg border border-border p-1">
              {LEVELS.map((value) => (
                <button
                  key={value}
                  type="button"
                  onClick={() => setLevel(value)}
                  className={`rounded-md px-3 py-1 text-sm transition-colors ${
                    level === value ? "bg-primary text-primary-foreground" : "text-muted-foreground hover:bg-muted"
                  }`}
                >
                  {value}
                </button>
              ))}
            </div>
          </div>
          <div className="grid gap-3 md:grid-cols-2">
            {prompts.map((p) => (
              <button
                key={p.id}
                type="button"
                onClick={() => { setActive(p); setFeedback(null); }}
                className={`block w-full rounded-xl border bg-card p-4 text-left transition-colors ${active?.id === p.id ? "border-primary" : "border-border hover:border-primary/50"}`}
              >
                <span className="block">{p.prompt}</span>
                <span className="mt-1 block text-xs text-muted-foreground">{p.cefrLevel} · {p.category.replace(/_/g, " ")}</span>
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
              <div className="mt-4 rounded-xl border border-border bg-background p-4">
                <p className="mb-1 text-sm font-medium">Prompt para evaluación con IA</p>
                <p className="whitespace-pre-wrap text-sm text-muted-foreground">{aiPrompt}</p>
              </div>
            </div>
          )}
        </div>
      </PageContainer>
    </>
  );
}
