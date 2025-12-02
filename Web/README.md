# Customers UI

A complete React + TypeScript web application for managing customers, institutions, addresses, and documents.

## Tech Stack

- **Frontend Framework**: React 18 + TypeScript
- **Build Tool**: Vite
- **Styling**: TailwindCSS + shadcn/ui
- **Routing**: React Router v6
- **State Management**:
  - TanStack Query (React Query) for server state
  - Zustand for UI/global state
- **Forms**: React Hook Form + Zod validation
- **HTTP Client**: Native fetch API
- **Deployment**: Docker + nginx

## Features

### Authentication
- Token-based authentication with localStorage
- Login page with form validation
- AuthGuard for protecting routes
- Automatic Bearer token injection in API requests
- Logout functionality

### CRUD Modules

1. **Institutions**
   - List, Create, Read, Update, Delete operations
   - Table with sorting and pagination
   - Can include related customers

2. **Customers**
   - Full CRUD with institution relationship
   - Linked to institutions via institutionId

3. **Addresses**
   - Full CRUD with customer relationship
   - Fields: country, city, street, isCurrent flag

4. **Documents**
   - Full CRUD with customer relationship
   - Fields: title, content, isActive flag

### UI Features
- Responsive design
- TanStack Table with sorting, pagination, row selection
- Form validation with error messages
- Toast notifications for success/error feedback
- Confirmation modals for delete operations
- Sidebar navigation
- Clean, modern UI with shadcn/ui components

## Getting Started

### Prerequisites

- Node.js 20+
- npm

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd Web
```

2. Install dependencies:
```bash
npm install
```

3. Configure environment variables:

Copy `.env.example` to `.env` and update the API URLs:

```bash
VITE_DATA_API_URL=http://localhost:20011
VITE_AUTH_API_URL=http://localhost:20031
```

### Development

Run the development server:

```bash
npm run dev
```

The application will be available at `http://localhost:20043`

### Build

Build for production:

```bash
npm run build
```

Preview the production build:

```bash
npm run preview
```

### Docker Deployment

Build the Docker image:

```bash
docker build -t customers-ui .
```

Run the container:

```bash
docker run -p 20043:80 customers-ui
```

The application will be available at `http://localhost:20043`

## Project Structure

```
src/
├── components/
│   ├── ui/              # shadcn/ui reusable components
│   └── layout/          # Sidebar, Navbar, Layout
├── features/
│   ├── auth/            # Authentication module
│   ├── institutions/    # Institutions CRUD
│   ├── customers/       # Customers CRUD
│   ├── addresses/       # Addresses CRUD
│   └── documents/       # Documents CRUD
├── lib/
│   ├── fetcher.ts       # Base fetch wrapper with token injection
│   ├── utils.ts         # Utility functions
│   └── env.ts           # Environment configuration
├── providers/           # React Query and other providers
├── App.tsx              # Main App component
├── router.tsx           # Route definitions
└── main.tsx             # Application entry point
```

## API Structure

### Authentication Endpoints
- `POST /login` - Login with email and password
- `POST /register` - Register new user
- `GET /me` - Get current user info

### Data Endpoints
All data endpoints require Bearer token authentication:

- **Institutions**: `/institutions`
- **Customers**: `/customers`
- **Addresses**: `/addresses`
- **Documents**: `/documents`

Each endpoint supports standard CRUD operations:
- `GET /` - List all
- `GET /:id` - Get by ID
- `POST /` - Create
- `PUT /:id` - Update
- `PATCH /:id` - Partial update
- `DELETE /:id` - Delete

## Development Notes

- Authentication token is stored in `localStorage` under the key `auth_token`
- All API requests automatically inject the Bearer token via the fetcher wrapper
- Forms use Zod schemas for validation
- TanStack Query handles API caching and invalidation
- The application uses React Router for client-side routing
- All routes except `/login` are protected by AuthGuard

## License

MIT
