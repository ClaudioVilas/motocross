#!/bin/bash
set -e

echo "🔥 Instalación de Herramientas para Firebase + Google Cloud"
echo "============================================================"
echo ""

# Check if running on macOS
if [[ "$OSTYPE" != "darwin"* ]]; then
    echo "❌ Este script es para macOS. Para otros sistemas, instala manualmente."
    exit 1
fi

# Install Google Cloud SDK
if ! command -v gcloud &> /dev/null; then
    echo "📦 Instalando Google Cloud SDK..."
    curl https://sdk.cloud.google.com | bash
    exec -l $SHELL
else
    echo "✅ Google Cloud SDK ya está instalado"
fi

# Install Firebase CLI
if ! command -v firebase &> /dev/null; then
    echo "📦 Instalando Firebase CLI..."
    curl -sL https://firebase.tools | bash
else
    echo "✅ Firebase CLI ya está instalado"
fi

# Check Docker
if ! command -v docker &> /dev/null; then
    echo "⚠️  Docker no está instalado."
    echo "   Descárgalo desde: https://www.docker.com/products/docker-desktop"
    echo ""
else
    echo "✅ Docker ya está instalado"
fi

echo ""
echo "✅ Instalación completada!"
echo ""
echo "📋 Próximos pasos:"
echo "   1. Reinicia tu terminal"
echo "   2. Ejecuta: gcloud auth login"
echo "   3. Ejecuta: firebase login"
echo "   4. Ejecuta: ./FIREBASE_SETUP.sh"
