"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type GrammarRule } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function GrammarPage() {
  const [rules, setRules] = useState<GrammarRule[]>([]);

  useEffect(() => {
    Promise.all([
      api.get<GrammarRule[]>("/grammar?cefrLevel=A1"),
      api.get<GrammarRule[]>("/grammar?cefrLevel=A2"),
      api.get<GrammarRule[]>("/grammar?cefrLevel=B1"),
    ]).then(([a1, a2, b1]) => setRules([...a1.data, ...a2.data, ...b1.data]));
  }, []);

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-2xl font-semibold">Gramática</h1>
          <Link href="/review" className="text-sm text-primary underline">Repaso SRS</Link>
        </div>
        <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">
          {rules.map((r) => (
            <Link key={r.id} href={`/grammar/${r.id}`} className="block rounded-xl border border-border bg-card p-4 transition-colors hover:border-primary/50">
              <h2 className="font-medium">{r.title}</h2>
              <p className="text-sm text-muted-foreground">{r.cefrLevel} · {r.category}</p>
            </Link>
          ))}
        </div>
      </PageContainer>
    </>
  );
}
