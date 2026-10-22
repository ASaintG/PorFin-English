"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type ProgressStats } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";
import { useAuthStore } from "@/stores/authStore";

export default function DashboardPage() {
  const { currentLevel, currentSubLevel } = useAuthStore();
  const [stats, setStats] = useState<ProgressStats | null>(null);

  useEffect(() => {
    api.get<ProgressStats>("/progress/stats").then((r) => setStats(r.data)).catch(() => {});
  }, []);

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="space-y-10">
          <header>
            <h1 className="mb-2 text-2xl font-semibold">Dashboard</h1>
            <p className="text-muted-foreground">
              Nivel {currentLevel ?? stats?.currentLevel} · Subnivel {currentSubLevel ?? stats?.currentSubLevel} ·{" "}
              {stats?.totalPracticeMinutes ?? 0} min practicados
            </p>
          </header>

          <section aria-label="Estadísticas">
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
              <Stat label="Vocabulario" value={stats?.wordsMastered ?? "—"} />
              <Stat label="Gramática" value={stats?.grammarMastered ?? "—"} />
              <Stat label="Lectura" value={stats?.readingCompleted ?? "—"} />
              <Stat label="Listening" value={stats?.listeningCompleted ?? "—"} />
              <Stat label="Speaking" value={stats?.speakingCompleted ?? "—"} />
              <Stat label="Writing" value={stats?.writingCompleted ?? "—"} />
              <Stat label="Racha" value={stats?.streak ?? 0} highlight />
              <Stat label="Pendientes hoy" value={stats?.wordsDueToday ?? "—"} highlight />
            </div>
          </section>

          {stats?.skills && (
            <section>
              <h2 className="mb-4 font-medium">Progreso por habilidad</h2>
              <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
                {stats.skills.map((s) => (
                  <div
                    key={s.skill}
                    className="rounded-xl border border-border bg-card p-4"
                  >
                    <div className="mb-2 flex justify-between text-sm">
                      <span>{s.skill}</span>
                      <span className="text-muted-foreground">{s.percent}%</span>
                    </div>
                    <div className="h-2 rounded-full bg-progress-track">
                      <div className="h-full rounded-full bg-success" style={{ width: `${s.percent}%` }} />
                    </div>
                  </div>
                ))}
              </div>
            </section>
          )}

          <section aria-label="Acciones">
            <div className="grid grid-cols-1 gap-5 md:grid-cols-2 lg:grid-cols-4">
              <ActionCard title="Sesión diaria (~15 min)" href="/review" desc="Repaso vocabulario + gramática" />
              <ActionCard title="Ruta de aprendizaje" href="/path" desc="Unidades CEFR por nivel" />
              <ActionCard title="Lecciones temáticas" href="/lessons" desc="Restaurante, trabajo, direcciones..." />
              <ActionCard title="Lectura y listening" href="/reading" desc="Comprensión con TTS" />
              <ActionCard title="Speaking y writing" href="/speaking" desc="Producción oral y escrita B1" />
              <ActionCard title="Simulador de examen" href="/exam" desc="Formato Cambridge simplificado" />
            </div>
          </section>
        </div>
      </PageContainer>
    </>
  );
}

function Stat({ label, value, highlight }: { label: string; value: string | number; highlight?: boolean }) {
  return (
    <div className="h-full rounded-xl border border-border bg-card p-4">
      <p className="text-sm text-muted-foreground">{label}</p>
      <p className={`text-2xl font-semibold ${highlight ? "text-warning" : "text-foreground"}`}>{value}</p>
    </div>
  );
}

function ActionCard({ title, href, desc }: { title: string; href: string; desc: string }) {
  return (
    <Link
      href={href}
      className="flex h-full flex-col rounded-xl border border-border bg-card p-5 transition-colors hover:border-primary/50"
    >
      <h3 className="font-medium">{title}</h3>
      <p className="mt-1 text-sm text-muted-foreground">{desc}</p>
    </Link>
  );
}
