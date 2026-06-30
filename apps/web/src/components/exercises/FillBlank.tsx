"use client";

import type { FillBlankContent } from "@/api/client";

interface FillBlankProps {
  content: FillBlankContent;
  onSubmit: (correct: boolean) => void;
  disabled?: boolean;
}

export function FillBlank({ content, onSubmit, disabled }: FillBlankProps) {
  const handleSelect = (index: number) => {
    if (disabled) return;
    onSubmit(index === content.correctIndex);
  };

  return (
    <div className="w-full max-w-lg rounded-2xl border border-zinc-200 bg-white p-6 dark:border-zinc-800 dark:bg-zinc-900">
      <p className="mb-4 text-lg">{content.sentence}</p>
      <div className="grid grid-cols-2 gap-2">
        {content.options.map((opt, i) => (
          <button
            key={opt}
            type="button"
            disabled={disabled}
            onClick={() => handleSelect(i)}
            className="rounded-lg border border-zinc-200 px-3 py-2 text-sm hover:bg-emerald-50 dark:border-zinc-700 dark:hover:bg-emerald-950"
          >
            {opt}
          </button>
        ))}
      </div>
    </div>
  );
}
