import Link from "next/link";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function HelpPage() {
  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-3xl">
          <h1 className="mb-6 text-2xl font-semibold">Ayuda</h1>
          <div className="space-y-6">
            <section>
              <h2 className="mb-1 font-medium">¿Cómo funciona el repaso espaciado?</h2>
              <p className="text-sm text-muted-foreground">
                Cada palabra o ejercicio vuelve a aparecer en el momento ideal para que no lo
                olvides. Marca con honestidad si recordabas la respuesta.
              </p>
            </section>
            <section>
              <h2 className="mb-1 font-medium">¿Cómo avanzo de nivel?</h2>
              <p className="text-sm text-muted-foreground">
                Completa las lecciones y el simulador de examen. Tu progreso se actualiza en el
                dashboard según las habilidades practicadas.
              </p>
            </section>
            <section>
              <h2 className="mb-1 font-medium">¿No escucho el audio?</h2>
              <p className="text-sm text-muted-foreground">
                La pronunciación usa la voz del navegador. Prueba con Chrome o Edge y revisa que
                el volumen del sistema esté activo.
              </p>
            </section>
            <section>
              <h2 className="mb-1 font-medium">¿Dónde reporto un problema?</h2>
              <p className="text-sm text-muted-foreground">
                Escríbenos desde{" "}
                <Link href="/settings/feedback" className="text-primary hover:underline">
                  Feedback
                </Link>
                . Incluye el ejercicio o la pantalla donde ocurrió.
              </p>
            </section>
          </div>
        </div>
      </PageContainer>
    </>
  );
}
