# 🚀 Guía Completa: Deploy en Render - Backend + PostgreSQL

## 📋 Resumen

Esta guía te lleva paso a paso para desplegar:
1. **Base de datos PostgreSQL** en Render (Free Tier)
2. **API .NET 8** en Render (Free Tier)
3. **Migraciones automáticas** al iniciar la aplicación
4. **Configuración de variables de entorno**

---

## ✅ Prerrequisitos

- ✅ Código subido a GitHub: https://github.com/ClaudioVilas/motocross
- ✅ Cuenta en Render: https://render.com (crear si no tienes)
- ✅ Backend compila sin errores: `dotnet build`

---

## 🗄️ PASO 1: Crear Base de Datos PostgreSQL

### 1.1 Ir a Render Dashboard

1. Abre: https://dashboard.render.com/
2. Inicia sesión con tu cuenta

### 1.2 Crear Nueva Base de Datos

1. Clic en **"New +"** (arriba derecha)
2. Selecciona **"PostgreSQL"**

### 1.3 Configurar PostgreSQL

```
Name: motocross-db
Database: motocross
User: motocross_user
Region: Oregon (US West) - o el más cercano a ti
PostgreSQL Version: 15
Datadog API Key: (dejar vacío)
Plan: Free
```

3. Clic en **"Create Database"**

### 1.4 Esperar a que Esté Disponible

- Estado cambiará de "Creating" → "Available"
- Toma ~2-3 minutos
- **NO CIERRES** esta pantalla, la necesitarás en el siguiente paso

### 1.5 Copiar Connection String

Una vez que esté "Available":

1. Ve a la pestaña **"Info"**
2. Busca **"Internal Database URL"**
3. Clic en el ícono de copiar 📋
4. Guárdala temporalmente (la usaremos en el Paso 2)

**Ejemplo de Internal Database URL:**
```
postgresql://motocross_user:XXXXXXXX@dpg-xxxxx-a/motocross?sslmode=require
```

> ⚠️ **Importante:** Usa **"Internal Database URL"**, NO la "External Database URL"

---

## 🌐 PASO 2: Crear Web Service (API Backend)

### 2.1 Crear Nuevo Servicio

1. Vuelve al Dashboard: https://dashboard.render.com/
2. Clic en **"New +"** → **"Web Service"**

### 2.2 Conectar Repositorio GitHub

1. Selecciona **"Build and deploy from a Git repository"**
2. Clic en **"Next"**
3. Si es la primera vez:
   - Clic en **"Connect GitHub"**
   - Autoriza Render en GitHub
   - Selecciona **"All repositories"** o solo **"ClaudioVilas/motocross"**
4. Busca y selecciona: **ClaudioVilas/motocross**
5. Clic en **"Connect"**

### 2.3 Configurar Web Service

Completa el formulario:

```
Name: motocross-api
Region: Oregon (US West) - MISMO que la base de datos
Branch: main
Root Directory: (dejar vacío)
Runtime: .NET
Build Command:
cd backend && dotnet restore && dotnet build --configuration Release

Start Command:
cd backend/src/Api && dotnet run --no-build --configuration Release --urls http://0.0.0.0:$PORT

Plan: Free
```

> 💡 **Nota:** El comando de Start es importante - usa `$PORT` que Render proporciona dinámicamente.

### 2.4 Configurar Variables de Entorno

**ANTES** de hacer clic en "Create Web Service", scrollea hacia abajo hasta **"Environment Variables"**:

#### Variable 1: Connection String

```
Key: ConnectionStrings__DefaultConnection
Value: [PEGA AQUÍ LA INTERNAL DATABASE URL DEL PASO 1.5]
```

**Ejemplo:**
```
Key: ConnectionStrings__DefaultConnection
Value: postgresql://motocross_user:XXXXXXXX@dpg-xxxxx-a/motocross?sslmode=require
```

> ⚠️ **Importante:** Usa dos guiones bajos `__` entre ConnectionStrings y DefaultConnection

#### Variable 2: Entorno de Producción

```
Key: ASPNETCORE_ENVIRONMENT
Value: Production
```

