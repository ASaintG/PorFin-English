"use client";

import { useEffect, useRef, useState } from "react";

interface ExamTimerProps {
  durationMinutes: number;
  started: boolean;
  onExpire: () => void;
}

export function ExamTimer({ durationMinutes, started, onExpire }: ExamTimerProps) {
  const [secondsLeft, setSecondsLeft] = useState(durationMinutes * 60);
  const expiredRef = useRef(false);
  const intervalRef = useRef<number | null>(null);
  const onExpireRef = useRef(onExpire);

  useEffect(() => {
    onExpireRef.current = onExpire;
  }, [onExpire]);

  useEffect(() => {
    if (!started) return;

    intervalRef.current = window.setInterval(() => {
      setSecondsLeft((s) => Math.max(0, s - 1));
    }, 1000);

    return () => {
      if (intervalRef.current !== null) {
        window.clearInterval(intervalRef.current);
        intervalRef.current = null;
      }
    };
  }, [started]);

  useEffect(() => {
    if (secondsLeft !== 0 || expiredRef.current) return;

    expiredRef.current = true;
    if (intervalRef.current !== null) {
      window.clearInterval(intervalRef.current);
      intervalRef.current = null;
    }
    onExpireRef.current();
  }, [secondsLeft]);

  const mins = Math.floor(secondsLeft / 60);
  const secs = secondsLeft % 60;
  const urgent = secondsLeft <= 300;

  return (
    <div className={`mb-4 rounded-lg border px-4 py-2 text-sm ${urgent ? "border-warning bg-warning/10 text-warning" : "border-border bg-card text-foreground"}`}>
      Tiempo restante: {mins}:{secs.toString().padStart(2, "0")}
      {urgent && secondsLeft > 0 && <span className="ml-2">¡Menos de 5 minutos!</span>}
      {secondsLeft === 0 && <span className="ml-2 font-medium">Tiempo agotado</span>}
    </div>
  );
}
