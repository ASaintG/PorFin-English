"use client";

import { useCallback, useEffect, useRef, useSyncExternalStore } from "react";

const subscribe = () => () => {};

export function useSpeech() {
  const supported = useSyncExternalStore(
    subscribe,
    () => typeof window !== "undefined" && "speechSynthesis" in window,
    () => false,
  );
  const voiceRef = useRef<SpeechSynthesisVoice | null>(null);

  useEffect(() => {
    if (!("speechSynthesis" in window)) return;
    const loadVoices = () => {
      const voices = window.speechSynthesis.getVoices();
      voiceRef.current =
        voices.find((v) => v.lang.startsWith("en-US") && !v.localService) ??
        voices.find((v) => v.lang.startsWith("en")) ??
        null;
    };
    loadVoices();
    window.speechSynthesis?.addEventListener("voiceschanged", loadVoices);
    return () => window.speechSynthesis?.removeEventListener("voiceschanged", loadVoices);
  }, []);

  const speak = useCallback((text: string) => {
    if (!supported || !text) return;
    window.speechSynthesis.cancel();
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = "en-US";
    utterance.rate = 0.9;
    if (voiceRef.current) utterance.voice = voiceRef.current;
    window.speechSynthesis.speak(utterance);
  }, [supported]);

  const speakWithHighlight = useCallback((text: string, onWord?: (index: number) => void) => {
    if (!supported || !text) return;
    window.speechSynthesis.cancel();
    const words = text.split(/\s+/);
    let i = 0;
    const next = () => {
      if (i >= words.length) return;
      onWord?.(i);
      const utterance = new SpeechSynthesisUtterance(words[i]);
      utterance.lang = "en-US";
      utterance.rate = 0.85;
      if (voiceRef.current) utterance.voice = voiceRef.current;
      utterance.onend = () => { i++; next(); };
      window.speechSynthesis.speak(utterance);
    };
    next();
  }, [supported]);

  return { speak, speakWithHighlight, supported };
}
