import { create } from "zustand";
import { persist } from "zustand/middleware";
import api, { type AuthResponse } from "@/api/client";
import { getAuthToken, setAuthToken } from "@/lib/authToken";

interface AuthState {
  token: string | null;
  userId: string | null;
  email: string | null;
  currentLevel: string | null;
  currentSubLevel: string | null;
  placementCompleted: boolean;
  login: (email: string, password: string) => Promise<AuthResponse>;
  register: (email: string, password: string) => Promise<AuthResponse>;
  logout: () => Promise<void>;
  setAuth: (data: AuthResponse) => Promise<void>;
}

async function setSessionCookie(token: string) {
  await fetch("/api/auth/session", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ token }),
  });
}

async function clearSessionCookie() {
  await fetch("/api/auth/session", { method: "DELETE" });
}

async function applyAuth(data: AuthResponse, set: (s: Partial<AuthState>) => void) {
  setAuthToken(data.token);
  await setSessionCookie(data.token);
  set({
    token: data.token,
    userId: data.userId,
    email: data.email,
    currentLevel: data.currentLevel,
    currentSubLevel: data.currentSubLevel,
    placementCompleted: data.placementCompleted,
  });
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      userId: null,
      email: null,
      currentLevel: null,
      currentSubLevel: null,
      placementCompleted: false,
      setAuth: async (data) => applyAuth(data, set),
      login: async (email, password) => {
        const { data } = await api.post<AuthResponse>("/auth/login", { email, password });
        await applyAuth(data, set);
        return data;
      },
      register: async (email, password) => {
        const { data } = await api.post<AuthResponse>("/auth/register", { email, password });
        await applyAuth(data, set);
        return data;
      },
      logout: async () => {
        setAuthToken(null);
        await clearSessionCookie();
        set({
          token: null,
          userId: null,
          email: null,
          currentLevel: null,
          currentSubLevel: null,
          placementCompleted: false,
        });
      },
    }),
    {
      name: "inglesapp-auth",
      partialize: (state) => ({
        userId: state.userId,
        email: state.email,
        currentLevel: state.currentLevel,
        currentSubLevel: state.currentSubLevel,
        placementCompleted: state.placementCompleted,
      }),
      onRehydrateStorage: () => () => {
        const token = getAuthToken();
        if (token) useAuthStore.setState({ token });
      },
    },
  ),
);
