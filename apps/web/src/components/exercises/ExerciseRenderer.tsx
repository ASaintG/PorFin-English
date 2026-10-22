"use client";

import type {
  ExerciseContent,
  FillBlankContent,
  MultipleChoiceContent,
  WordOrderContent,
} from "@/api/client";
import { FillBlank } from "@/components/exercises/FillBlank";
import { MultipleChoice } from "@/components/exercises/MultipleChoice";
import { WordOrder } from "@/components/exercises/WordOrder";

interface ExerciseRendererProps {
  type: string;
  content: ExerciseContent;
  onSubmit: (correct: boolean) => void;
  disabled?: boolean;
}

export function ExerciseRenderer({ type, content, onSubmit, disabled }: ExerciseRendererProps) {
  const normalized = type.toLowerCase();

  if (normalized === "multiplechoice" || normalized === "multiple-choice") {
    const value = content as MultipleChoiceContent;
    return (
      <MultipleChoice
        question={value.question}
        options={value.options}
        correctIndex={value.correctIndex}
        onSubmit={onSubmit}
        disabled={disabled}
      />
    );
  }

  if (normalized === "wordorder" || normalized === "word-order") {
    const value = content as WordOrderContent;
    return (
      <WordOrder
        words={value.words}
        correctOrder={value.correctOrder}
        onSubmit={onSubmit}
        disabled={disabled}
      />
    );
  }

  return <FillBlank content={content as FillBlankContent} onSubmit={onSubmit} disabled={disabled} />;
}
