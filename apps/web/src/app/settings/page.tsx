import Link from "next/link";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function SettingsPage() {
  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-3xl">
          <h1 className="mb-6 text-2xl font-semibold">Ajustes</h1>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <Link
              href="/settings/feedback"
              className="rounded-xl border border-border bg-card p-5 transition-colors hover:border-primary/50"
            >
              <h2 className="mb-1 font-medium">Feedback</h2>
              <p className="text-sm text-muted-foreground">
                Envía sugerencias o reporta errores.
              </p>
            </Link>
            <Link
              href="/settings/attributions"
              className="rounded-xl border border-border bg-card p-5 transition-colors hover:border-primary/50"
            >
              <h2 className="mb-1 font-medium">Atribuciones</h2>
              <p className="text-sm text-muted-foreground">
                Fuentes del contenido y licencias.
              </p>
            </Link>
          </div>
        </div>
      </PageContainer>
    </>
  );
}
