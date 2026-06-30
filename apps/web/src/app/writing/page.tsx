"use client";

import { useEffect, useState } from "react";
import api, { type WritingPrompt } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";

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
      <main className="mx-auto max-w-3xl px-4 py-8">
        <h1 className="mb-6 text-2xl font-semibold">Writing</h1>
        <div className="mb-4 space-y-2">
          {prompts.map((p) => (
            <button key={p.id} type="button" onClick={() => { setActive(p); setText(""); setModel(null); }}
              className={`block w-full rounded-xl border p-3 text-left text-sm ${active?.id === p.id ? "border-emerald-500" : "border-zinc-200"}`}>
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
              className="w-full rounded-xl border border-zinc-300 p-3 dark:border-zinc-700 dark:bg-zinc-950"
              placeholder="Escribe tu respuesta en inglés..."
            />
            <p className="mt-1 text-sm text-zinc-500">{wordCount} / {active.wordLimit} palabras sugeridas</p>
            <button type="button" onClick={submit} className="mt-3 rounded-lg bg-emerald-700 px-4 py-2 text-sm text-white">Ver modelo</button>
            {model && (
              <div className="mt-4 rounded-xl bg-zinc-100 p-4 dark:bg-zinc-900">
                <p className="mb-1 text-sm font-medium">Respuesta modelo:</p>
                <p className="text-sm italic">{model}</p>
              </div>
            )}
          </>
        )}
      </main>
    </>
  );
}
