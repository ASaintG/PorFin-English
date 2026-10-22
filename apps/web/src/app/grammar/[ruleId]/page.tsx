"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type GrammarRule } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function GrammarRulePage() {
  const { ruleId } = useParams<{ ruleId: string }>();
  const [rule, setRule] = useState<GrammarRule | null>(null);

  useEffect(() => {
    api.get<GrammarRule>(`/grammar/${ruleId}`).then((r) => setRule(r.data));
  }, [ruleId]);

  if (!rule) return <><AppHeader /><PageContainer>Cargando...</PageContainer></>;

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-3xl">
          <Link href="/grammar" className="mb-4 block text-sm text-muted-foreground hover:text-foreground">← Volver</Link>
          <h1 className="mb-2 text-2xl font-semibold">{rule.title}</h1>
          <p className="mb-4 text-sm text-muted-foreground">{rule.cefrLevel} · {rule.category}</p>
          <p className="mb-6 leading-relaxed">{rule.explanation}</p>
          <h2 className="mb-2 font-medium">Ejemplos</h2>
          <ul className="mb-8 list-inside list-disc space-y-1 italic">
            {rule.examples.map((e) => <li key={e}>{e}</li>)}
          </ul>
          <Link href="/review" className="inline-block rounded-lg bg-primary px-4 py-2 text-sm text-primary-foreground transition-colors hover:bg-primary-hover">
            Practicar en repaso diario
          </Link>
        </div>
      </PageContainer>
    </>
  );
}
