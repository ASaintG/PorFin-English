# Changelog

## [0.1.0] - 2026-06-30

### Added

- Monorepo InglesApp: Next.js 16 + ASP.NET Core 10 + MongoDB
- Vocabulario A1–B2 con repetición espaciada (SM-2)
- Gramática, lecciones temáticas (20 unidades), desbloqueo progresivo
- Reading y listening con TTS y comprensión
- Speaking (Web Speech STT) y writing con track job interview B1
- Placement test, learning path, sesión diaria, simulador de examen
- Cronómetro de examen (30 min) con auto-envío
- PWA mínima (manifest + service worker)
- Offline: IndexedDB + `POST /api/reviews/batch`
- Cookie JWT httpOnly + proxy Next.js 16
- UI de feedback (`/settings/feedback`)
- Tests xUnit: SM-2, auth, batch reviews, lesson unlock
- CI GitHub Actions (build + test + lint)
- Script `pnpm db:reset`
- Licencia MIT

### Security

- Secrets movidos a `appsettings.Development.json` / variables de entorno
- Usuario demo documentado solo para desarrollo

### Known issues

- Paquetes transitivos `SharpCompress` / `Snappier` (vía MongoDB.Driver) con advisories NuGet pendientes de resolución upstream