#### Variable 3: Frontend URL (temporal)

```
Key: Frontend__Url
Value: http://localhost:5173
```

> 💡 **Nota:** Actualizaremos esto después de desplegar el frontend

### 2.5 Crear el Servicio

1. Verifica que todas las configuraciones sean correctas
2. Clic en **"Create Web Service"**

### 2.6 Esperar el Primer Deploy

El deploy tomará ~5-10 minutos la primera vez:

1. Verás los logs en tiempo real
2. Render hará:
   - Clone del repositorio
   - Restaurar dependencias .NET
   - Build del proyecto
   - Ejecutar las migraciones automáticas
   - Iniciar la aplicación

**Lo que verás en los logs:**

```
==> Cloning from https://github.com/ClaudioVilas/motocross...
==> Running build command 'cd backend && dotnet restore && dotnet build...'
==> Build successful
==> Starting service with 'cd backend/src/Api && dotnet run...'
==> Applying database migrations...
==> Application started on port XXXXX
==> Your service is live 🎉
```

---

## ✅ PASO 3: Verificar el Despliegue

### 3.1 Obtener la URL del Backend

Una vez que el deploy termine:

1. En el Dashboard de Render, ve a tu servicio **"motocross-api"**
2. Arriba verás la URL, algo como:
   ```
   https://motocross-api.onrender.com
   ```
3. **Cópiala** - la necesitarás para el frontend

### 3.2 Probar el Health Endpoint

Abre tu navegador o usa curl:

```bash
curl https://motocross-api.onrender.com/health
```

**Respuesta esperada:**
```json
{
  "status": "healthy",
  "timestamp": "2026-05-18T..."
}
```

### 3.3 Verificar Swagger (Opcional)

Si está en Development, puedes acceder a:
```
https://motocross-api.onrender.com/swagger
```

> ⚠️ **Nota:** Swagger solo está disponible en Development. En Production estará deshabilitado (como debe ser).

### 3.4 Verificar Base de Datos

#### Opción A: Desde Render

1. Ve a tu PostgreSQL database en Render
2. Pestaña **"Connect"**
3. Usa el **"PSQL Command"** para conectarte
4. Ejecuta:
   ```sql
   \dt
   ```
   Deberías ver las tablas: Users, Sessions, TrackingPoints, Laps

#### Opción B: Probar Endpoints

```bash
# Crear una sesión de prueba
curl -X POST https://motocross-api.onrender.com/api/sessions \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Session","description":"Primera prueba"}'

# Obtener sesiones
curl https://motocross-api.onrender.com/api/sessions
```

---

## 🔧 PASO 4: Configuración Adicional

### 4.1 Actualizar Frontend URL (Después de desplegar frontend)

1. Ve a tu servicio **motocross-api** en Render
2. Pestaña **"Environment"**
3. Edita la variable `Frontend__Url`
4. Cambia a: `https://tu-frontend.vercel.app`
5. Guarda → El servicio se reiniciará automáticamente

### 4.2 Habilitar Auto-Deploy

Por defecto, Render hace auto-deploy en cada push a `main`:

1. Ve a **Settings** del servicio
2. Verifica que **"Auto-Deploy"** esté **ON**
3. Ahora cada vez que hagas `git push`, Render desplegará automáticamente

---

## 📊 Monitoreo y Logs

### Ver Logs en Tiempo Real

1. Ve a tu servicio **motocross-api**
2. Pestaña **"Logs"**
3. Verás los logs de la aplicación en tiempo real

### Eventos Comunes en Logs

**Aplicación iniciando:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://0.0.0.0:10000
info: Microsoft.Hosting.Lifetime[0]
      Application started.
