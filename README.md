# InglesApp

App web para aprender inglés de A1 a B2 con repetición espaciada (SM-2). Vocabulario, gramática, lectura, listening, speaking y writing orientados a contexto real y empleo.

**Licencia:** MIT

## Stack

- **Frontend:** Next.js 16 (App Router), React 19, Tailwind CSS
- **Backend:** ASP.NET Core 10, Clean Architecture
- **Base de datos:** MongoDB
- **Monorepo:** pnpm + Turborepo

## Requisitos

- Node.js 20+
- pnpm 9+
- .NET 10 SDK
- Docker (MongoDB)
- Python 3 (opcional, regenerar vocabulario B1/B2)

## Inicio rápido

```bash
pnpm install
pnpm db:up
pnpm dev
```

- Web: http://localhost:3000
- API: http://localhost:5000

Si el puerto 5000 está ocupado: `pnpm ports:free` o `pnpm dev:clean`.

### Reset de base de datos

```bash
pnpm db:reset
# Reinicia la API para volver a ejecutar el seed automático
```

## Usuario demo (solo desarrollo)

| Email | Contraseña |
|-------|------------|
| `demo@inglesapp.dev` | `demo123` |

No uses estas credenciales en producción.

## Variables de entorno

Copia `.env.example` y exporta las variables, o usa `appsettings.Development.json` en la API:

| Variable | Descripción |
|----------|-------------|
| `MongoDb__ConnectionString` | URI de MongoDB |
| `MongoDb__DatabaseName` | Nombre de la BD (`inglesapp`) |
| `Jwt__Secret` | Clave JWT (mín. 32 caracteres) |
| `NEXT_PUBLIC_API_URL` | URL de la API (`http://localhost:5000`) |

## Funcionalidades v0.1

- Vocabulario A1–B2 (~1000 A1, ~300 A2, ~700 B1, ~700 B2, más packs temáticos)
- Gramática A1–B1 con 45 temas, explicaciones ricas y 5 tipos de ejercicios (fill-in-blank, multiple choice, word order y corrección de errores)
- Lecciones temáticas con desbloqueo progresivo (20 unidades)
- Lectura por niveles, listening con TTS y preguntas de comprensión
- Speaking (STT) y writing por niveles, con prompts de producción y evaluación por IA
- Placement test, learning path, sesión diaria con filtros por nivel/tema, simulador de examen con cronómetro
- PWA básica y offline (cola de repasos + batch sync)
- Cookie JWT `httpOnly` para rutas protegidas

## Rutas principales

| Ruta | Descripción |
|------|-------------|
| `/dashboard` | Progreso por habilidad |
| `/review` | Sesión diaria vocab + gramática |
| `/lessons`, `/path` | Lecciones y ruta CEFR |
| `/reading`, `/listening` | Comprensión |
| `/speaking`, `/writing` | Producción oral/escrita |
| `/exam` | Simulador de examen |
| `/onboarding/placement` | Test de nivel |
| `/settings/feedback` | Reportar errores / sugerencias |

## Tests

```bash
cd apps/api && dotnet test
```

## Regenerar vocabulario B1/B2

```bash
python3 scripts/generate_b1_b2_vocab.py
```

## Estructura

```
apps/
  web/          # Next.js
  api/          # .NET (Domain, Application, Infrastructure, Api, Tests)
scripts/        # Generadores de seed
```

## Seguridad

- JWT en cookie `httpOnly` para protección de rutas Next.js
- Bearer token en `sessionStorage` para llamadas API (sesión de pestaña)
- Cambia `Jwt__Secret` en producción
- El seed de demo solo aplica en Development

## Limitaciones conocidas

- PWA offline básica (no full offline-first)
- Subniveles CEFR (`A1.1`) no filtran contenido fino aún
- Sin corrección IA en speaking/writing
- Vulnerabilidades NuGet transitivas en MongoDB.Driver (documentadas en CHANGELOG)
