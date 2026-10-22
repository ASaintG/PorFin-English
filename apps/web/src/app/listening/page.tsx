"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import api, { type ListeningSummary } from "@/api/client";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function ListeningPage() {
  const [items, setItems] = useState<ListeningSummary[]>([]);

  useEffect(() => {
    api.get<ListeningSummary[]>("/listening/due?cefrLevel=A1").then((r) => setItems(r.data));
  }, []);

  return (
    <>
      <AppHeader />
      <PageContainer>
        <h1 className="mb-6 text-2xl font-semibold">Listening</h1>
        <div className="grid gap-3 md:grid-cols-2 xl:grid-cols-3">
          {items.map((l) => (
            <Link key={l.id} href={`/listening/${l.id}`} className="block rounded-xl border border-border bg-card p-4 transition-colors hover:border-primary/50">
              <h2 className="font-medium">{l.title}</h2>
              <p className="text-sm text-muted-foreground">{l.cefrLevel} · {l.category}</p>
            </Link>
          ))}
        </div>
      </PageContainer>
    </>
  );
}
