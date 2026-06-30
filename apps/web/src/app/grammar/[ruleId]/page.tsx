"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type GrammarRule } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";

export default function GrammarRulePage() {
  const { ruleId } = useParams<{ ruleId: string }>();
  const [rule, setRule] = useState<GrammarRule | null>(null);

  useEffect(() => {
    api.get<GrammarRule>(`/grammar/${ruleId}`).then((r) => setRule(r.data));
  }, [ruleId]);

  if (!rule) return <><AppHeader /><main className="p-8">Cargando...</main></>;

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-3xl px-4 py-8">
        <Link href="/grammar" className="mb-4 block text-sm text-zinc-500">← Volver</Link>
        <h1 className="mb-2 text-2xl font-semibold">{rule.title}</h1>
        <p className="mb-4 text-sm text-zinc-500">{rule.cefrLevel} · {rule.category}</p>
        <p className="mb-6 leading-relaxed">{rule.explanation}</p>
        <h2 className="mb-2 font-medium">Ejemplos</h2>
        <ul className="mb-8 list-inside list-disc space-y-1 italic">
          {rule.examples.map((e) => <li key={e}>{e}</li>)}
        </ul>
        <Link href="/review" className="rounded-lg bg-emerald-700 px-4 py-2 text-sm text-white">
          Practicar en repaso diario
        </Link>
      </main>
    </>
  );
}
