# 🏁 Motocross Tracking Platform MVP

Una plataforma de tracking en tiempo real para motorsports (motocross, karting) con soporte para múltiples proveedores de rastreo GPS.

## 🔥 Deployment en Vivo

| Servicio | URL | Estado |
|----------|-----|--------|
| **Frontend** | https://motocross-10576.web.app | 🚀 Auto-deploy desde GitHub |
| **Backend** | Cloud Run (dinámico) | 🚀 Auto-deploy desde GitHub |
| **Database** | Cloud SQL PostgreSQL 15 | ✅ Managed |

**GitHub:** https://github.com/ClaudioVilas/motocross

---

## 🏗️ Arquitectura

### Infraestructura (Firebase + Google Cloud)

```
┌─────────────────────────────────────────────┐
│  Firebase Hosting (Frontend - React)        │
│  https://motocross-10576.web.app           │
└─────────────────┬───────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────┐
│  Cloud Run (Backend - .NET 8 API)          │
│  Docker Container - Auto-scale             │
└─────────────────┬───────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────┐
│  Cloud SQL (PostgreSQL 15)                 │
│  Database - Managed + Backups              │
└─────────────────────────────────────────────┘

          Cloud Build (CI/CD)
          Auto-deploy on push to main
```

### Monorepo Structure

```
motocross/
├── backend/                  # .NET 8 Web API
│   ├── src/
│   │   ├── Domain/          # Entities, Value Objects, Interfaces
│   │   ├── Application/     # Services, CQRS Commands/Queries
│   │   ├── Infrastructure/  # EF Core, Repositories, External Services
│   │   └── Api/            # Controllers, SignalR Hubs, Startup
│   └── Dockerfile          # Multi-stage Docker build
├── frontend/               # React 18 + TypeScript
│   ├── src/
│   │   ├── components/    # UI Components
│   │   ├── services/      # API + SignalR clients
│   │   ├── stores/        # Zustand state
│   │   └── pages/         # Routes
│   └── dist/             # Build output → Firebase Hosting
├── docker-compose.yml    # Local development
├── cloudbuild.yaml       # CI/CD configuration
├── firebase.json         # Firebase Hosting config
└── FIREBASE_SETUP.sh     # Automated deployment script
```

---

## 🚀 Tech Stack

### Backend (.NET 8)
- **Framework:** ASP.NET Core Web API
- **Architecture:** Clean Architecture (4 layers)
- **Database:** Entity Framework Core + PostgreSQL
- **Real-time:** SignalR
- **Patterns:** CQRS-lite, Repository, DI
- **Packages:** MediatR, FluentValidation, Identity
- **Container:** Docker multi-stage build

### Frontend (React)
- **Framework:** React 18 + TypeScript
- **Build:** Vite 8
- **Styling:** TailwindCSS v4
- **State:** Zustand (global) + React Query (server)
- **Routing:** React Router DOM
- **Maps:** Mapbox GL
- **Real-time:** @microsoft/signalr
- **PWA:** vite-plugin-pwa

### Infrastructure
- **Backend:** Google Cloud Run (Docker containers)
- **Database:** Google Cloud SQL (PostgreSQL 15)
- **Frontend:** Firebase Hosting (CDN global)
- **CI/CD:** Cloud Build (auto-deploy desde GitHub)
- **Registry:** Google Container Registry

---

## 📦 Instalación y Deployment

### Opción 1: Setup Automático (Recomendado)

```bash
# 1. Instalar herramientas necesarias
./INSTALL_TOOLS.sh

# 2. Autenticar
gcloud auth login
firebase login

# 3. Desplegar todo automáticamente
./FIREBASE_SETUP.sh
```

El script `FIREBASE_SETUP.sh` hará:
- ✅ Configurar Google Cloud project
- ✅ Habilitar APIs necesarias
- ✅ Crear Cloud SQL (PostgreSQL)
- ✅ Construir imagen Docker
- ✅ Desplegar backend a Cloud Run
- ✅ Configurar CI/CD desde GitHub
- ✅ Mostrar URLs finales

### Opción 2: Desarrollo Local

```bash
# Iniciar backend + PostgreSQL con Docker
docker-compose up -d

# Backend: http://localhost:8080
# Database: localhost:5432 (motocross/motocross_user/motocross_password)

# Ver logs
docker-compose logs -f backend

# Detener
docker-compose down
```

### Opción 3: Setup Manual

Ver [FIREBASE_DEPLOYMENT.md](FIREBASE_DEPLOYMENT.md) para instrucciones detalladas paso a paso.

---

## 🛠️ Desarrollo Local sin Docker

### Backend

```bash
cd backend

# Restaurar dependencias
dotnet restore

# Configurar connection string en appsettings.Development.json
# ConnectionStrings__DefaultConnection: "Host=localhost;Database=motocross;Username=postgres;Password=yourpassword"

# Ejecutar
dotnet run --project src/Api/Motocross.Api.csproj
```

Backend: http://localhost:5000

### Frontend

```bash
cd frontend

# Instalar dependencias
npm install

# Configurar variables de entorno
cp .env.example .env.development

# Ejecutar
npm run dev
```

Frontend: http://localhost:5173

---

## 🔍 API Endpoints

### Health Check
```bash
curl http://localhost:8080/health
# {"status":"healthy","timestamp":"2026-05-18T..."}
```

