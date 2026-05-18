# ✅ SETUP COMPLETO - PRÓXIMOS PASOS

## 📊 Estado Actual

✅ Código completo y funcional (155 archivos, 26,655+ líneas)  
✅ Backend .NET 8 con Clean Architecture  
✅ Frontend React + TypeScript + Vite  
✅ Git inicializado y 4 commits creados  
✅ Repositorio GitHub creado: https://github.com/ClaudioVilas/motocross  
❌ **Falta:** Push a GitHub y despliegue

## 🚀 Paso 1: Push a GitHub (2 minutos)

### Opción A: HTTPS con Token (MÁS RÁPIDO)

1. **Crear Personal Access Token:**
   - Ve a: https://github.com/settings/tokens/new
   - Note: "Motocross Project"
   - Expiration: 90 días
   - Marca: ✅ **repo** (todos los permisos)
   - Clic en "Generate token"
   - **COPIA EL TOKEN AHORA** (no lo verás de nuevo)

2. **Push:**
   ```bash
   cd /Users/claudiovilas/Downloads/Proyectos/Motocross
   git push -u origin main
   ```
   
   Cuando pida:
   - **Username:** ClaudioVilas
   - **Password:** [pega tu token aquí]

3. **Verificar:**
   - Ve a: https://github.com/ClaudioVilas/motocross
   - Deberías ver todos tus archivos

