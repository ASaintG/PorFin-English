"use client";

import { FormEvent, useState } from "react";
import api from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";

export default function FeedbackPage() {
  const [type, setType] = useState("suggestion");
  const [content, setContent] = useState("");
  const [relatedItemId, setRelatedItemId] = useState("");
  const [message, setMessage] = useState("");

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    await api.post("/suggestions", {
      type,
      content,
      relatedItemId: relatedItemId || null,
    });
    setMessage("¡Gracias! Tu mensaje fue enviado.");
    setContent("");
    setRelatedItemId("");
  };

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-lg px-4 py-8">
        <h1 className="mb-4 text-2xl font-semibold">Enviar feedback</h1>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium">Tipo</label>
            <select
              value={type}
              onChange={(e) => setType(e.target.value)}
              className="w-full rounded-lg border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
            >
              <option value="suggestion">Sugerencia</option>
              <option value="bug">Reportar error</option>
            </select>
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Mensaje</label>
            <textarea
              required
              value={content}
              onChange={(e) => setContent(e.target.value)}
              rows={5}
              className="w-full rounded-lg border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">ID de ejercicio (opcional)</label>
            <input
              value={relatedItemId}
              onChange={(e) => setRelatedItemId(e.target.value)}
              className="w-full rounded-lg border border-zinc-300 px-3 py-2 text-sm dark:border-zinc-700 dark:bg-zinc-950"
            />
          </div>
          <button type="submit" className="rounded-lg bg-emerald-700 px-4 py-2 text-sm text-white">
            Enviar
          </button>
        </form>
        {message && <p className="mt-4 text-sm text-emerald-700">{message}</p>}
      </main>
    </>
  );
}
