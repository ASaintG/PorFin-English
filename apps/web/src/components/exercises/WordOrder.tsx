"use client";

import { useState } from "react";

interface WordOrderProps {
  words: string[];
  correctOrder: string[];
  onSubmit: (correct: boolean) => void;
  disabled?: boolean;
}

export function WordOrder({ words, correctOrder, onSubmit, disabled }: WordOrderProps) {
  const [selected, setSelected] = useState<number[]>([]);
  const remaining = words.map((word, index) => ({ word, index })).filter(({ index }) => !selected.includes(index));

  const handlePick = (index: number) => {
    if (disabled) return;
    setSelected([...selected, index]);
  };

  const handleSubmit = () => {
    const correct = selected.map((index) => words[index]).join(" ") === correctOrder.join(" ");
    onSubmit(correct);
    setSelected([]);
  };

  return (
    <div className="w-full max-w-lg rounded-2xl border border-border bg-card p-6">
      <p className="mb-2 text-sm text-muted-foreground">Ordena las palabras:</p>
      <div className="mb-4 min-h-[2.5rem] rounded-lg bg-muted p-3">
        {selected.map((index) => words[index]).join(" ") || "..."}
      </div>
      <div className="mb-4 flex flex-wrap gap-2">
        {remaining.map(({ word, index }) => (
          <button
            key={index}
            type="button"
            disabled={disabled}
            onClick={() => handlePick(index)}
            className="rounded-full border border-border bg-background px-3 py-1 text-sm transition-colors hover:border-primary/50"
          >
            {word}
          </button>
        ))}
      </div>
      <button
        type="button"
        disabled={disabled || selected.length !== words.length}
        onClick={handleSubmit}
        className="rounded-lg bg-primary px-4 py-2 text-sm text-primary-foreground transition-colors hover:bg-primary-hover disabled:opacity-40"
      >
        Comprobar
      </button>
    </div>
  );
}
