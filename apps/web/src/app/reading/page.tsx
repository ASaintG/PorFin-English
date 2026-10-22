"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type ReadingSummary } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

const LEVELS = ["A1", "A2", "B1"] as const;

export default function ReadingListPage() {
  const [texts, setTexts] = useState<ReadingSummary[]>([]);
  const [level, setLevel] = useState("A1");

  useEffect(() => {
    api.get<ReadingSummary[]>(`/reading?cefrLevel=${level}`).then((r) => setTexts(r.data));
  }, [level]);

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
          <h1 className="text-2xl font-semibold">Lectura</h1>
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
        <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">
          {texts.map((t) => (
            <Link key={t.id} href={`/reading/${t.id}`} className="block rounded-xl border border-border bg-card p-4 transition-colors hover:border-primary/50">
              <h2 className="font-medium">{t.title}</h2>
              <p className="text-sm text-muted-foreground">{t.cefrLevel} · {t.wordCount} palabras</p>
              <p className="mt-1 text-xs text-muted-foreground">{t.attribution}</p>
            </Link>
          ))}
        </div>
        <Link href="/settings/attributions" className="mt-6 block text-sm text-muted-foreground underline hover:text-foreground">Atribuciones de contenido</Link>
      </PageContainer>
    </>
  );
}
