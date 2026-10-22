import type { Metadata, Viewport } from "next";
import { Geist_Mono, Noto_Sans } from "next/font/google";
import { ServiceWorkerRegister } from "@/components/ServiceWorkerRegister";
import "./globals.css";

const notoSans = Noto_Sans({
  variable: "--font-noto-sans",
  subsets: ["latin"],
  weight: ["400", "500", "700"],
  display: "swap",
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "InglesApp — Aprende inglés en serio",
  description: "Vocabulario, gramática, lectura y speaking A1–B2 con repetición espaciada.",
  manifest: "/manifest.json",
  appleWebApp: { capable: true, title: "InglesApp" },
};

export const viewport: Viewport = {
  themeColor: "#4f46e5",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="es"
      className={`${notoSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="flex min-h-full flex-col bg-background font-sans text-foreground">
        <ServiceWorkerRegister />
        {children}
      </body>
    </html>
  );
}
