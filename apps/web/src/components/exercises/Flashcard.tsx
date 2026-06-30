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
    <div className="w-full max-w-lg rounded-2xl border border-zinc-200 bg-white p-8 shadow-sm dark:border-zinc-800 dark:bg-zinc-900">
      <div className="mb-2 text-xs font-medium uppercase tracking-wider text-zinc-500">
        {category} · {partOfSpeech}
      </div>

      <div className="mb-6 flex items-center justify-between gap-4">
        <h2 className="text-4xl font-semibold text-zinc-900 dark:text-zinc-50">{word}</h2>
        {supported && (
          <button
            type="button"
            onClick={handleSpeak}
            className="rounded-full border border-zinc-200 px-3 py-2 text-sm hover:bg-zinc-50 dark:border-zinc-700 dark:hover:bg-zinc-800"
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
          className="mb-6 w-full rounded-xl bg-zinc-100 py-3 text-sm font-medium text-zinc-700 hover:bg-zinc-200 dark:bg-zinc-800 dark:text-zinc-200 dark:hover:bg-zinc-700"
        >
          Mostrar traducción
        </button>
      ) : (
        <div className="mb-6 space-y-3">
          <p className="text-2xl text-emerald-700 dark:text-emerald-400">{translation}</p>
          {exampleSentences[0] && (
            <p className="text-sm italic text-zinc-600 dark:text-zinc-400">
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
          className="rounded-xl border border-red-200 bg-red-50 py-3 font-medium text-red-700 transition hover:bg-red-100 disabled:cursor-not-allowed disabled:opacity-40 dark:border-red-900 dark:bg-red-950 dark:text-red-300"
        >
          No sé
        </button>
        <button
          type="button"
          disabled={!revealed || disabled}
          onClick={onKnow}
          className="rounded-xl border border-emerald-200 bg-emerald-50 py-3 font-medium text-emerald-700 transition hover:bg-emerald-100 disabled:cursor-not-allowed disabled:opacity-40 dark:border-emerald-900 dark:bg-emerald-950 dark:text-emerald-300"
        >
          Sé
        </button>
      </div>
    </div>
  );
}
