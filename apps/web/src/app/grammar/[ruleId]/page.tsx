"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type GrammarRule } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";
import { ProductionTask } from "@/components/production/ProductionTask";

function cleanItem(item: string, prefix: string) {
  return item.startsWith(prefix) ? item.slice(prefix.length) : item;
}

export default function GrammarRulePage() {
  const { ruleId } = useParams<{ ruleId: string }>();
  const [rule, setRule] = useState<GrammarRule | null>(null);

  useEffect(() => {
    api.get<GrammarRule>(`/grammar/${ruleId}`).then((r) => setRule(r.data));
  }, [ruleId]);

  if (!rule) return <><AppHeader /><PageContainer>Cargando...</PageContainer></>;

  const subRules = rule.relatedStructures.filter((item) => item.startsWith("Sub-regla: "));
  const errors = rule.relatedStructures.filter((item) => item.startsWith("Error común: "));
  const production = rule.relatedStructures.filter((item) => item.startsWith("Producción ("));
  const productionTasks = rule.productionTasks ?? [];
  const taskTypes = new Set(productionTasks.map((task) => task.type.toLowerCase()));
  for (const item of production) {
    const match = item.match(/^Producción \(([^)]+)\)/);
    if (match) taskTypes.add(match[1].toLowerCase());
  }

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-3xl">
          <Link href="/grammar" className="mb-4 block text-sm text-muted-foreground hover:text-foreground">← Volver</Link>
          <h1 className="mb-2 text-2xl font-semibold">{rule.title}</h1>
          <p className="mb-4 text-sm text-muted-foreground">{rule.cefrLevel} · {rule.category}</p>
          <p className="mb-6 leading-relaxed">{rule.explanation}</p>

          {subRules.length > 0 && (
            <section className="mb-6">
              <h2 className="mb-2 font-medium">Reglas y formas</h2>
              <ul className="list-inside list-disc space-y-1 text-sm text-muted-foreground">
                {subRules.map((item) => <li key={item}>{cleanItem(item, "Sub-regla: ")}</li>)}
              </ul>
            </section>
          )}

          <h2 className="mb-2 font-medium">Ejemplos</h2>
          <ul className="mb-6 list-inside list-disc space-y-1 italic">
            {rule.examples.map((e) => <li key={e}>{e}</li>)}
          </ul>

          {errors.length > 0 && (
            <section className="mb-6 rounded-xl border border-destructive/30 bg-destructive/5 p-4">
              <h2 className="mb-2 font-medium">Errores comunes de hispanohablantes</h2>
              <ul className="list-inside list-disc space-y-1 text-sm">
                {errors.map((item) => <li key={item}>{cleanItem(item, "Error común: ")}</li>)}
              </ul>
            </section>
          )}

          {(productionTasks.length > 0 || production.length > 0) && (
            <section className="mb-8">
              <div className="mb-3 flex flex-wrap items-end justify-between gap-2">
                <div>
                  <h2 className="text-lg font-semibold">Tarea de producción</h2>
                  <p className="text-sm text-muted-foreground">
                    Practica lo aprendido con speaking, writing y listening.
                  </p>
                </div>
                <div className="flex flex-wrap gap-2 text-xs text-muted-foreground">
                  {taskTypes.has("speaking") && (
                    <span className="rounded-full bg-violet-500/10 px-2.5 py-1 text-violet-300">Speaking</span>
                  )}
                  {taskTypes.has("writing") && (
                    <span className="rounded-full bg-amber-500/10 px-2.5 py-1 text-amber-300">Writing</span>
                  )}
                  {taskTypes.has("listening") && (
                    <span className="rounded-full bg-sky-500/10 px-2.5 py-1 text-sky-400">Listening</span>
                  )}
                </div>
              </div>

              {productionTasks.length > 0 ? (
                <div className="space-y-4">
                  {productionTasks.map((task, index) => (
                    <ProductionTask key={`${task.type}-${index}`} task={task} />
                  ))}
                </div>
              ) : (
                <div className="rounded-xl border border-primary/30 bg-primary/5 p-4">
                  <ul className="space-y-2 text-sm">
                    {production.map((item) => <li key={item}>{cleanItem(item, "Producción ")}</li>)}
                  </ul>
                </div>
              )}
            </section>
          )}

          <Link href="/review" className="inline-block rounded-lg bg-primary px-4 py-2 text-sm text-primary-foreground transition-colors hover:bg-primary-hover">
            Practicar en repaso diario
          </Link>
        </div>
      </PageContainer>
    </>
  );
}
