# 🚀 Guía de Instalación Rápida - Firebase + Google Cloud

## ⚡ Instalación Automática (Recomendada)

```bash
chmod +x INSTALL_TOOLS.sh
./INSTALL_TOOLS.sh
```

---

## 📦 Instalación Manual

### 1. Google Cloud SDK

**macOS:**
```bash
curl https://sdk.cloud.google.com | bash
exec -l $SHELL
```

**Windows:**
Descargar desde: https://cloud.google.com/sdk/docs/install

**Linux:**
```bash
curl https://sdk.cloud.google.com | bash
exec -l $SHELL
```

### 2. Firebase CLI

**Usando npm (recomendado):**
```bash
npm install -g firebase-tools
```

**O usando curl:**
```bash
curl -sL https://firebase.tools | bash
```

### 3. Docker Desktop

**macOS/Windows:**
Descargar desde: https://www.docker.com/products/docker-desktop

**Linux:**
```bash
# Ubuntu/Debian
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
```

---

## 🔐 Autenticación

### Google Cloud

```bash
# Login
gcloud auth login

# Configurar proyecto
gcloud config set project motocross-10576

# Verificar
gcloud config list
```

### Firebase

```bash
# Login
firebase login

# Verificar
firebase projects:list
```

### Docker (para GCR)

```bash
gcloud auth configure-docker
```

---

## ✅ Verificación

```bash
# Verificar versiones
gcloud --version
firebase --version
docker --version

# Verificar autenticación
gcloud auth list
firebase projects:list
```

---

## 🚀 Despliegue

Una vez instalado todo:

```bash
# Ejecutar setup automático
./FIREBASE_SETUP.sh
```

O seguir los pasos en [FIREBASE_DEPLOYMENT.md](FIREBASE_DEPLOYMENT.md)

---

## 🐛 Troubleshooting

### Error: "gcloud: command not found"

Reinicia tu terminal o ejecuta:
```bash
source ~/.bashrc  # o ~/.zshrc si usas zsh
```

### Error: "firebase: command not found"

Si instalaste con npm:
```bash
npm install -g firebase-tools --force
```

### Error: "docker: command not found"

Asegúrate de que Docker Desktop esté corriendo.

---

## 📞 Soporte

Si tienes problemas, consulta:
- [Google Cloud SDK Docs](https://cloud.google.com/sdk/docs)
- [Firebase CLI Docs](https://firebase.google.com/docs/cli)
- [Docker Docs](https://docs.docker.com/)
