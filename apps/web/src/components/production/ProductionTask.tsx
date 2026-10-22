"use client";

import { useState } from "react";
import type { ProductionTask as ProductionTaskData } from "@/api/client";
import { useSpeech } from "@/hooks/useSpeech";
import { useSpeechRecognition } from "@/hooks/useSpeechRecognition";

interface IconProps {
  className?: string;
}

function MicIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M12 2a3 3 0 0 0-3 3v7a3 3 0 0 0 6 0V5a3 3 0 0 0-3-3Z" />
      <path d="M19 10v2a7 7 0 0 1-14 0v-2" />
      <path d="M12 19v3" />
    </svg>
  );
}

function PenIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M12 20h9" />
      <path d="M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4Z" />
    </svg>
  );
}

function HeadphonesIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className={className} aria-hidden="true">
      <path d="M3 18v-6a9 9 0 0 1 18 0v6" />
      <path d="M21 19a2 2 0 0 1-2 2h-1a2 2 0 0 1-2-2v-3a2 2 0 0 1 2-2h3Z" />
      <path d="M3 19a2 2 0 0 0 2 2h1a2 2 0 0 0 2-2v-3a2 2 0 0 0-2-2H3Z" />
    </svg>
  );
}

function PlayIcon({ className }: IconProps) {
  return (
    <svg viewBox="0 0 24 24" fill="currentColor" className={className} aria-hidden="true">
      <path d="M8 5.14v13.72a1 1 0 0 0 1.5.86l11-6.86a1 1 0 0 0 0-1.72l-11-6.86A1 1 0 0 0 8 5.14Z" />
    </svg>
  );
}

const TASK_CONFIG = {
  speaking: {
    label: "Speaking",
    description: "Practica tu expresión oral.",
    wrapper: "border-violet-500/30 bg-violet-500/5",
    iconWrapper: "bg-violet-500/15 text-violet-300",
    badge: "bg-violet-500/15 text-violet-300",
    Icon: MicIcon,
  },
  writing: {
    label: "Writing",
    description: "Practica tu expresión escrita.",
    wrapper: "border-amber-500/30 bg-amber-500/5",
    iconWrapper: "bg-amber-500/15 text-amber-300",
    badge: "bg-amber-500/15 text-amber-300",
    Icon: PenIcon,
  },
  listening: {
    label: "Listening",
    description: "Escucha y comprueba tu comprensión.",
    wrapper: "border-sky-500/30 bg-sky-500/5",
    iconWrapper: "bg-sky-500/15 text-sky-400",
    badge: "bg-sky-500/15 text-sky-400",
    Icon: HeadphonesIcon,
  },
} as const;

type TaskType = keyof typeof TASK_CONFIG;

