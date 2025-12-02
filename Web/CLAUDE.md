# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

React 18 + TypeScript web application for managing customers, institutions, addresses, and documents. Built with Vite, TailwindCSS, shadcn/ui, React Router v6, TanStack Query, Zustand, and React Hook Form + Zod.

## Environment Setup

Required environment variables in `.env`:
```
VITE_DATA_API_URL=<data-api-endpoint>
VITE_AUTH_API_URL=<auth-api-endpoint>
```

## Development Commands

```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run linter (if configured)
npm run lint
```

## Architecture

### Authentication Flow
- Token-based authentication stored in `localStorage`
- Bearer token automatically injected into all API requests via fetch wrapper
- AuthGuard component protects routes requiring authentication
- Two permission levels:
  - Admin: `data:read,data:remove,data:write` (full CRUD)
  - User: `data:read` (read-only)

### Data Model Relationships
```
Institution (1) ──> (*) Customer (1) ──> (*) Address
                           └──> (*) Document
```

- Institutions have multiple customers
- Customers belong to one institution and have multiple addresses and documents
- API supports `includeCustomers=true` query parameter for nested data retrieval

### API Structure

**Auth Endpoints** (`VITE_AUTH_API_URL`):
- `POST /login` - Returns `{id, token}` or `{id, email, company, token}`
- `POST /register` - Creates user with `{email, company, password}`
- `GET /me` - Returns token info including permissions

**Data Endpoints** (`VITE_DATA_API_URL`):
- `GET /institutions` - List all (supports `?includeCustomers=true`)
- `POST /institutions` - Create (requires `data:write`)
- `GET /institutions/:id` - Get single (supports `?includeCustomers=true`)
- `PUT /institutions/:id` - Full update (requires `data:write`)
- `PATCH /institutions/:id` - Partial update (requires `data:write`)
- `DELETE /institutions/:id` - Delete (requires `data:remove`)
- Similar CRUD patterns for `/customers`, `/addresses`, `/documents`

### Project Structure

```
src/
  components/
    ui/              # shadcn/ui components (Button, Input, Modal, Table, Toast)
    layout/          # Sidebar, Navbar, Layout wrapper
  features/
    auth/            # Login page, AuthGuard, auth hooks, auth API
    institutions/    # CRUD pages, API hooks, types
    customers/       # CRUD pages, API hooks, types
    addresses/       # CRUD pages, API hooks, types
    documents/       # CRUD pages, API hooks, types
  lib/
    fetcher.ts       # Base fetch wrapper with Bearer token injection
                     # Zod schemas for validation
  providers/         # QueryProvider, RouterProvider, ThemeProvider
  router.tsx         # Route definitions with protected routes
  main.tsx           # App entry point
```

### Feature Module Pattern

Each feature module (`institutions`, `customers`, `addresses`, `documents`) follows:
- `api.ts` - API functions using fetch wrapper
- `hooks.ts` - TanStack Query hooks (useQuery, useMutation)
- `types.ts` - TypeScript interfaces for entities
- `pages/` - List, Detail, Create, Edit pages
  - List: TanStack Table with sorting, pagination, row selection
  - Detail: Read-only view with full entity information
  - Create/Edit: React Hook Form + Zod validation
  - Delete: Confirmation modal before API call

### State Management
- **TanStack Query**: Server state, API caching, mutations
- **Zustand**: UI state, global client state
- **localStorage**: Auth token persistence

### Key Implementation Details
- Use inline `fetch` for API calls (no axios)
- All API calls go through base fetcher that injects Bearer token from localStorage
- Toast notifications for success/error feedback on mutations
- Forms use React Hook Form with Zod schemas for validation
- Protected routes wrapped in AuthGuard check token existence
- Responsive design with TailwindCSS utilities

## Docker Deployment

Project includes Dockerfile for static hosting. After building:
```bash
docker build -t customers-ui .
docker run -p 8080:80 customers-ui
```
