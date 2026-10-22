"use client";

interface MultipleChoiceProps {
  question: string;
  options: string[];
  correctIndex: number;
  onSubmit: (correct: boolean) => void;
  disabled?: boolean;
}

export function MultipleChoice({ question, options, correctIndex, onSubmit, disabled }: MultipleChoiceProps) {
  return (
    <div className="w-full max-w-lg rounded-2xl border border-border bg-card p-6">
      <p className="mb-4 font-medium">{question}</p>
      <div className="space-y-2">
        {options.map((opt, i) => (
          <button
            key={opt}
            type="button"
            disabled={disabled}
            onClick={() => onSubmit(i === correctIndex)}
            className="block w-full rounded-lg border border-border bg-background px-4 py-2 text-left text-sm transition-colors hover:border-primary/50 hover:bg-primary/10"
          >
            {opt}
          </button>
        ))}
      </div>
    </div>
  );
}
