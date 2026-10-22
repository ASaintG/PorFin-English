"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import api, { type PlacementQuestion } from "@/api/client";
import { FillBlank } from "@/components/exercises/FillBlank";
import { PageContainer } from "@/components/layout/PageContainer";
import { useAuthStore } from "@/stores/authStore";

export default function PlacementPage() {
  const router = useRouter();
  const { placementCompleted, token, email, userId, setAuth } = useAuthStore();
  const [questions, setQuestions] = useState<PlacementQuestion[]>([]);
  const [index, setIndex] = useState(0);
  const [correct, setCorrect] = useState(0);
  const [level, setLevel] = useState("A1");

  useEffect(() => {
    if (placementCompleted) router.replace("/dashboard");
    else api.get<PlacementQuestion[]>("/placement/questions").then((r) => setQuestions(r.data));
  }, [placementCompleted, router]);

  const finish = async (finalCorrect: number) => {
    const { data } = await api.post<{ assignedLevel: string; assignedSubLevel: string }>("/placement/complete", {
      correct: finalCorrect,
      total: questions.length,
      finalLevel: level,
    });
    if (token && userId && email) {
      await setAuth({
        token,
        userId,
        email,
        currentLevel: data.assignedLevel,
        currentSubLevel: data.assignedSubLevel,
        placementCompleted: true,
      });
    }
    router.push("/dashboard");
  };

  const handleAnswer = (isCorrect: boolean) => {
    const newCorrect = correct + (isCorrect ? 1 : 0);
    setCorrect(newCorrect);
    const next = index + 1;
    if (next >= questions.length) finish(newCorrect);
    else {
      if (next % 2 === 0 && isCorrect) {
        const levels = ["A1", "A2", "B1", "B2"];
        const li = levels.indexOf(level);
        if (li < levels.length - 1) setLevel(levels[li + 1]);
      }
      setIndex(next);
    }
  };

  const q = questions[index];
  if (!q) return <PageContainer className="flex min-h-screen items-center justify-center">Cargando test...</PageContainer>;

  return (
    <PageContainer className="flex min-h-screen flex-col justify-center">
      <div className="mx-auto w-full max-w-lg">
        <h1 className="mb-2 text-2xl font-semibold">Test de nivel</h1>
        <p className="mb-6 text-sm text-muted-foreground">Pregunta {index + 1} de {questions.length} · Nivel actual: {level}</p>
        <FillBlank content={q.content} onSubmit={handleAnswer} />
      </div>
    </PageContainer>
  );
}
