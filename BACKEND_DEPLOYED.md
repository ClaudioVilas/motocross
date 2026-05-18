# ✅ Backend Desplegado - Configuración Rápida

## 🌐 URL del Backend (Render)

```
https://motocross-8m01.onrender.com
```

## ✅ Endpoints Disponibles

### Health Check
```bash
curl https://motocross-8m01.onrender.com/health
```

Respuesta:
```json
{
  "status": "healthy",
  "timestamp": "2026-05-18T..."
}
```

### API Base
```
https://motocross-8m01.onrender.com/api
```

### SignalR Hub
```
https://motocross-8m01.onrender.com/hubs/tracking
```

### Swagger (si está habilitado)
```
https://motocross-8m01.onrender.com/swagger
```

---

## 🚀 Configurar Frontend para Usar Este Backend

### Opción 1: Variables de Entorno Locales

Crea `frontend/.env.local`:

```env
VITE_API_BASE_URL=https://motocross-8m01.onrender.com
VITE_SIGNALR_HUB_URL=https://motocross-8m01.onrender.com/hubs/tracking
VITE_MAPBOX_TOKEN=
```

Luego ejecuta:
```bash
cd frontend
npm run dev
```

### Opción 2: Deploy en Vercel

1. Ve a: https://vercel.com/new
2. Importa el repositorio: `ClaudioVilas/motocross`
3. Configura:
   - **Framework Preset:** Vite
   - **Root Directory:** `frontend`
   - **Build Command:** `npm run build`
   - **Output Directory:** `dist`

4. **Variables de Entorno en Vercel:**

```
VITE_API_BASE_URL=https://motocross-8m01.onrender.com
VITE_SIGNALR_HUB_URL=https://motocross-8m01.onrender.com/hubs/tracking
VITE_MAPBOX_TOKEN=(tu token de Mapbox)
```

5. Clic en **Deploy**

### Opción 3: Deploy en GitHub Pages

Ya configurado en el workflow. Solo asegúrate de que las variables de entorno estén en GitHub Secrets:

1. Ve a: https://github.com/ClaudioVilas/motocross/settings/secrets/actions
2. Agrega:
   - `VITE_API_BASE_URL` = `https://motocross-8m01.onrender.com`
   - `VITE_SIGNALR_HUB_URL` = `https://motocross-8m01.onrender.com/hubs/tracking`

---

## 🔧 Actualizar CORS en Render

Una vez que tengas la URL del frontend (Vercel o GitHub Pages), actualiza el backend:

1. Ve a: https://dashboard.render.com/
2. Selecciona tu servicio: **motocross-8m01**
3. Ve a **Environment**
4. Edita o agrega:
   ```
   Key: Frontend__Url
   Value: https://tu-frontend-url.vercel.app
   ```
5. Guarda → El servicio se reiniciará automáticamente

---

## 📊 Endpoints de la API

### Sesiones

```bash
# Crear sesión
curl -X POST https://motocross-8m01.onrender.com/api/sessions \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Session","description":"Primera prueba"}'

# Obtener todas las sesiones
curl https://motocross-8m01.onrender.com/api/sessions

# Obtener sesión activa
curl https://motocross-8m01.onrender.com/api/sessions/active

# Obtener sesión por ID
curl https://motocross-8m01.onrender.com/api/sessions/{id}
```

### Autenticación

```bash
# Registrar usuario
curl -X POST https://motocross-8m01.onrender.com/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!","displayName":"Test User"}'

# Login
curl -X POST https://motocross-8m01.onrender.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'
```

---

## ⚡ Notas Importantes

### Free Tier de Render
- El servicio se duerme después de **15 minutos** de inactividad
- La primera request después de dormir puede tardar **30-60 segundos**
- Esto es normal y esperado en el plan gratuito

### CORS
- Por defecto acepta:
  - `http://localhost:5173` (Vite dev)
  - `http://localhost:3000`
  - `https://*.vercel.app`
- Para otras URLs, actualiza `Frontend__Url` en Render

### Base de Datos
- PostgreSQL 15 en Render
- Migraciones automáticas al iniciar
- Tablas: Users, Sessions, TrackingPoints, Laps

---

## 🎯 Estado del Deployment

✅ Backend API desplegado y funcionando  
✅ PostgreSQL database conectada  
✅ Migraciones automáticas configuradas  
✅ SignalR WebSocket habilitado  
✅ CORS configurado para desarrollo  
✅ Health check respondiendo  

📋 **Pendiente:**
- [ ] Desplegar frontend en Vercel o GitHub Pages
- [ ] Configurar variables de entorno del frontend
- [ ] Actualizar `Frontend__Url` en Render
- [ ] Obtener token de Mapbox (opcional)

---

## 📚 Recursos

- **Dashboard Render:** https://dashboard.render.com/
- **Logs del Backend:** Dashboard → motocross-8m01 → Logs
- **Repositorio GitHub:** https://github.com/ClaudioVilas/motocross
- **Documentación API:** Ver `docs/API.md`

---

**🎉 Backend completamente operativo en producción!**
