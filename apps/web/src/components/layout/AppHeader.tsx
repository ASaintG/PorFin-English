"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuthStore } from "@/stores/authStore";

const links = [
  { href: "/dashboard", label: "Dashboard" },
  { href: "/grammar", label: "Gramática" },
  { href: "/path", label: "Ruta" },
  { href: "/lessons", label: "Lecciones" },
  { href: "/review", label: "Repaso" },
  { href: "/reading", label: "Lectura" },
  { href: "/listening", label: "Listening" },
  { href: "/speaking", label: "Speaking" },
  { href: "/writing", label: "Writing" },
  { href: "/exam", label: "Examen" },
  { href: "/settings/feedback", label: "Feedback" },
];

export function AppHeader() {
  const pathname = usePathname();
  const { email, logout } = useAuthStore();

  return (
    <header className="border-b border-zinc-200 bg-white dark:border-zinc-800 dark:bg-zinc-950">
      <div className="mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-4 px-4 py-3">
        <Link href="/dashboard" className="text-lg font-semibold">InglesApp</Link>
        <nav className="flex flex-wrap gap-3">
          {links.map((link) => (
            <Link
              key={link.href}
              href={link.href}
              className={`text-sm ${pathname.startsWith(link.href) ? "text-emerald-700 font-medium" : "text-zinc-600"}`}
            >
              {link.label}
            </Link>
          ))}
        </nav>
        <div className="flex items-center gap-3">
          <span className="hidden text-xs text-zinc-500 sm:inline">{email}</span>
          <button type="button" onClick={() => void logout()} className="text-sm text-zinc-600">Salir</button>
        </div>
      </div>
    </header>
  );
}
