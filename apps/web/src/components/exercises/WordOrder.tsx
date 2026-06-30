"use client";

import { useState } from "react";

interface WordOrderProps {
  words: string[];
  correctOrder: string[];
  onSubmit: (correct: boolean) => void;
  disabled?: boolean;
}

export function WordOrder({ words, correctOrder, onSubmit, disabled }: WordOrderProps) {
  const [selected, setSelected] = useState<string[]>([]);
  const remaining = words.filter((w) => !selected.includes(w));

  const handlePick = (word: string) => {
    if (disabled) return;
    setSelected([...selected, word]);
  };

  const handleSubmit = () => {
    const correct = selected.join(" ") === correctOrder.join(" ");
    onSubmit(correct);
    setSelected([]);
  };

  return (
    <div className="w-full max-w-lg rounded-2xl border border-zinc-200 bg-white p-6 dark:border-zinc-800 dark:bg-zinc-900">
      <p className="mb-2 text-sm text-zinc-500">Ordena las palabras:</p>
      <div className="mb-4 min-h-[2.5rem] rounded-lg bg-zinc-100 p-3 dark:bg-zinc-800">
        {selected.join(" ") || "..."}
      </div>
      <div className="mb-4 flex flex-wrap gap-2">
        {remaining.map((w) => (
          <button
            key={w}
            type="button"
            disabled={disabled}
            onClick={() => handlePick(w)}
            className="rounded-full border border-zinc-300 px-3 py-1 text-sm dark:border-zinc-600"
          >
            {w}
          </button>
        ))}
      </div>
      <button
        type="button"
        disabled={disabled || selected.length !== words.length}
        onClick={handleSubmit}
        className="rounded-lg bg-emerald-700 px-4 py-2 text-sm text-white disabled:opacity-40"
      >
        Comprobar
      </button>
    </div>
  );
}
