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
    <div className="w-full max-w-lg rounded-2xl border border-border bg-card p-6">
      <p className="mb-4 text-lg">{content.sentence}</p>
      <div className="grid grid-cols-2 gap-2">
        {content.options.map((opt, i) => (
          <button
            key={opt}
            type="button"
            disabled={disabled}
            onClick={() => handleSelect(i)}
            className="rounded-lg border border-border bg-background px-3 py-2 text-sm transition-colors hover:border-primary/50 hover:bg-primary/10"
          >
            {opt}
          </button>
        ))}
      </div>
    </div>
  );
}
