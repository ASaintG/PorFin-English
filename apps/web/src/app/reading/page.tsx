"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type ReadingSummary } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";

export default function ReadingListPage() {
  const [texts, setTexts] = useState<ReadingSummary[]>([]);

  useEffect(() => {
    api.get<ReadingSummary[]>("/reading?cefrLevel=A1").then((r) => setTexts(r.data));
  }, []);

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-3xl px-4 py-8">
        <h1 className="mb-6 text-2xl font-semibold">Lectura</h1>
        <div className="space-y-3">
          {texts.map((t) => (
            <Link key={t.id} href={`/reading/${t.id}`} className="block rounded-xl border border-zinc-200 p-4 hover:border-emerald-300 dark:border-zinc-800">
              <h2 className="font-medium">{t.title}</h2>
              <p className="text-sm text-zinc-500">{t.cefrLevel} · {t.wordCount} palabras</p>
              <p className="mt-1 text-xs text-zinc-400">{t.attribution}</p>
            </Link>
          ))}
        </div>
        <Link href="/settings/attributions" className="mt-6 block text-sm text-zinc-500 underline">Atribuciones de contenido</Link>
      </main>
    </>
  );
}
