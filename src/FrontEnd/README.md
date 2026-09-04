# Customer Support CRM — Frontend

React + TypeScript (Vite) frontend for the Customer Support CRM. Consumes the
ASP.NET Core backend in `src/BackEnd`.

See [`docs/frontend-architecture.md`](docs/frontend-architecture.md) for the
full architecture (project structure, API/auth/localization/theme
infrastructure, security boundaries).

## Getting started

```bash
npm install
cp .env.example .env.local   # then adjust VITE_API_BASE_URL if needed
npm run dev
```

## Scripts

| Command                                   | Purpose                                              |
| ----------------------------------------- | ---------------------------------------------------- |
| `npm run dev`                             | Start the Vite dev server                            |
| `npm run build`                           | Type-check (`tsc -b`) and produce a production build |
| `npm run preview`                         | Serve the production build locally                   |
| `npm run lint`                            | ESLint                                               |
| `npm run format` / `npm run format:check` | Prettier                                             |
