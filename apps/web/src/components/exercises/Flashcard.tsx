"use client";

import { useState } from "react";
import { useSpeech } from "@/hooks/useSpeech";

interface FlashcardProps {
  word: string;
  translation: string;
  partOfSpeech: string;
  category: string;
  exampleSentences: string[];
  onKnow: () => void;
  onDontKnow: () => void;
  disabled?: boolean;
}

export function Flashcard({
  word,
  translation,
  partOfSpeech,
  category,
  exampleSentences,
  onKnow,
  onDontKnow,
  disabled,
}: FlashcardProps) {
  const [revealed, setRevealed] = useState(false);
  const { speak, supported } = useSpeech();

  const handleReveal = () => setRevealed(true);

  const handleSpeak = () => speak(word);

  return (
    <div className="w-full max-w-lg rounded-2xl border border-border bg-card p-8 shadow-sm">
      <div className="mb-2 text-xs font-medium uppercase tracking-wider text-muted-foreground">
        {category} · {partOfSpeech}
      </div>

      <div className="mb-6 flex items-center justify-between gap-4">
        <h2 className="text-4xl font-semibold text-foreground">{word}</h2>
        {supported && (
          <button
            type="button"
            onClick={handleSpeak}
            className="rounded-full border border-border px-3 py-2 text-sm transition-colors hover:bg-muted"
            aria-label="Escuchar pronunciación"
          >
            🔊
          </button>
        )}
      </div>

      {!revealed ? (
        <button
          type="button"
          onClick={handleReveal}
          className="mb-6 w-full rounded-xl bg-muted py-3 text-sm font-medium text-foreground transition-colors hover:bg-muted/80"
        >
          Mostrar traducción
        </button>
      ) : (
        <div className="mb-6 space-y-3">
          <p className="text-2xl text-foreground">{translation}</p>
          {exampleSentences[0] && (
            <p className="text-sm italic text-muted-foreground">
              &ldquo;{exampleSentences[0]}&rdquo;
            </p>
          )}
        </div>
      )}

      <div className="grid grid-cols-2 gap-3">
        <button
          type="button"
          disabled={!revealed || disabled}
          onClick={onDontKnow}
          className="rounded-xl border border-destructive/30 bg-destructive/10 py-3 font-medium text-destructive transition hover:bg-destructive/20 disabled:cursor-not-allowed disabled:opacity-40"
        >
          No sé
        </button>
        <button
          type="button"
          disabled={!revealed || disabled}
          onClick={onKnow}
          className="rounded-xl border border-success/30 bg-success/10 py-3 font-medium text-success transition hover:bg-success/20 disabled:cursor-not-allowed disabled:opacity-40"
        >
          Sé
        </button>
      </div>
    </div>
  );
}