function normalize(value: string) {
  return value
    .toLowerCase()
    .replace(/[^a-z0-9'\s]/g, "")
    .replace(/\s+/g, " ")
    .trim();
}

export function ProductionTask({ task }: { task: ProductionTaskData }) {
  const type = (task.type.toLowerCase() as TaskType) in TASK_CONFIG
    ? (task.type.toLowerCase() as TaskType)
    : "writing";
  const config = TASK_CONFIG[type];
  const { Icon } = config;

  const [answer, setAnswer] = useState("");
  const [feedback, setFeedback] = useState<{ ok: boolean; message: string } | null>(null);
  const [showTranscript, setShowTranscript] = useState(false);

  const { speak, supported: ttsSupported } = useSpeech();
  const {
    supported: sttSupported,
    listening,
    transcript,
    start: startRecording,
  } = useSpeechRecognition();

  const isListening = type === "listening";
  const isSpeaking = type === "speaking";
  const wordCount = answer.split(/\s+/).filter(Boolean).length;

  const playAudio = () => {
    if (task.audioUrl) {
      void new Audio(task.audioUrl).play().catch(() => {});
      return;
    }
    if (task.ttsText) speak(task.ttsText);
  };

  const checkDictation = () => {
    if (!task.ttsText) return;
    const expected = normalize(task.ttsText);
    const provided = normalize(answer);

    if (!provided) {
      setFeedback({ ok: false, message: "Escribe lo que escuchaste antes de comprobar." });
      return;
    }

    setFeedback(
      provided === expected
        ? { ok: true, message: "¡Correcto! Has transcrito la oración completa." }
        : { ok: false, message: "Casi. Vuelve a escuchar y revisa las palabras que faltan." },
    );
  };

  return (
    <article className={`rounded-2xl border p-5 ${config.wrapper}`}>
      <div className="flex items-start gap-3">
        <span className={`inline-flex h-10 w-10 shrink-0 items-center justify-center rounded-xl ${config.iconWrapper}`}>
          <Icon className="h-5 w-5" />
        </span>
        <div className="min-w-0">
          <div className="flex flex-wrap items-center gap-2">
            <h3 className="font-semibold">{config.label}</h3>
            <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${config.badge}`}>
              {config.description}
            </span>
          </div>
          <p className="mt-1 text-sm leading-relaxed text-muted-foreground">{task.prompt}</p>
        </div>
      </div>

      {isListening && (
        <div className="mt-4">
          <button
            type="button"
            onClick={playAudio}
            disabled={!task.audioUrl && !ttsSupported}
            className="inline-flex items-center gap-2 rounded-xl bg-sky-500 px-4 py-2.5 text-sm font-semibold text-white transition-colors hover:bg-sky-400 disabled:cursor-not-allowed disabled:opacity-40"
          >
            <PlayIcon className="h-4 w-4" />
            Reproducir audio
          </button>
          {!task.audioUrl && !ttsSupported && (
            <p className="mt-2 text-xs text-warning">
              Tu navegador no permite reproducir el audio TTS.
            </p>
          )}
        </div>
      )}

      {task.questions.length > 0 && (
        <ul className="mt-4 space-y-1 text-sm">
          {task.questions.map((question) => (
            <li key={question} className="flex gap-2">
              <span className="text-muted-foreground">•</span>
              <span>{question}</span>
            </li>
          ))}
        </ul>
      )}

      {isSpeaking && sttSupported && (
        <div className="mt-4 flex flex-wrap items-center gap-3">
          <button
            type="button"
            onClick={startRecording}
            disabled={listening}
            className="rounded-xl border border-violet-500/40 bg-violet-500/10 px-4 py-2 text-sm font-medium text-violet-300 transition-colors hover:bg-violet-500/20 disabled:opacity-40"
          >
            {listening ? "Escuchando..." : "Grabar respuesta"}
          </button>
          {transcript && (
            <button
              type="button"
              onClick={() => setAnswer(transcript)}
              className="text-sm text-muted-foreground underline underline-offset-2 hover:text-foreground"
            >
              Usar transcripción
            </button>
          )}
        </div>
      )}

      {isSpeaking && transcript && (
        <p className="mt-2 rounded-lg bg-background/60 p-3 text-sm text-muted-foreground">
          {transcript}
        </p>
      )}

      <textarea
        value={answer}
        onChange={(event) => {
          setAnswer(event.target.value);
          setFeedback(null);
        }}
        rows={isListening ? 3 : 5}
        className="mt-4 w-full rounded-xl border border-input bg-background p-3 text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none"
        placeholder={
          isListening
            ? "Escribe exactamente la oración que escuchaste..."
            : isSpeaking
              ? "Escribe tu respuesta o usa el micrófono..."
              : "Escribe tu respuesta en inglés..."
        }
      />

      <div className="mt-2 flex flex-wrap items-center justify-between gap-3">
        <span className="text-xs text-muted-foreground">{wordCount} palabras</span>
        <div className="flex items-center gap-3">
          {isListening && task.ttsText && (
            <button
              type="button"
              onClick={() => setShowTranscript((value) => !value)}
              className="text-xs text-muted-foreground underline underline-offset-2 hover:text-foreground"
            >
              {showTranscript ? "Ocultar transcripción" : "Ver transcripción"}
            </button>
          )}
          {isListening && (
            <button
              type="button"
              onClick={checkDictation}
              className="rounded-lg border border-sky-500/40 px-3 py-1.5 text-sm font-medium text-sky-400 transition-colors hover:bg-sky-500/10"
            >
              Comprobar dictado
            </button>
          )}
        </div>
      </div>

      {showTranscript && task.ttsText && (
        <p className="mt-3 rounded-lg border border-border bg-background/60 p-3 text-sm italic text-muted-foreground">
          {task.ttsText}
        </p>
      )}

      {feedback && (
        <p
          role="status"
          className={`mt-3 rounded-lg px-3 py-2 text-sm ${
            feedback.ok
              ? "bg-success/10 text-success"
              : "bg-warning/10 text-warning"
          }`}
        >
          {feedback.message}
        </p>
      )}
    </article>
  );
}
