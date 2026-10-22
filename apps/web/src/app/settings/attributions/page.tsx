import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

export default function AttributionsPage() {
  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-3xl">
          <h1 className="mb-4 text-2xl font-semibold">Atribuciones de contenido</h1>
          <ul className="space-y-3 text-sm text-muted-foreground">
            <li>Vocabulario A1/A2: listas CEFR open-source y frases adaptadas.</li>
            <li>Tatoeba (CC BY 2.0): frases de ejemplo bilingües.</li>
            <li>Project Gutenberg: textos de dominio público adaptados para niveles A2–B1.</li>
            <li>Audio: generado con Web Speech API del navegador (sin archivos pregrabados).</li>
          </ul>
        </div>
      </PageContainer>
    </>
  );
}
