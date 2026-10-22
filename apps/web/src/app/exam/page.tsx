"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import api, { type ComprehensionQuestion, type FillBlankContent } from "@/api/client";
import { ExamTimer } from "@/components/exam/ExamTimer";
import { ComprehensionQuestions } from "@/components/exercises/ComprehensionQuestions";
import { FillBlank } from "@/components/exercises/FillBlank";
import { Flashcard } from "@/components/exercises/Flashcard";
import { AppHeader } from "@/components/layout/AppHeader";
import { PageContainer } from "@/components/layout/PageContainer";

type ExamItem =
  | { kind: "vocab"; id: string; skill: string; word: string; translation: string; partOfSpeech: string; category: string }
  | { kind: "grammar"; id: string; skill: string; content: FillBlankContent }
  | { kind: "comprehension"; id: string; skill: string; title: string; transcript?: string; questions: (ComprehensionQuestion & { correctIndex: number })[] };

export default function ExamPage() {
  const [started, setStarted] = useState(false);
  const [durationMinutes, setDurationMinutes] = useState(30);
  const [items, setItems] = useState<ExamItem[]>([]);
  const [index, setIndex] = useState(0);
  const [answers, setAnswers] = useState<{ itemId: string; skill: string; correct: boolean }[]>([]);
  const [result, setResult] = useState<{ scorePercent: number; sectionScores: { section: string; correct: number; total: number }[] } | null>(null);
  const [expired, setExpired] = useState(false);
  const answersRef = useRef(answers);
  const indexRef = useRef(index);
  const itemsRef = useRef(items);

  useEffect(() => {
    answersRef.current = answers;
    indexRef.current = index;
    itemsRef.current = items;
  }, [answers, index, items]);

  const submitExam = useCallback(async (allAnswers: typeof answers) => {
    const { data } = await api.post<typeof result>("/exam/submit", { answers: allAnswers });
    setResult(data);
    setExpired(true);
  }, []);

  const handleExpire = useCallback(() => {
    const remaining = itemsRef.current.slice(indexRef.current).map((item) => ({
      itemId: item.id,
      skill: item.skill,
      correct: false,
    }));
    submitExam([...answersRef.current, ...remaining]);
  }, [submitExam]);

  const start = async () => {
    const { data } = await api.post<{ durationMinutes: number; sections: { section: string; skill: string; items: Record<string, unknown>[] }[] }>("/exam/start");
    const flat: ExamItem[] = [];
    for (const sec of data.sections) {
      for (const raw of sec.items) {
        if (raw.word && raw.itemId) {
          flat.push({
            kind: "vocab",
            id: String(raw.itemId),
            skill: sec.skill,
            word: String(raw.word),
            translation: String(raw.translation ?? ""),
            partOfSpeech: String(raw.partOfSpeech ?? ""),
            category: String(raw.category ?? ""),
          });
        } else if (raw.content) {
          flat.push({
            kind: "grammar",
            id: String(raw.id),
            skill: sec.skill,
            content: raw.content as FillBlankContent,
          });
        } else if (raw.questions) {
          flat.push({
            kind: "comprehension",
            id: String(raw.id),
            skill: sec.skill,
            title: String(raw.title ?? "Comprehension"),
            transcript: raw.transcript ? String(raw.transcript) : undefined,
            questions: (raw.questions as (ComprehensionQuestion & { correctIndex: number })[]),
          });
        }
      }
    }
    setDurationMinutes(data.durationMinutes ?? 30);
    setItems(flat);
    setStarted(true);
    setIndex(0);
    setAnswers([]);
    setResult(null);
    setExpired(false);
  };

  const advance = (correct: boolean) => {
    if (expired || result) return;
    const item = items[index];
    const next = [...answers, { itemId: item.id, skill: item.skill, correct }];
    if (index < items.length - 1) {
      setAnswers(next);
      setIndex(index + 1);
    } else {
      submitExam(next);
    }
  };

  const item = items[index];

  return (
    <>
      <AppHeader />
      <PageContainer>
        <div className="mx-auto w-full max-w-lg">
        <h1 className="mb-4 text-2xl font-semibold">Simulador de examen</h1>
        {!started && (
          <button type="button" onClick={start} className="rounded-lg bg-primary px-5 py-2 text-primary-foreground transition-colors hover:bg-primary-hover">
            Iniciar examen (~30 min)
          </button>
        )}
        {started && !result && (
          <ExamTimer durationMinutes={durationMinutes} started={started && !result} onExpire={handleExpire} />
        )}
        {started && !result && item && !expired && (
          <>
            <p className="mb-4 text-sm text-muted-foreground">Pregunta {index + 1} de {items.length} · {item.skill}</p>
            {item.kind === "vocab" && (
              <Flashcard
                word={item.word}
                translation={item.translation}
                partOfSpeech={item.partOfSpeech}
                category={item.category}
                exampleSentences={[]}
                onKnow={() => advance(true)}
                onDontKnow={() => advance(false)}
              />
            )}
            {item.kind === "grammar" && <FillBlank content={item.content} onSubmit={advance} />}
            {item.kind === "comprehension" && (
              <div>
                <h2 className="mb-2 font-medium">{item.title}</h2>
                {item.transcript && <p className="mb-4 text-sm italic text-muted-foreground">{item.transcript}</p>}
                <ComprehensionQuestions
                  questions={item.questions}
                  onSubmit={(selected) => {
                    const correct = item.questions.every((q, i) => selected[i] === q.correctIndex);
                    advance(correct);
                  }}
                />
              </div>
            )}
          </>
        )}
        {result && (
          <div>
            <p className="text-xl font-semibold">Resultado: {result.scorePercent}%</p>
            {result.sectionScores.map((s) => (
              <p key={s.section} className="text-sm text-muted-foreground">{s.section}: {s.correct}/{s.total}</p>
            ))}
          </div>
        )}
        </div>
      </PageContainer>
    </>
  );
}
