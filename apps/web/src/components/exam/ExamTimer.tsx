"use client";

import { useCallback, useEffect, useRef, useState } from "react";

interface ExamTimerProps {
  durationMinutes: number;
  started: boolean;
  onExpire: () => void;
}

export function ExamTimer({ durationMinutes, started, onExpire }: ExamTimerProps) {
  const [secondsLeft, setSecondsLeft] = useState(durationMinutes * 60);
  const expiredRef = useRef(false);

  const onExpireRef = useRef(onExpire);
  onExpireRef.current = onExpire;

  useEffect(() => {
    if (!started) return;
    setSecondsLeft(durationMinutes * 60);
    expiredRef.current = false;
  }, [started, durationMinutes]);

  useEffect(() => {
    if (!started || secondsLeft <= 0) return;

    const id = window.setInterval(() => {
      setSecondsLeft((s) => {
        if (s <= 1) {
          if (!expiredRef.current) {
            expiredRef.current = true;
            onExpireRef.current();
          }
          return 0;
        }
        return s - 1;
      });
    }, 1000);

    return () => window.clearInterval(id);
  }, [started, secondsLeft]);

  const mins = Math.floor(secondsLeft / 60);
  const secs = secondsLeft % 60;
  const urgent = secondsLeft <= 300;

  return (
    <div className={`mb-4 rounded-lg border px-4 py-2 text-sm ${urgent ? "border-amber-500 bg-amber-50 text-amber-800" : "border-zinc-200"}`}>
      Tiempo restante: {mins}:{secs.toString().padStart(2, "0")}
      {urgent && secondsLeft > 0 && <span className="ml-2">¡Menos de 5 minutos!</span>}
      {secondsLeft === 0 && <span className="ml-2 font-medium">Tiempo agotado</span>}
    </div>
  );
}
