"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { UserMenu } from "./UserMenu";

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

  return (
    <header className="border-b border-border bg-card">
      <div className="mx-auto flex w-full max-w-[1400px] flex-wrap items-center justify-between gap-x-6 gap-y-3 px-6 py-4 lg:px-8 xl:flex-nowrap">
        <Link href="/dashboard" className="shrink-0 text-lg font-bold tracking-tight text-foreground">
          InglesApp
        </Link>

        <nav className="order-3 flex w-full items-center gap-4 overflow-x-auto pb-1 xl:order-none xl:w-auto xl:flex-1 xl:justify-center xl:gap-6 xl:overflow-visible xl:pb-0">
          {links.map((link) => (
            <Link
              key={link.href}
              href={link.href}
              className={`whitespace-nowrap text-sm transition-colors ${
                pathname.startsWith(link.href)
                  ? "font-medium text-primary"
                  : "text-muted-foreground hover:text-foreground"
              }`}
            >
              {link.label}
            </Link>
          ))}
        </nav>

        <UserMenu />
      </div>
    </header>
  );
}
