# 🏁 Motocross Tracking Platform

A production-ready motorsports tracking platform for real-time position monitoring, lap timing, and telemetry visualization.

## 🎯 Overview

This platform provides real-time tracking for motorsports events (motocross, karting) with support for multiple tracking providers including mobile GPS, BLE tags, and external GPS devices.

## 🏗️ Architecture

**Monorepo Structure:**
- `frontend/` - React + TypeScript + Vite PWA
- `backend/` - ASP.NET Core .NET 8 Web API with Clean Architecture

**Key Principles:**
- Clean Architecture & Domain-Driven Design
- CQRS-lite pattern
- Provider abstraction (ITrackingProvider)
- Real-time communication via SignalR
- Mobile-first, PWA-ready

## 🚀 Tech Stack

### Frontend
- **Framework:** React 18 + TypeScript
- **Build Tool:** Vite
- **Styling:** TailwindCSS
- **State Management:** Zustand
- **Data Fetching:** React Query (TanStack Query)
- **Routing:** React Router
- **Maps:** Mapbox GL
- **Real-time:** SignalR Client
- **PWA:** Vite PWA Plugin

### Backend
- **Framework:** ASP.NET Core .NET 8
- **Architecture:** Clean Architecture (4 layers)
- **Database:** PostgreSQL + Entity Framework Core
- **Real-time:** SignalR
- **Patterns:** Repository, CQRS-lite, Dependency Injection

## 📁 Project Structure

```
motocross-tracking/
├── frontend/                    # React application
│   ├── src/
│   │   ├── components/         # Reusable UI components
│   │   ├── features/           # Feature-based modules
│   │   │   ├── tracking/      # Tracking feature
│   │   │   ├── sessions/      # Session management
│   │   │   └── telemetry/     # Telemetry displays
│   │   ├── hooks/             # Custom React hooks
│   │   ├── services/          # API and SignalR services
│   │   ├── stores/            # Zustand stores
│   │   ├── providers/         # Context providers
│   │   ├── pages/             # Route pages
│   │   ├── shared/            # Shared utilities
│   │   └── types/             # TypeScript types
│   ├── public/
│   └── package.json
│
├── backend/                     # .NET 8 solution
│   ├── src/
│   │   ├── Api/               # ASP.NET Core Web API
│   │   ├── Application/       # Use cases, DTOs, interfaces
│   │   ├── Domain/            # Entities, aggregates, domain services
│   │   └── Infrastructure/    # EF Core, SignalR, external services
│   └── Motocross.sln
│
├── .github/
│   └── workflows/             # CI/CD pipelines
├── docs/                       # Documentation
└── README.md
```

## 🔧 Setup Instructions

### Prerequisites
- Node.js 20+
- .NET 8 SDK
- PostgreSQL 15+
- Git

### Local Development

#### Backend Setup
```bash
cd backend
dotnet restore
dotnet build
dotnet ef database update --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api
```

#### Frontend Setup
```bash
cd frontend
npm install
npm run dev
```

### Environment Variables

#### Frontend (.env.local)
```env
VITE_API_BASE_URL=http://localhost:5000
VITE_SIGNALR_HUB_URL=http://localhost:5000/hubs/tracking
VITE_MAPBOX_TOKEN=your_mapbox_token
```

#### Backend (appsettings.Development.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=motocross;Username=postgres;Password=your_password"
  }
}
```

## 🌐 Deployment

### Frontend (Vercel)
- Automatic deployment on push to `main`
- Environment variables configured via Vercel dashboard
- Production URL: https://motocross-tracking.vercel.app

### Backend (Render)
- Automatic deployment on push to `main`
- PostgreSQL database managed by Render
- Production URL: https://motocross-api.onrender.com

## 📡 Key Features

### MVP Features
- ✅ Real-time GPS position tracking
- ✅ Live route visualization on map
- ✅ Session recording (start/stop)
- ✅ Lap timing and detection
- ✅ Start/finish line configuration
- ✅ Live telemetry updates (speed, location)
- ✅ Mobile-first responsive UI
- ✅ PWA support for iPhone Safari
- ✅ Dark modern UI theme

### Tracking Provider Support
- 📱 Mobile phone GPS (via browser Geolocation API)
- 🔵 BLE tags (abstraction ready)
- 🛰️ External GPS devices (abstraction ready)
- 📡 Future LTE/GNSS devices

## 🏛️ Architecture Deep Dive

### Backend Layers

1. **Domain Layer**
   - Core business entities (Session, TrackingPoint, Lap)
   - Domain services (LapDetectionService)
   - Value objects (Coordinate, Speed)
   - Provider abstractions (ITrackingProvider, IPositionSource)

2. **Application Layer**
   - Use cases/Commands/Queries (CQRS-lite)
   - DTOs (SessionDto, TrackingPointDto)
   - Service interfaces (ISessionService, ITrackingService)

3. **Infrastructure Layer**
   - EF Core implementation
   - Repository implementations
   - SignalR hubs
   - External service integrations

4. **API Layer**
   - Controllers (minimal logic)
   - Middleware
   - SignalR hub registration
   - Dependency injection configuration

### Frontend Architecture

- **Feature-based organization:** Each feature is self-contained
- **Separation of concerns:** Components, hooks, services separated
- **Provider abstraction:** TrackingService abstracts GPS sources
- **State management:** Zustand for global state, React Query for server state
- **Real-time:** SignalR connection managed via custom hook

## 🔐 Security

- CORS configured for production domains
- Environment variables for sensitive data
- API authentication ready (extend with JWT)
- HTTPS enforced in production

## 📈 Scalability Considerations

- Provider abstraction supports multiple GPS sources
- SignalR scales with Redis backplane (configurable)
- Database indexes on frequently queried fields
- Background services for heavy processing
- Offline buffering foundation in frontend

## 🧪 Testing Strategy

- Backend: Unit tests for domain logic, integration tests for API
- Frontend: Component tests with Vitest, E2E with Playwright
- CI/CD runs tests before deployment

## 🛣️ Roadmap

### Phase 1 (MVP) ✅
- Basic tracking with mobile GPS
- Session management
- Real-time updates
- Map visualization

### Phase 2
- BLE tag integration
- Advanced lap analysis
- Multiple simultaneous riders
- Historical session playback

### Phase 3
- Native mobile app (Capacitor)
- Offline mode
- Advanced telemetry (accelerometer, gyro)
- Social features

### Phase 4
- Live event streaming
- Spectator mode
- Race management tools
- Analytics dashboard

## � Repository

**GitHub:** https://github.com/ClaudioVilas/motocross

## �📚 Documentation

- [Architecture Guide](docs/ARCHITECTURE.md)
- [API Documentation](docs/API.md)
- [Deployment Guide](docs/DEPLOYMENT.md)
- [Contributing Guidelines](docs/CONTRIBUTING.md)

## 🤝 Contributing

This is an MVP project. Contributions welcome once core features are stable.

## 📄 License

MIT License - See LICENSE file for details

## 👨‍💻 Development Workflow

1. Create feature branch from `main`
2. Implement changes following Clean Architecture principles
3. Test locally (frontend + backend)
4. Push and create PR
5. CI/CD runs tests
6. Merge triggers automatic deployment

## 🆘 Support

For issues or questions, please open a GitHub issue.

---

Built with ❤️ for the motorsports community
