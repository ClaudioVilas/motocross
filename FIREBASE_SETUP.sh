#!/bin/bash
set -e

echo "🔥 Firebase & Google Cloud Setup para Motocross"
echo "================================================"
echo ""

PROJECT_ID="motocross-10576"
REGION="us-central1"
DB_INSTANCE="motocross-db"
DB_NAME="motocross"
DB_USER="motocross_user"
SERVICE_NAME="motocross-api"

echo "📦 Proyecto: $PROJECT_ID"
echo "🌎 Región: $REGION"
echo ""

# Set project
echo "1️⃣  Configurando proyecto..."
gcloud config set project $PROJECT_ID

# Enable required APIs
echo "2️⃣  Habilitando APIs necesarias..."
gcloud services enable \
  cloudbuild.googleapis.com \
  run.googleapis.com \
  sql-component.googleapis.com \
  sqladmin.googleapis.com \
  containerregistry.googleapis.com \
  cloudresourcemanager.googleapis.com \
  serviceusage.googleapis.com

echo ""
echo "3️⃣  Creando instancia de Cloud SQL (PostgreSQL)..."
gcloud sql instances create $DB_INSTANCE \
  --database-version=POSTGRES_15 \
  --tier=db-f1-micro \
  --region=$REGION \
  --root-password=$(openssl rand -base64 32) \
  --backup \
  --backup-start-time=03:00 \
  --maintenance-window-day=SUN \
  --maintenance-window-hour=02 \
  --maintenance-release-channel=production \
  --availability-type=zonal \
  --storage-type=SSD \
  --storage-size=10GB \
  --storage-auto-increase \
  --database-flags=cloudsql.iam_authentication=on || echo "⚠️  Instancia ya existe"

echo ""
echo "4️⃣  Creando base de datos..."
gcloud sql databases create $DB_NAME \
  --instance=$DB_INSTANCE || echo "⚠️  Base de datos ya existe"

echo ""
echo "5️⃣  Creando usuario de base de datos..."
DB_PASSWORD=$(openssl rand -base64 24)
gcloud sql users create $DB_USER \
  --instance=$DB_INSTANCE \
  --password=$DB_PASSWORD || echo "⚠️  Usuario ya existe"

echo ""
echo "6️⃣  Obteniendo connection name..."
CONNECTION_NAME=$(gcloud sql instances describe $DB_INSTANCE --format='value(connectionName)')
echo "   Connection Name: $CONNECTION_NAME"

# Build and deploy
echo ""
echo "7️⃣  Construyendo imagen Docker..."
cd backend
docker build -t gcr.io/$PROJECT_ID/$SERVICE_NAME:latest .

echo ""
echo "8️⃣  Subiendo imagen a Container Registry..."
docker push gcr.io/$PROJECT_ID/$SERVICE_NAME:latest

echo ""
echo "9️⃣  Desplegando a Cloud Run..."
gcloud run deploy $SERVICE_NAME \
  --image gcr.io/$PROJECT_ID/$SERVICE_NAME:latest \
  --region=$REGION \
  --platform=managed \
  --allow-unauthenticated \
  --set-env-vars="ASPNETCORE_ENVIRONMENT=Production,Frontend__Url=https://$PROJECT_ID.web.app" \
  --set-env-vars="ConnectionStrings__DefaultConnection=Host=/cloudsql/$CONNECTION_NAME;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD" \
  --add-cloudsql-instances=$CONNECTION_NAME \
  --max-instances=10 \
  --min-instances=0 \
  --memory=512Mi \
  --cpu=1 \
  --timeout=300

echo ""
echo "🔟  Obteniendo URL del servicio..."
SERVICE_URL=$(gcloud run services describe $SERVICE_NAME --region=$REGION --format='value(status.url)')

echo ""
echo "1️⃣1️⃣  Configurando Cloud Build trigger..."
cd ..
gcloud builds submit --config=cloudbuild.yaml

echo ""
echo "✅ ¡Setup completado!"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "📝 Información importante:"
echo "   Backend URL: $SERVICE_URL"
echo "   Frontend URL: https://$PROJECT_ID.web.app"
echo "   Database Connection: $CONNECTION_NAME"
echo "   Database Name: $DB_NAME"
echo "   Database User: $DB_USER"
echo "   Database Password: $DB_PASSWORD"
echo ""
echo "🔐 IMPORTANTE: Guarda la contraseña de la base de datos en un lugar seguro."
echo ""
echo "📋 Próximos pasos:"
echo "   1. Actualiza frontend/.env.production con la URL del backend"
echo "   2. Despliega el frontend: npm run build && firebase deploy"
echo "   3. Verifica el servicio: curl $SERVICE_URL/health"
echo ""
echo "🚀 El backend se desplegará automáticamente en cada push a GitHub"
