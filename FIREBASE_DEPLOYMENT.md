# 🔥 Despliegue en Firebase + Google Cloud

## Arquitectura

- **Backend:** Cloud Run (contenedor Docker con .NET 8)
- **Base de datos:** Cloud SQL (PostgreSQL 15)
- **Frontend:** Firebase Hosting
- **CI/CD:** Cloud Build (despliegue automático desde GitHub)

---

## 🚀 Despliegue Automático

### Requisitos Previos

```bash
# Instalar Google Cloud SDK
brew install google-cloud-sdk

# Instalar Firebase CLI
npm install -g firebase-tools

# Autenticarse
gcloud auth login
firebase login
```

---

## 📦 Opción 1: Script Automático (Recomendado)

```bash
# Dar permisos de ejecución
chmod +x FIREBASE_SETUP.sh

# Ejecutar setup completo
./FIREBASE_SETUP.sh
```

El script hará:
1. ✅ Configurar proyecto de Google Cloud
2. ✅ Habilitar APIs necesarias
3. ✅ Crear instancia de Cloud SQL (PostgreSQL)
4. ✅ Crear base de datos y usuario
5. ✅ Construir imagen Docker
6. ✅ Desplegar backend a Cloud Run
7. ✅ Configurar despliegue automático con Cloud Build

---

## 🔧 Opción 2: Paso a Paso Manual

### 1. Configurar Google Cloud

```bash
# Establecer proyecto
gcloud config set project motocross-10576

# Habilitar APIs
gcloud services enable \
  cloudbuild.googleapis.com \
  run.googleapis.com \
  sql-component.googleapis.com \
  sqladmin.googleapis.com \
  containerregistry.googleapis.com
```

### 2. Crear Cloud SQL (PostgreSQL)

```bash
# Crear instancia
gcloud sql instances create motocross-db \
  --database-version=POSTGRES_15 \
  --tier=db-f1-micro \
  --region=us-central1 \
  --root-password=$(openssl rand -base64 32)

# Crear base de datos
gcloud sql databases create motocross \
  --instance=motocross-db

# Crear usuario
gcloud sql users create motocross_user \
  --instance=motocross-db \
  --password=$(openssl rand -base64 24)
```

### 3. Construir y Desplegar Backend

```bash
# Construir imagen Docker
cd backend
docker build -t gcr.io/motocross-10576/motocross-api:latest .

# Autenticar Docker con GCR
gcloud auth configure-docker

# Subir imagen
docker push gcr.io/motocross-10576/motocross-api:latest

# Desplegar a Cloud Run
gcloud run deploy motocross-api \
  --image gcr.io/motocross-10576/motocross-api:latest \
  --region=us-central1 \
  --platform=managed \
  --allow-unauthenticated \
  --add-cloudsql-instances=motocross-10576:us-central1:motocross-db
```

### 4. Configurar Variables de Entorno

```bash
# Obtener connection string
CONNECTION_NAME=$(gcloud sql instances describe motocross-db --format='value(connectionName)')

# Actualizar servicio con variables
gcloud run services update motocross-api \
  --region=us-central1 \
  --set-env-vars="ConnectionStrings__DefaultConnection=Host=/cloudsql/$CONNECTION_NAME;Database=motocross;Username=motocross_user;Password=YOUR_PASSWORD"
```

### 5. Desplegar Frontend

```bash
cd frontend

# Actualizar .env.production con URL del backend
SERVICE_URL=$(gcloud run services describe motocross-api --region=us-central1 --format='value(status.url)')
echo "VITE_API_BASE_URL=$SERVICE_URL" > .env.production
echo "VITE_SIGNALR_HUB_URL=$SERVICE_URL/hubs/tracking" >> .env.production

# Build y deploy
npm run build
firebase deploy --only hosting
```

---

## 🔄 CI/CD Automático

### Configurar Cloud Build Trigger

```bash
# Conectar repositorio de GitHub
gcloud builds triggers create github \
  --repo-name=motocross \
  --repo-owner=ClaudioVilas \
  --branch-pattern="^main$" \
  --build-config=cloudbuild.yaml
```

Ahora cada `git push` a `main` desplegará automáticamente el backend.

