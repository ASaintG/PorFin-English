"use client";

import { useState } from "react";
import type { ComprehensionQuestion } from "@/api/client";

interface Props {
  questions: ComprehensionQuestion[];
  onSubmit: (answers: number[]) => void;
  disabled?: boolean;
}

export function ComprehensionQuestions({ questions, onSubmit, disabled }: Props) {
  const [answers, setAnswers] = useState<number[]>(questions.map(() => -1));

  return (
    <div className="space-y-4">
      {questions.map((q, qi) => (
        <div key={q.id} className="rounded-xl border border-zinc-200 p-4 dark:border-zinc-800">
          <p className="mb-2 font-medium">{q.question}</p>
          <div className="space-y-1">
            {q.options.map((opt, oi) => (
              <label key={opt} className="flex cursor-pointer items-center gap-2 text-sm">
                <input
                  type="radio"
                  name={q.id}
                  disabled={disabled}
                  checked={answers[qi] === oi}
                  onChange={() => {
                    const next = [...answers];
                    next[qi] = oi;
                    setAnswers(next);
                  }}
                />
                {opt}
              </label>
            ))}
          </div>
        </div>
      ))}
      <button
        type="button"
        disabled={disabled || answers.some((a) => a < 0)}
        onClick={() => onSubmit(answers)}
        className="rounded-lg bg-emerald-700 px-4 py-2 text-sm text-white disabled:opacity-40"
      >
        Enviar respuestas
      </button>
    </div>
  );
}
