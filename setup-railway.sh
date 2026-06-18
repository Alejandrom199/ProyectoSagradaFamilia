#!/usr/bin/env bash
# =============================================================================
# setup-railway.sh — Infraestructura completa de Railway para Sagrada Familia
#
# Uso:
#   1. railway login          (una sola vez)
#   2. chmod +x setup-railway.sh
#   3. ./setup-railway.sh
#
# Requisitos: railway CLI instalado (npm install -g @railway/cli)
# =============================================================================

set -e

# ── Colores ──────────────────────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; NC='\033[0m'
log()  { echo -e "${GREEN}[OK]${NC} $1"; }
warn() { echo -e "${YELLOW}[!!]${NC} $1"; }
err()  { echo -e "${RED}[ERROR]${NC} $1"; exit 1; }

# ── Verificar login ───────────────────────────────────────────────────────────
echo ""
echo "============================================================"
echo "  Sagrada Familia — Setup de Railway"
echo "============================================================"
echo ""
railway whoami 2>/dev/null || err "No estás logueado. Ejecuta: railway login"
log "Autenticado en Railway"

# ── Nombre del proyecto ───────────────────────────────────────────────────────
PROJECT_NAME="sagrada-familia"

# ── Crear proyecto ────────────────────────────────────────────────────────────
echo ""
warn "Creando proyecto '$PROJECT_NAME'..."
railway init --name "$PROJECT_NAME"
log "Proyecto creado"

# =============================================================================
# SERVICIO 1: Backend .NET (ProyectoSagradaFamilia)
# =============================================================================
echo ""
warn "Creando servicio: ProyectoSagradaFamilia (backend .NET)..."
railway service create --name ProyectoSagradaFamilia

warn "Configurando source del backend (GitHub)..."
# Conectar al repo y rama — ajustar si el repo cambia
railway service connect \
  --service ProyectoSagradaFamilia \
  --repo "Alejandrom199/ProyectoSagradaFamilia" \
  --branch "main" \
  --rootDirectory "SagradaFamilia" 2>/dev/null || warn "Conectar repo manualmente en el dashboard (Settings → Source)"

warn "Seteando variables del backend..."
railway variables set \
  "ConnectionStrings__PostgresConnection=Host=aws-1-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.xwajbkydcsvmygvvszza;Password=SUPABASE_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;" \
  "JwtSettings__SecretKey=REEMPLAZAR_CON_CLAVE_SEGURA" \
  "JwtSettings__Issuer=SagradaFamiliaAPI" \
  "JwtSettings__Audience=SagradaFamiliaClient" \
  "JwtSettings__ExpirationMinutes=60" \
  "JwtSettings__RefreshTokenExpirationDays=7" \
  "ProphetApi__BaseUrl=https://PREDICTOR_URL_AQUI" \
  "AppSettings__FrontendUrl=https://startling-frangollo-fa4cb9.netlify.app" \
  "SmtpSettings__Host=smtp.gmail.com" \
  "SmtpSettings__Port=587" \
  "SmtpSettings__EnableSsl=true" \
  "SmtpSettings__FromName=Sagrada Familia" \
  "SmtpSettings__FromEmail=alejandrom199916@gmail.com" \
  "SmtpSettings__Username=alejandrom199916@gmail.com" \
  "SmtpSettings__Password=GMAIL_APP_PASSWORD" \
  --service ProyectoSagradaFamilia

warn "Generando dominio público para el backend..."
railway domain --service ProyectoSagradaFamilia 2>/dev/null || warn "Generar dominio manualmente en el dashboard"

log "Backend configurado"

# =============================================================================
# SERVICIO 2: Predictor Python (sagrada-familia-api)
# =============================================================================
echo ""
warn "Creando servicio: sagrada-familia-api (predictor Python)..."
railway service create --name sagrada-familia-api

warn "Configurando source del predictor (GitHub)..."
railway service connect \
  --service sagrada-familia-api \
  --repo "Alejandrom199/ProyectoSagradaFamilia" \
  --branch "main" \
  --rootDirectory "sagrada-predict-api" 2>/dev/null || warn "Conectar repo manualmente en el dashboard (Settings → Source)"

warn "Seteando variables del predictor..."
railway variables set \
  "NET_BACKEND_URL=https://BACKEND_URL_AQUI" \
  --service sagrada-familia-api

warn "Generando dominio público para el predictor..."
railway domain --service sagrada-familia-api 2>/dev/null || warn "Generar dominio manualmente en el dashboard"

log "Predictor configurado"

# =============================================================================
# PASOS MANUALES (Railway CLI no los soporta todavía)
# =============================================================================
echo ""
echo "============================================================"
echo "  PASOS MANUALES REQUERIDOS EN EL DASHBOARD"
echo "============================================================"
echo ""
echo "1. NETWORKING — Puerto del backend:"
echo "   Dashboard → ProyectoSagradaFamilia → Settings → Networking"
echo "   Cambiar puerto de 5000 a 8080"
echo ""
echo "2. VARIABLES — Reemplazar placeholders en ProyectoSagradaFamilia:"
echo "   - SUPABASE_PASSWORD    → contraseña de Supabase"
echo "   - GMAIL_APP_PASSWORD   → contraseña de app de Gmail (16 chars)"
echo "   - REEMPLAZAR_CON_CLAVE_SEGURA → JWT secret key (min 32 chars)"
echo ""
echo "3. VARIABLES CRUZADAS — Una vez que tengas los dominios:"
echo "   - ProphetApi__BaseUrl en ProyectoSagradaFamilia → URL del predictor"
echo "   - NET_BACKEND_URL en sagrada-familia-api → URL del backend"
echo ""
echo "4. DEPLOY — Triggerear primer deploy de cada servicio:"
echo "   railway up --service ProyectoSagradaFamilia --detach (desde SagradaFamilia/)"
echo "   railway up --service sagrada-familia-api --detach (desde sagrada-predict-api/)"
echo ""
echo "============================================================"
log "Setup completado"
