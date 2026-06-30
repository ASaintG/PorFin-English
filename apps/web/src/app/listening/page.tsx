"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type ListeningSummary } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";

export default function ListeningPage() {
  const [items, setItems] = useState<ListeningSummary[]>([]);

  useEffect(() => {
    api.get<ListeningSummary[]>("/listening/due?cefrLevel=A1").then((r) => setItems(r.data));
  }, []);

  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-3xl px-4 py-8">
        <h1 className="mb-6 text-2xl font-semibold">Listening</h1>
        <div className="space-y-3">
          {items.map((l) => (
            <Link key={l.id} href={`/listening/${l.id}`} className="block rounded-xl border border-zinc-200 p-4 hover:border-emerald-300 dark:border-zinc-800">
              <h2 className="font-medium">{l.title}</h2>
              <p className="text-sm text-zinc-500">{l.cefrLevel} · {l.category}</p>
            </Link>
          ))}
        </div>
      </main>
    </>
  );
}
