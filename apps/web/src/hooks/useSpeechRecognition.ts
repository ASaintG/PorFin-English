"use client";

import { useCallback, useState, useSyncExternalStore } from "react";

type SpeechRecognitionInstance = {
  lang: string;
  interimResults: boolean;
  onresult: ((event: { results: ArrayLike<{ 0: { transcript: string } }> }) => void) | null;
  onend: (() => void) | null;
  start: () => void;
};

const subscribe = () => () => {};

export function useSpeechRecognition() {
  const supported = useSyncExternalStore(
    subscribe,
    () =>
      typeof window !== "undefined" &&
      ("SpeechRecognition" in window || "webkitSpeechRecognition" in window),
    () => false,
  );
  const [listening, setListening] = useState(false);
  const [transcript, setTranscript] = useState("");

  const start = useCallback(() => {
    if (!supported) return;
    const win = window as unknown as {
      SpeechRecognition?: new () => SpeechRecognitionInstance;
      webkitSpeechRecognition?: new () => SpeechRecognitionInstance;
    };
    const SR = win.SpeechRecognition ?? win.webkitSpeechRecognition;
    if (!SR) return;
    const recognition = new SR();
    recognition.lang = "en-US";
    recognition.interimResults = true;
    recognition.onresult = (event: { results: ArrayLike<{ 0: { transcript: string } }> }) => {
      let text = "";
      for (let i = 0; i < event.results.length; i++) text += event.results[i][0].transcript;
      setTranscript(text);
    };
    recognition.onend = () => setListening(false);
    recognition.start();
    setListening(true);
    setTranscript("");
  }, [supported]);

  return { supported, listening, transcript, start };
}