---

## 🧪 Desarrollo Local con Docker

```bash
# Levantar backend + PostgreSQL
docker-compose up -d

# Ver logs
docker-compose logs -f backend

# Detener
docker-compose down
```

URLs locales:
- Backend: http://localhost:8080
- Database: localhost:5432

---

## 📊 Verificación

### Backend Health Check

```bash
# Cloud Run
curl https://motocross-api-XXXXXX-uc.a.run.app/health

# Local
curl http://localhost:8080/health
```

Respuesta esperada:
```json
{
  "status": "healthy",
  "timestamp": "2026-05-18T..."
}
```

### Frontend

```bash
# Firebase Hosting
open https://motocross-10576.web.app

# Local
cd frontend && npm run dev
```

---

## 💰 Costos Estimados (Tier Gratuito)

- **Cloud Run:** 2M requests/mes gratis
- **Cloud SQL:** db-f1-micro = ~$7/mes
- **Firebase Hosting:** 10GB almacenamiento + 360MB/día gratis
- **Container Registry:** 0.5GB gratis

**Total estimado:** ~$7-10/mes

---

## 🔐 Seguridad

### Secrets en Cloud Build

```bash
# Crear secret para DB password
echo -n "your-db-password" | gcloud secrets create db-password --data-file=-

# Dar acceso a Cloud Build
gcloud secrets add-iam-policy-binding db-password \
  --member=serviceAccount:PROJECT_NUMBER@cloudbuild.gserviceaccount.com \
  --role=roles/secretmanager.secretAccessor
```

### Actualizar cloudbuild.yaml para usar secrets:

```yaml
availableSecrets:
  secretManager:
  - versionName: projects/motocross-10576/secrets/db-password/versions/latest
    env: 'DB_PASSWORD'
```

---

## 🛠️ Comandos Útiles

```bash
# Ver logs del backend
gcloud run services logs read motocross-api --region=us-central1

# Ver logs de Cloud SQL
gcloud sql operations list --instance=motocross-db

# Conectar a la base de datos
gcloud sql connect motocross-db --user=motocross_user --database=motocross

# Actualizar imagen del servicio
gcloud run services update motocross-api \
  --image gcr.io/motocross-10576/motocross-api:latest \
  --region=us-central1

# Escalar servicio
gcloud run services update motocross-api \
  --min-instances=1 \
  --max-instances=10
```

---

## 🐛 Troubleshooting

### Error: "Cloud SQL connection failed"

```bash
# Verificar que Cloud SQL Proxy está habilitado
gcloud sql instances describe motocross-db --format='value(settings.ipConfiguration.requireSsl)'

# Reiniciar servicio
gcloud run services update motocross-api --region=us-central1
```

### Error: "Container failed to start"

```bash
# Ver logs detallados
gcloud run services logs read motocross-api --region=us-central1 --limit=50

# Verificar imagen
gcloud container images list --repository=gcr.io/motocross-10576
```

### Error: "Database migrations failed"

```bash
# Conectar manualmente y ejecutar migraciones
gcloud sql connect motocross-db --user=motocross_user --database=motocross

# O ejecutar desde Cloud Run (el backend las hace automáticamente en startup)
```

---

## 📚 Recursos

- [Cloud Run Documentation](https://cloud.google.com/run/docs)
- [Cloud SQL for PostgreSQL](https://cloud.google.com/sql/docs/postgres)
- [Firebase Hosting](https://firebase.google.com/docs/hosting)
- [Cloud Build](https://cloud.google.com/build/docs)

---

## ✅ Checklist Post-Deployment

- [ ] Backend responde en Cloud Run URL
- [ ] Health check devuelve `{"status":"healthy"}`
- [ ] Base de datos conectada correctamente
- [ ] Frontend desplegado en Firebase Hosting
- [ ] CORS configurado con URL del frontend
- [ ] Cloud Build trigger configurado
- [ ] Variables de entorno configuradas
- [ ] Logs funcionando correctamente
- [ ] SignalR conectando correctamente
- [ ] Autenticación de usuarios funcionando

---

**🎯 Una vez completado el setup, el sistema estará 100% automatizado.**
