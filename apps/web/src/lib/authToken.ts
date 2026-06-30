let memoryToken: string | null = null;

export function setAuthToken(token: string | null) {
  memoryToken = token;
  if (token) sessionStorage.setItem("inglesapp-token", token);
  else sessionStorage.removeItem("inglesapp-token");
}

export function getAuthToken(): string | null {
  if (memoryToken) return memoryToken;
  if (typeof window !== "undefined") {
    memoryToken = sessionStorage.getItem("inglesapp-token");
  }
  return memoryToken;
}