### Sessions
```bash
# Crear sesión
POST /api/sessions
{
  "name": "Práctica Matutina",
  "startFinishLine": {
    "start": { "latitude": -34.6037, "longitude": -58.3816 },
    "finish": { "latitude": -34.6037, "longitude": -58.3816 }
  }
}

# Listar sesiones
GET /api/sessions

# Obtener sesión
GET /api/sessions/{id}
```

### SignalR Hub
```javascript
const connection = new HubConnectionBuilder()
  .withUrl("http://localhost:8080/hubs/tracking")
  .build();

await connection.start();

// Enviar ubicación
await connection.invoke("SendLocation", sessionId, {
  latitude: -34.6037,
  longitude: -58.3816,
  speed: 45.5
});

// Escuchar actualizaciones
connection.on("ReceiveLocation", (update) => {
  console.log("Nueva ubicación:", update);
});
```

Ver [BACKEND_DEPLOYED.md](BACKEND_DEPLOYED.md) para documentación completa de la API.

---

## 📚 Documentación

| Documento | Descripción |
|-----------|-------------|
| [INSTALLATION_GUIDE.md](INSTALLATION_GUIDE.md) | Guía de instalación de Google Cloud SDK + Firebase CLI |
| [FIREBASE_DEPLOYMENT.md](FIREBASE_DEPLOYMENT.md) | Guía completa de deployment paso a paso |
| [BACKEND_DEPLOYED.md](BACKEND_DEPLOYED.md) | Documentación de API endpoints con ejemplos |
| [SETUP_COMPLETE.md](SETUP_COMPLETE.md) | Resumen del proyecto completo |

---

## 🎯 Features

### Tracking en Tiempo Real
- ✅ Múltiples proveedores de GPS (móvil, BLE, GPS externo)
- ✅ Actualización de posición en tiempo real via SignalR
- ✅ Visualización en mapa (Mapbox)
- ✅ Detección automática de vueltas

### Gestión de Sesiones
- ✅ Crear/pausar/reanudar/completar sesiones
- ✅ Configurar línea de inicio/fin
- ✅ Historial de sesiones
- ✅ Exportar datos

### Arquitectura
- ✅ Clean Architecture (4 capas)
- ✅ Domain-Driven Design
- ✅ CQRS-lite pattern
- ✅ Repository pattern
- ✅ Dependency Injection
- ✅ Value Objects

---

## 💰 Costos Estimados

| Servicio | Tier Gratuito | Costo Estimado |
|----------|---------------|----------------|
| **Cloud Run** | 2M requests/mes | **GRATIS** |
| **Cloud SQL** | - | **~$7/mes** (db-f1-micro) |
| **Firebase Hosting** | 10GB storage + 360MB/día | **GRATIS** |
| **Container Registry** | 0.5GB storage | **GRATIS** |
| **Cloud Build** | 120 builds/día | **GRATIS** |

**Total estimado:** ~$7-10/mes

---

## 🔄 CI/CD Automático

Cada `git push` a `main` despliega automáticamente:

1. **Cloud Build** detecta el push
2. Construye imagen Docker del backend
3. Sube imagen a Container Registry
4. Despliega a Cloud Run
5. ✅ Backend actualizado en ~5 minutos

Para el frontend:
```bash
cd frontend
npm run build
firebase deploy --only hosting
```

---

## 🐛 Troubleshooting

### Backend no inicia en Cloud Run

```bash
# Ver logs
gcloud run services logs read motocross-api --region=us-central1 --limit=50

# Verificar variables de entorno
gcloud run services describe motocross-api --region=us-central1 --format=yaml
```

### Error de conexión a Cloud SQL

```bash
# Verificar conexión
gcloud sql instances describe motocross-db

# Conectar manualmente
gcloud sql connect motocross-db --user=motocross_user --database=motocross
```

### Frontend no conecta al backend

1. Verificar `.env.production` tiene la URL correcta de Cloud Run
2. Verificar CORS en backend (`Program.cs`)
3. Rebuild y redeploy frontend

Ver [FIREBASE_DEPLOYMENT.md](FIREBASE_DEPLOYMENT.md) sección "Troubleshooting" para más detalles.

---

## 👨‍💻 Desarrollo

### Agregar Nuevo Endpoint

1. Crear DTO en `Application/DTOs/`
2. Crear Command/Query en `Application/Commands/` o `Queries/`
3. Crear Handler con MediatR
4. Agregar Controller en `Api/Controllers/`
5. Actualizar documentación

### Agregar Nueva Entidad

1. Crear entidad en `Domain/Entities/`
2. Configurar en `Infrastructure/Persistence/MotocrossDbContext.cs`
3. Crear migración: `dotnet ef migrations add NombreMigracion`
4. Aplicar migración (automático en startup o manual con `dotnet ef database update`)

---

## 📞 Contacto y Soporte

- **GitHub Issues:** https://github.com/ClaudioVilas/motocross/issues
- **Documentación Firebase:** https://firebase.google.com/docs
- **Documentación Cloud Run:** https://cloud.google.com/run/docs
- **Documentación .NET:** https://learn.microsoft.com/en-us/aspnet/core

---

## 📝 Licencia

Este proyecto es un MVP privado. Todos los derechos reservados.

---

**🎯 Estado del Proyecto:**
- ✅ Backend completo y funcional (.NET 8 + Clean Architecture)
- ✅ Frontend completo (React + TypeScript + PWA)
- ✅ Infrastructure as Code (Docker + Cloud Build)
- ✅ CI/CD automático configurado
- 🚀 Listo para deployment con `./FIREBASE_SETUP.sh`