```

**Migraciones ejecutándose:**
```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executing DbCommand [Parameters=[], CommandType='Text'...]
```

**Requests HTTP:**
```
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/1.1 GET https://motocross-api.onrender.com/health
```

---

## 🆘 Troubleshooting

### Problema 1: "Build failed"

**Error común:**
```
error NU1102: Unable to find package Microsoft.AspNetCore.Identity
```

**Solución:**
- Verifica que `Motocross.Application.csproj` tenga la versión correcta del paquete
- Nuestro proyecto usa `Microsoft.AspNetCore.Identity` version `2.3.10`

### Problema 2: "Database connection failed"

**Error:**
```
Npgsql.NpgsqlException: Connection failed
```

**Soluciones:**
1. Verifica que la variable `ConnectionStrings__DefaultConnection` esté configurada
2. Usa **Internal Database URL**, no External
3. Verifica que backend y database estén en la **misma región**

### Problema 3: "Migrations not running"

**Solución:**
- Las migraciones se ejecutan automáticamente en `Program.cs`
- Revisa los logs para ver si hubo errores
- Si necesitas ejecutar manualmente:
  ```bash
  # En tu máquina local
  cd backend/src/Api
  dotnet ef migrations add InitialCreate
  git add . && git commit -m "Add migrations" && git push
  ```

### Problema 4: "Service keeps crashing"

**Solución:**
1. Ve a **Logs** y busca el error
2. Errores comunes:
   - Puerto incorrecto: Asegúrate de usar `$PORT` en el Start Command
   - Connection string mal formado: Verifica los `__` (doble guión bajo)
   - Dependencias faltantes: Re-ejecuta el build

### Problema 5: "CORS errors desde frontend"

**Solución:**
1. Verifica que `Frontend__Url` esté configurado correctamente
2. Debe incluir el protocolo: `https://tu-app.vercel.app`
3. Sin trailing slash al final
4. Restart el servicio después de cambiar la variable

---

## 📝 Comandos Útiles

### Ver estado de la base de datos

Desde tu terminal local:
```bash
# Conectar a PostgreSQL en Render
# (Usa el "External Database URL" de Render → Database → Connect)
psql postgresql://motocross_user:XXXXX@dpg-xxxxx.oregon-postgres.render.com/motocross

# Dentro de psql:
\dt                    # Listar tablas
\d+ Users             # Ver estructura de tabla Users
SELECT COUNT(*) FROM Sessions;  # Contar sesiones
```

### Forzar re-deploy

```bash
# En tu repositorio local
git commit --allow-empty -m "Force redeploy"
git push origin main
```

### Ver variables de entorno configuradas

```bash
# En Render Dashboard → Service → Environment
# O desde el terminal de Render (Shell tab):
printenv | grep -i connection
```

---

## ✅ Checklist Final

- [ ] PostgreSQL database creada y "Available"
- [ ] Web Service creado y desplegado
- [ ] Health endpoint responde: `/health`
- [ ] Logs muestran "Application started"
- [ ] No hay errores en los logs
- [ ] Variables de entorno configuradas correctamente
- [ ] Base de datos tiene las tablas creadas (Users, Sessions, etc.)
- [ ] URL del backend guardada para configurar frontend

---

## 🎯 Próximo Paso

Una vez que el backend esté funcionando:

1. **Guarda la URL del backend**: `https://motocross-api.onrender.com`
2. **Ve a desplegar el frontend** en Vercel o GitHub Pages
3. **Configura las variables de entorno del frontend** con esta URL
4. **Actualiza `Frontend__Url`** en Render con la URL del frontend

---

## 📚 Recursos Adicionales

- **Render Docs - .NET:** https://render.com/docs/deploy-dotnet
- **Render Docs - PostgreSQL:** https://render.com/docs/databases
- **Logs en tiempo real:** Dashboard → Service → Logs tab
- **Shell acceso:** Dashboard → Service → Shell tab (para debugging)

---

**🚀 URLs Finales:**

- **Backend API:** `https://motocross-8m01.onrender.com`
- **Health Check:** `https://motocross-8m01.onrender.com/health`
- **Swagger (dev):** `https://motocross-8m01.onrender.com/swagger`
- **Database Dashboard:** Render → Databases → motocross-db

---

💡 **Tip:** Render Free Tier duerme después de 15 minutos de inactividad. La primera request puede tardar ~1 minuto en "despertar" el servicio. Esto es normal y esperado en el plan gratuito.

🎉 **¡Listo! Tu backend está en producción con PostgreSQL funcionando.**