### Opción B: SSH (si prefieres)
📖 Ver: [PUSH_TO_GITHUB.md](PUSH_TO_GITHUB.md#option-2-setup-ssh-keys-one-time-setup---5-minutes)

---

## 🌐 Paso 2: Deploy Backend en Render (5 minutos)

### 2.1 Crear Web Service

1. Ve a: https://dashboard.render.com/
2. Clic en "New +" → "Web Service"
3. Conecta tu repositorio GitHub: `ClaudioVilas/motocross`
4. Configura:
   ```
   Name: motocross-api
   Runtime: .NET
   Branch: main
   Root Directory: backend
   Build Command: dotnet restore && dotnet build --configuration Release
   Start Command: cd src/Api && dotnet run --no-build --configuration Release --urls http://0.0.0.0:$PORT
   Plan: Free
   ```
5. Clic en "Create Web Service"

### 2.2 Crear PostgreSQL Database

1. En Render Dashboard → "New +" → "PostgreSQL"
2. Configura:
   ```
   Name: motocross-db
   Database: motocross
   User: motocross_user
   Region: Oregon (US West)
   Plan: Free
   ```
3. Clic en "Create Database"
4. Espera a que esté disponible (2-3 minutos)

### 2.3 Conectar Database al Web Service

1. Ve a tu Web Service → "Environment"
2. Add Environment Variable:
   ```
   Key: ConnectionStrings__DefaultConnection
   Value: [copiar Internal Database URL de PostgreSQL]
   ```
3. Add otra variable:
   ```
   Key: ASPNETCORE_ENVIRONMENT
   Value: Production
   ```
4. **Guarda la URL del backend** (ej: `https://motocross-api.onrender.com`)

---

## 🎨 Paso 3: Deploy Frontend en Vercel (3 minutos)

### Opción A: Vercel CLI (Recomendado)

```bash
cd /Users/claudiovilas/Downloads/Proyectos/Motocross/frontend
npm install -g vercel
vercel login
vercel --prod
```

Cuando pregunte:
- Set up and deploy: **Yes**
- Which scope: Tu cuenta
- Link to existing project: **No**
- Project name: **motocross**
- Directory: `./`
- Override settings: **No**

### Opción B: Vercel Dashboard

1. Ve a: https://vercel.com/new
2. Import `ClaudioVilas/motocross`
3. Configura:
   ```
   Framework Preset: Vite
   Root Directory: frontend
   Build Command: npm run build
   Output Directory: dist
   ```
4. **NO HAGAS DEPLOY TODAVÍA**

### 3.1 Configurar Variables de Entorno

En Vercel → Project Settings → Environment Variables:

```bash
# Backend URL (de Render - Paso 2)
VITE_API_BASE_URL=https://motocross-api.onrender.com

# SignalR Hub URL
VITE_SIGNALR_HUB_URL=https://motocross-api.onrender.com/hubs/tracking

# Mapbox Token (opcional por ahora)
VITE_MAPBOX_TOKEN=pk.ey... (obtener de https://mapbox.com)
```

### 3.2 Deploy

- Si usaste CLI: Ya está desplegado
- Si usaste Dashboard: Clic en "Deploy"

**Guarda la URL del frontend** (ej: `https://motocross-claudiovilas.vercel.app`)

---

## 🔧 Paso 4: Actualizar CORS en Backend (1 minuto)

1. Ve a Render → Tu Web Service → Environment
2. Add Environment Variable:
   ```
   Key: Frontend__Url
   Value: https://motocross-claudiovilas.vercel.app
   ```
3. Guarda (el servicio se reiniciará automáticamente)

---

## ✅ Paso 5: Verificar que Todo Funciona

### Backend
```bash
curl https://motocross-api.onrender.com/health
# Debería responder: {"status":"healthy","timestamp":"..."}
```

### Frontend
1. Abre tu URL de Vercel en el navegador
2. Abre DevTools (F12) → Console
3. Crea una nueva sesión
4. Inicia el tracking (permite permisos de ubicación)
5. Verifica que se reciben puntos GPS

### SignalR (Real-time)
1. DevTools → Network → WS
2. Deberías ver conexión WebSocket activa
3. Al rastrear, deberías ver mensajes en tiempo real

---

## 📱 Paso 6: Configurar Secrets de GitHub (Opcional - para CI/CD)

Solo si quieres CI/CD automático:

### En GitHub → Settings → Secrets and variables → Actions

**Para Frontend:**
```
VERCEL_TOKEN=[de Vercel Settings → Tokens]
VERCEL_ORG_ID=[de Vercel Project Settings]
VERCEL_PROJECT_ID=[de Vercel Project Settings]
VITE_API_BASE_URL=https://motocross-api.onrender.com
VITE_SIGNALR_HUB_URL=https://motocross-api.onrender.com/hubs/tracking
VITE_MAPBOX_TOKEN=[tu token de Mapbox]
```

**Para Backend:**
```
RENDER_SERVICE_ID=[de Render Service Settings]
RENDER_API_KEY=[de Render Account → API Keys]
```

---

## 🎯 URLs Finales

Después de completar todos los pasos:

- 🌐 **GitHub:** https://github.com/ClaudioVilas/motocross
- 🎨 **Frontend:** https://motocross-[tu-usuario].vercel.app
- 🔧 **Backend:** https://motocross-api.onrender.com
- 📊 **Database:** Render PostgreSQL (interno)

---

## 📝 Checklist de Despliegue

- [ ] Push a GitHub completado
- [ ] Backend desplegado en Render
- [ ] PostgreSQL creado y conectado
- [ ] Frontend desplegado en Vercel
- [ ] Variables de entorno configuradas
- [ ] CORS actualizado en backend
- [ ] Health check del backend responde
- [ ] Frontend carga sin errores
- [ ] Crear sesión funciona
- [ ] Tracking GPS funciona
- [ ] SignalR conecta correctamente
- [ ] Puntos de tracking se reciben en tiempo real

---

## 🆘 Problemas Comunes

### "fatal: Authentication failed"
→ Usa tu **token** como password, no tu contraseña de GitHub

### Backend en Render: "Build failed"
→ Verifica que Root Directory sea "backend"
→ Verifica que Build Command sea correcto

### Frontend: "Failed to fetch"
→ Verifica VITE_API_BASE_URL en Vercel
→ Verifica que backend esté corriendo (curl /health)
→ Verifica CORS (Frontend__Url en Render)

### "Cannot read from remote repository"
→ Usa HTTPS en lugar de SSH
→ O configura SSH keys: [PUSH_TO_GITHUB.md](PUSH_TO_GITHUB.md)

---

## 💡 Tips

1. **Render Free Tier:** Se duerme después de 15 minutos de inactividad
   - Primera request puede tardar 1 minuto en despertar
   - Es normal, no es un error

2. **PostgreSQL Free Tier:** 
   - 1GB de almacenamiento
   - Suficiente para miles de sesiones

3. **Vercel Free Tier:**
   - Generoso para proyectos personales
   - Deploy automático en cada push

4. **Logs:**
   - Render: Service → Logs
   - Vercel: Project → Deployments → [deployment] → Function Logs

---

## 🎉 ¡Ya Casi Está!

**Tiempo total estimado:** 15 minutos

Una vez completados estos pasos, tendrás:
- ✅ Aplicación web en vivo
- ✅ API backend funcionando
- ✅ Base de datos en producción
- ✅ Real-time con SignalR
- ✅ CI/CD automático

**Después puedes:**
- 🏍️ Rastrear sesiones reales
- 📊 Ver estadísticas de vueltas
- 🔄 Recibir updates en tiempo real
- 📱 Usar desde tu móvil (PWA)

---

📖 **Documentación completa:**
- [PUSH_TO_GITHUB.md](PUSH_TO_GITHUB.md) - Guía detallada de push
- [SETUP_COMPLETE.md](SETUP_COMPLETE.md) - Información completa del proyecto
- [docs/DEPLOYMENT.md](docs/DEPLOYMENT.md) - Guía técnica de despliegue

🚀 **¡Comienza con el Paso 1 ahora!**
