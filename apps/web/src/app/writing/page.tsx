"use client";

import { useEffect, useState } from "react";
import api, { type WritingPrompt } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

const LEVELS = ["A1", "A2", "B1"] as const;

export default function WritingPage() {
  const [prompts, setPrompts] = useState<WritingPrompt[]>([]);
  const [active, setActive] = useState<WritingPrompt | null>(null);
  const [text, setText] = useState("");
  const [model, setModel] = useState<string | null>(null);
  const [level, setLevel] = useState("A2");

  useEffect(() => {
    api.get<WritingPrompt[]>(`/writing/prompts?cefrLevel=${level}`).then((r) => {
      setPrompts(r.data);
      setActive(null);
      setText("");
      setModel(null);
    });
  }, [level]);

  const submit = async () => {
    if (!active) return;
    const { data } = await api.post<{ modelAnswer: string; wordCount: number }>(
      "/writing/submit",
      { promptId: active.id, text },
    );
    setModel(data.modelAnswer);
  };

  const wordCount = text.split(/\s+/).filter(Boolean).length;
  const aiPrompt = active
    ? `Actúa como profesor de inglés ${level}. Corrige esta respuesta a la tarea: "${active.prompt}". Criterios: corrección gramatical, uso del tema, claridad y vocabulario. Señala 3 errores, explica cada uno y da una versión mejorada.`
    : "";

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-4xl">
          <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
            <h1 className="text-2xl font-semibold">Writing</h1>
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
          <div className="mb-4 grid gap-2 md:grid-cols-2">
            {prompts.map((p) => (
              <button
                key={p.id}
                type="button"
                onClick={() => { setActive(p); setText(""); setModel(null); }}
                className={`block w-full rounded-xl border bg-card p-3 text-left text-sm transition-colors ${active?.id === p.id ? "border-primary" : "border-border hover:border-primary/50"}`}
              >
                <span className="block">{p.prompt}</span>
                <span className="mt-1 block text-xs text-muted-foreground">{p.cefrLevel} · {p.category.replace(/_/g, " ")}</span>
              </button>
            ))}
          </div>
          {active && (
            <>
              <textarea
                value={text}
                onChange={(e) => setText(e.target.value)}
                rows={8}
                className="w-full rounded-xl border border-input bg-background p-3 text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none"
                placeholder="Escribe tu respuesta en inglés..."
              />
              <p className="mt-1 text-sm text-muted-foreground">{wordCount} / {active.wordLimit} palabras sugeridas</p>
              <button type="button" onClick={submit} className="mt-3 rounded-lg bg-primary px-4 py-2 text-sm text-primary-foreground transition-colors hover:bg-primary-hover">Ver modelo</button>
              <div className="mt-4 rounded-xl border border-border bg-card p-4">
                <p className="mb-1 text-sm font-medium">Prompt para evaluación con IA</p>
                <p className="whitespace-pre-wrap text-sm text-muted-foreground">{aiPrompt}</p>
              </div>
              {model && (
                <div className="mt-4 rounded-xl bg-muted p-4">
                  <p className="mb-1 text-sm font-medium">Respuesta modelo:</p>
                  <p className="text-sm italic">{model}</p>
                </div>
              )}
            </>
          )}
        </div>
      </PageContainer>
    </>
  );
}
