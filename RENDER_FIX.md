# 🔧 SOLUCIÓN: Error "yarn start" en Render

## ❌ Problema Actual

Render está detectando el proyecto como Node.js/Yarn en lugar de .NET porque hay un `package.json` en la raíz del monorepo.

```
error Command "start" not found.
```

## ✅ Solución Inmediata

### Opción 1: Reconfigurar el Servicio en Dashboard (RÁPIDO - 2 minutos)

1. Ve a: https://dashboard.render.com/
2. Selecciona tu servicio: **motocross-8m01**
3. Ve a **Settings** (pestaña superior)

4. **Actualiza estos valores:**

#### Runtime
```
Runtime: Docker
```
(Render auto-detectará .NET desde Docker)

O si no funciona, prueba:
```
Runtime: .NET
```

#### Root Directory
```
backend
```
⚠️ MUY IMPORTANTE: Debe ser exactamente `backend`

#### Build Command
```
dotnet restore && dotnet build --configuration Release
```

#### Start Command
```
cd src/Api && dotnet run --no-build --configuration Release --urls http://0.0.0.0:$PORT
```

5. **Guarda los cambios** (botón "Save Changes" abajo)
6. **Manual Deploy:** Clic en "Manual Deploy" → "Deploy latest commit"

---

### Opción 2: Usar render.yaml (RECOMENDADO - pero requiere recrear servicio)

Los archivos ya están creados en tu repositorio:
- ✅ `render.yaml` (raíz del proyecto)
- ✅ `.render-buildpacks.yml` (fuerza .NET runtime)

**Pasos:**

1. **Elimina el servicio actual:**
   - Dashboard → motocross-8m01 → Settings → Delete Service

2. **Crea nuevo servicio desde render.yaml:**
   - Dashboard → "New +"
   - Selecciona "Blueprint"
   - Conecta tu repositorio: `ClaudioVilas/motocross`
   - Render detectará automáticamente el `render.yaml`
   - Clic en "Apply"

3. **Render creará automáticamente:**
   - ✅ Web Service (motocross-api)
   - ✅ PostgreSQL Database (motocross-db)
   - ✅ Todas las variables de entorno
   - ✅ Configuración correcta de .NET

---

## 🔍 Verificación

Una vez que el deploy termine exitosamente, verifica:

```bash
curl https://motocross-8m01.onrender.com/health
```

Debe responder:
```json
{
  "status": "healthy",
  "timestamp": "..."
}
```

---

## 📝 Lo que se arregló

### Archivos creados/actualizados:

1. **`render.yaml`** (raíz del proyecto)
   - Configuración explícita del servicio .NET
   - `rootDir: backend` para indicar dónde está el código
   - Comandos correctos de build y start

2. **`.render-buildpacks.yml`**
   - Fuerza el uso del buildpack de .NET
   - Evita auto-detección de Node.js

3. **Commits sincronizados con GitHub**
   - Los archivos están en el repositorio
   - Listos para usar en Render

---

## 🚨 Error Común: "No se encuentra dotnet"

Si después de cambiar a .NET runtime ves:
```
dotnet: command not found
```

**Solución:** Cambia el Runtime a **"Docker"** en Settings y usa este Dockerfile:

Crear `backend/Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet build -c Release

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE $PORT
CMD cd src/Api && dotnet run --no-build --configuration Release --urls http://0.0.0.0:$PORT
```

---

## ⏱️ Tiempo Estimado

- **Opción 1 (Reconfigurar):** ~2-3 minutos + 5-10 minutos de deploy
- **Opción 2 (Recrear con YAML):** ~5 minutos + 5-10 minutos de deploy

---

## 💡 Recomendación

**USA OPCIÓN 1** primero (reconfigurar). Es más rápido y no pierdes el servicio actual.

Si no funciona, entonces usa Opción 2 (recrear con render.yaml).

---

## ✅ Checklist Post-Deploy

- [ ] Runtime configurado a `.NET` o `Docker`
- [ ] Root Directory: `backend`
- [ ] Build Command correcto
- [ ] Start Command correcto
- [ ] Variables de entorno configuradas
- [ ] Health endpoint responde
- [ ] Logs muestran "Application started"

---

**🎯 Una vez que funcione, la URL seguirá siendo:**
```
https://motocross-8m01.onrender.com
```
