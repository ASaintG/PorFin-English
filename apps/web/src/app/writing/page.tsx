"use client";

import { useEffect, useState } from "react";
import api, { type WritingPrompt } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function WritingPage() {
  const [prompts, setPrompts] = useState<WritingPrompt[]>([]);
  const [active, setActive] = useState<WritingPrompt | null>(null);
  const [text, setText] = useState("");
  const [model, setModel] = useState<string | null>(null);

  useEffect(() => {
    api.get<WritingPrompt[]>("/writing/prompts?cefrLevel=B1&category=job_interview").then((r) => setPrompts(r.data));
  }, []);

  const submit = async () => {
    if (!active) return;
    const { data } = await api.post<{ modelAnswer: string; wordCount: number }>(
      "/writing/submit",
      { promptId: active.id, text },
    );
    setModel(data.modelAnswer);
  };

  const wordCount = text.split(/\s+/).filter(Boolean).length;

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-4xl">
          <h1 className="mb-6 text-2xl font-semibold">Writing</h1>
          <div className="mb-4 grid gap-2 md:grid-cols-2">
            {prompts.map((p) => (
              <button
                key={p.id}
                type="button"
                onClick={() => { setActive(p); setText(""); setModel(null); }}
                className={`block w-full rounded-xl border bg-card p-3 text-left text-sm transition-colors ${active?.id === p.id ? "border-primary" : "border-border hover:border-primary/50"}`}
              >
                {p.prompt}
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
