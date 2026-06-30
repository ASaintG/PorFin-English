"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type ProgressStats } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
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
      <main className="mx-auto max-w-5xl px-4 py-8">
        <h1 className="mb-2 text-2xl font-semibold">Dashboard</h1>
        <p className="mb-6 text-zinc-600">
          Nivel {currentLevel ?? stats?.currentLevel} · Subnivel {currentSubLevel ?? stats?.currentSubLevel} ·{" "}
          {stats?.totalPracticeMinutes ?? 0} min practicados
        </p>

        <div className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <Stat label="Vocabulario" value={stats?.wordsMastered ?? "—"} />
          <Stat label="Gramática" value={stats?.grammarMastered ?? "—"} />
          <Stat label="Lectura" value={stats?.readingCompleted ?? "—"} />
          <Stat label="Listening" value={stats?.listeningCompleted ?? "—"} />
        </div>
        <div className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <Stat label="Speaking" value={stats?.speakingCompleted ?? "—"} />
          <Stat label="Writing" value={stats?.writingCompleted ?? "—"} />
          <Stat label="Racha" value={stats?.streak ?? 0} />
          <Stat label="Pendientes hoy" value={stats?.wordsDueToday ?? "—"} />
        </div>

        {stats?.skills && (
          <div className="mb-8 space-y-3">
            <h2 className="font-medium">Progreso por habilidad</h2>
            {stats.skills.map((s) => (
              <div key={s.skill}>
                <div className="mb-1 flex justify-between text-sm">
                  <span>{s.skill}</span>
                  <span>{s.percent}%</span>
                </div>
                <div className="h-2 rounded-full bg-zinc-200 dark:bg-zinc-800">
                  <div className="h-full rounded-full bg-emerald-600" style={{ width: `${s.percent}%` }} />
                </div>
              </div>
            ))}
          </div>
        )}

        <div className="grid gap-4 sm:grid-cols-2">
          <ActionCard title="Sesión diaria (~15 min)" href="/review" desc="Repaso vocabulario + gramática" />
          <ActionCard title="Ruta de aprendizaje" href="/path" desc="Unidades CEFR desbloqueables" />
          <ActionCard title="Lecciones temáticas" href="/lessons" desc="Restaurante, trabajo, direcciones..." />
          <ActionCard title="Lectura y listening" href="/reading" desc="Comprensión con TTS" />
          <ActionCard title="Speaking y writing" href="/speaking" desc="Producción oral y escrita B1" />
          <ActionCard title="Simulador de examen" href="/exam" desc="Formato Cambridge simplificado" />
        </div>
      </main>
    </>
  );
}

function Stat({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="rounded-xl border border-zinc-200 bg-white p-4 dark:border-zinc-800 dark:bg-zinc-900">
      <p className="text-sm text-zinc-500">{label}</p>
      <p className="text-2xl font-semibold">{value}</p>
    </div>
  );
}

function ActionCard({ title, href, desc }: { title: string; href: string; desc: string }) {
  return (
    <Link href={href} className="block rounded-xl border border-zinc-200 bg-white p-5 hover:border-emerald-300 dark:border-zinc-800 dark:bg-zinc-900">
      <h3 className="font-medium">{title}</h3>
      <p className="mt-1 text-sm text-zinc-500">{desc}</p>
    </Link>
  );
}
