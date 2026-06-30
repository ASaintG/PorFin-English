import { AppHeader } from "@/components/layout/AppHeader";

export default function AttributionsPage() {
  return (
    <>
      <AppHeader />
      <main className="mx-auto max-w-2xl px-4 py-8">
        <h1 className="mb-4 text-2xl font-semibold">Atribuciones de contenido</h1>
        <ul className="space-y-3 text-sm text-zinc-600">
          <li>Vocabulario A1/A2: listas CEFR open-source y frases adaptadas.</li>
          <li>Tatoeba (CC BY 2.0): frases de ejemplo bilingües.</li>
          <li>Project Gutenberg: textos de dominio público adaptados para niveles A2–B1.</li>
          <li>Audio: generado con Web Speech API del navegador (sin archivos pregrabados).</li>
        </ul>
      </main>
    </>
  );
}
