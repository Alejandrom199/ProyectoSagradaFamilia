#!/usr/bin/env bash
# =============================================================================
# setup-railway.sh — Infraestructura completa de Railway para Sagrada Familia
#
# Uso:
#   1. npm install -g @railway/cli
#   2. railway login
#   3. chmod +x setup-railway.sh
#   4. ./setup-railway.sh
#
# IMPORTANTE: Copia .env.railway.example → .env.railway y rellena los secretos
# antes de ejecutar. El script los leerá desde ahí.
# =============================================================================

set -e

RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33d'; CYAN='\033[0;36m'; NC='\033[0m'
log()   { echo -e "${GREEN}[OK]${NC} $1"; }
warn()  { echo -e "${YELLOW}[!!]${NC} $1"; }
err()   { echo -e "${RED}[ERROR]${NC} $1"; exit 1; }
info()  { echo -e "${CYAN}[--]${NC} $1"; }
paso()  { echo ""; echo -e "${CYAN}══════ $1 ══════${NC}"; }

# ── Cargar secretos desde .env.railway ───────────────────────────────────────
ENV_FILE=".env.railway"
if [ ! -f "$ENV_FILE" ]; then
  err "No existe $ENV_FILE. Copia .env.railway.example → .env.railway y rellena los valores."
fi
source "$ENV_FILE"

# Validar que las variables críticas estén definidas
[ -z "$SUPABASE_PASSWORD" ]     && err "SUPABASE_PASSWORD no está definido en $ENV_FILE"
[ -z "$SUPABASE_POOLER_HOST" ]  && err "SUPABASE_POOLER_HOST no está definido en $ENV_FILE"
[ -z "$SUPABASE_PROJECT_REF" ]  && err "SUPABASE_PROJECT_REF no está definido en $ENV_FILE"
[ -z "$JWT_SECRET_KEY" ]        && err "JWT_SECRET_KEY no está definido en $ENV_FILE"
[ -z "$GMAIL_APP_PASSWORD" ]    && err "GMAIL_APP_PASSWORD no está definido en $ENV_FILE"
[ -z "$NETLIFY_URL" ]           && err "NETLIFY_URL no está definido en $ENV_FILE"

echo ""
echo "╔══════════════════════════════════════════════════╗"
echo "║    Sagrada Familia — Setup de Railway            ║"
echo "╚══════════════════════════════════════════════════╝"

railway whoami 2>/dev/null || err "No estás logueado. Ejecuta: railway login"
log "Autenticado en Railway"

# ── Proyecto ──────────────────────────────────────────────────────────────────
paso "PROYECTO"
warn "Creando proyecto sagrada-familia..."
railway init --name "sagrada-familia"
log "Proyecto creado"

# =============================================================================
# SERVICIO 1: Backend .NET
# =============================================================================
paso "BACKEND .NET"
railway service create --name ProyectoSagradaFamilia
log "Servicio creado"

warn "Configurando variables del backend..."
railway variables set \
  "ConnectionStrings__PostgresConnection=Host=${SUPABASE_POOLER_HOST};Port=5432;Database=postgres;Username=postgres.${SUPABASE_PROJECT_REF};Password=${SUPABASE_PASSWORD};SSL Mode=Require;Trust Server Certificate=true;" \
  "JwtSettings__SecretKey=${JWT_SECRET_KEY}" \
  "JwtSettings__Issuer=SagradaFamiliaAPI" \
  "JwtSettings__Audience=SagradaFamiliaClient" \
  "JwtSettings__ExpirationMinutes=60" \
  "JwtSettings__RefreshTokenExpirationDays=7" \
  "AppSettings__FrontendUrl=${NETLIFY_URL}" \
  "SmtpSettings__Host=smtp.gmail.com" \
  "SmtpSettings__Port=587" \
  "SmtpSettings__EnableSsl=true" \
  "SmtpSettings__FromName=Sagrada Familia" \
  "SmtpSettings__FromEmail=alejandrom199916@gmail.com" \
  "SmtpSettings__Username=alejandrom199916@gmail.com" \
  "SmtpSettings__Password=${GMAIL_APP_PASSWORD}" \
  "ProphetApi__BaseUrl=PENDIENTE_VER_PASO_3" \
  --service ProyectoSagradaFamilia
log "Variables del backend configuradas"

warn "Generando dominio público..."
railway domain --service ProyectoSagradaFamilia 2>/dev/null || warn "Generar dominio manualmente"

# =============================================================================
# SERVICIO 2: Predictor Python
# =============================================================================
paso "PREDICTOR PYTHON"
railway service create --name sagrada-familia-api
log "Servicio creado"

warn "Configurando variables del predictor..."
railway variables set \
  "NET_BACKEND_URL=PENDIENTE_VER_PASO_3" \
  --service sagrada-familia-api
log "Variables del predictor configuradas"

warn "Generando dominio público..."
railway domain --service sagrada-familia-api 2>/dev/null || warn "Generar dominio manualmente"

# =============================================================================
# PASOS MANUALES — leer con atención
# =============================================================================
echo ""
echo "╔══════════════════════════════════════════════════════════════════╗"
echo "║              PASOS MANUALES EN EL DASHBOARD                     ║"
echo "╚══════════════════════════════════════════════════════════════════╝"
echo ""
echo "PASO 1 — Conectar cada servicio a GitHub:"
echo "  ProyectoSagradaFamilia → Settings → Source → GitHub"
echo "    Repo:           Alejandrom199/ProyectoSagradaFamilia"
echo "    Branch:         main"
echo "    Root Directory: SagradaFamilia"
echo ""
echo "  sagrada-familia-api → Settings → Source → GitHub"
echo "    Repo:           Alejandrom199/ProyectoSagradaFamilia"
echo "    Branch:         main"
echo "    Root Directory: sagrada-predict-api"
echo ""
echo "PASO 2 — Puerto del backend (MUY IMPORTANTE - sin esto da 502):"
echo "  ProyectoSagradaFamilia → Settings → Networking"
echo "  Cambiar puerto: 5000 → 8080"
echo ""
echo "PASO 3 — URLs cruzadas (hacerlo DESPUÉS de que ambos servicios"
echo "         tengan su dominio generado en el dashboard):"
echo ""
echo "  En ProyectoSagradaFamilia → Variables:"
echo "    ProphetApi__BaseUrl = https://<dominio-de-sagrada-familia-api>"
echo ""
echo "  En sagrada-familia-api → Variables:"
echo "    NET_BACKEND_URL = https://<dominio-de-ProyectoSagradaFamilia>"
echo ""
echo "PASO 4 — Deploy inicial de cada servicio:"
echo "  cd SagradaFamilia && railway up --service ProyectoSagradaFamilia --detach"
echo "  cd sagrada-predict-api && railway up --service sagrada-familia-api --detach"
echo ""
echo "PASO 5 — Verificar que el backend responde:"
echo "  curl https://<dominio-backend>/health"
echo "  (debe devolver 200)"
echo ""
log "Script completado — ahora sigue los pasos manuales de arriba"
