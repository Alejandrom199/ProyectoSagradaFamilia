# =============================================================================
# Stage 1 · builder
# Compila extensiones nativas, instala dependencias Python y descarga CmdStan.
# Esta imagen NUNCA llega a Docker Hub ni a producción.
#
# ARG APP_ENV:
#   - Existe solo en tiempo de build; no queda en las capas de la imagen final.
#   - Permite diferenciar instalaciones en CI/CD (ej. incluir dependencias de
#     test solo cuando APP_ENV=development).
#   - 'production' es el valor por defecto: sin extras de desarrollo.
# =============================================================================
FROM python:3.12-slim AS builder

ARG APP_ENV=production

WORKDIR /build

# Herramientas de compilación para extensiones C/C++ (numpy, prophet, etc.)
# Solo existen en el builder; la imagen final no las incluye.
RUN apt-get update && apt-get install -y --no-install-recommends \
        build-essential \
        gcc \
        g++ \
        curl \
    && rm -rf /var/lib/apt/lists/*

# Virtualenv aislado — nunca toca el Python del sistema
RUN python -m venv /opt/venv
ENV PATH="/opt/venv/bin:$PATH"

# ── Trick de caché de capas ───────────────────────────────────────────────────
# requirements.txt se copia antes que el código fuente.
# Si solo cambia el código (no las dependencias), Docker reutiliza esta capa
# sin re-descargar los paquetes pip.
COPY requirements.txt .
RUN pip install --upgrade pip \
 && pip install --no-cache-dir -r requirements.txt

# Descarga los binarios precompilados de CmdStan (requerido por Prophet).
# Se ejecuta aquí para que quede cacheado en la capa del builder y no se
# repita en cada build si el requirements.txt no cambió.
RUN python -c "import cmdstanpy; cmdstanpy.install_cmdstan()"


# =============================================================================
# Stage 2 · runtime
# Imagen final: runtime + artefacto. Sin compiladores, sin caché de pip,
# sin herramientas de build. Superficie mínima lista para Docker Hub.
# =============================================================================
FROM python:3.12-slim AS runtime

# libgomp1: runtime de OpenMP requerido por Stan para cálculos en paralelo
RUN apt-get update && apt-get install -y --no-install-recommends \
        libgomp1 \
    && rm -rf /var/lib/apt/lists/*

# Copiar virtualenv y CmdStan desde el stage builder
COPY --from=builder /opt/venv      /opt/venv
COPY --from=builder /root/.cmdstan /root/.cmdstan

# ── Variables de entorno de runtime ──────────────────────────────────────────
#
# REGLA DE SEGURIDAD: los ENV aquí son valores predeterminados NO SECRETOS.
# Si en el futuro se necesita una API key u otro secret, declararlo aquí con
# valor vacío ("") e inyectarlo desde docker-compose o un vault en CI/CD.
# Los ENV son visibles con 'docker inspect' — nunca poner contraseñas.
#
# app/core/config.py lee cada valor con os.getenv(key, default).
# Sobreescribir en docker-compose no requiere reconstruir la imagen.

ENV PATH="/opt/venv/bin:$PATH" \
    PYTHONUNBUFFERED=1 \
    PYTHONDONTWRITEBYTECODE=1 \
    \
    # ── Servidor ──────────────────────────────────────────────────────────────
    PORT="8000" \
    \
    # ── URL del backend .NET ──────────────────────────────────────────────────
    # 'api' es el nombre del servicio en docker-compose.
    # Docker resuelve el nombre vía su DNS interno de red (no es localhost).
    NET_BACKEND_URL="http://api:5000" \
    \
    # ── Parámetros de Prophet — sobreescribibles sin reconstruir la imagen ────
    PROPHET_MIN_MEDIDAS="3" \
    PROPHET_INTERVAL_WIDTH="0.80" \
    PROPHET_CAP_DEFAULT="22.0" \
    PROPHET_FLOOR_DEFAULT="2.5"

WORKDIR /app
COPY . .

EXPOSE 8000

# ── ENTRYPOINT en forma exec (JSON array) ─────────────────────────────────────
# Por qué ENTRYPOINT y no CMD:
#   CMD puede ser reemplazado completamente con 'docker run imagen otro-cmd'.
#   ENTRYPOINT garantiza que uvicorn SIEMPRE arranca; CMD solo añade argumentos
#   adicionales si se necesitan.
#
# Por qué exec form y no shell form:
#   Shell form → Docker invoca /bin/sh -c "uvicorn ..."
#                El shell es PID 1; uvicorn es un proceso hijo.
#                SIGTERM de 'docker stop' va al shell y se pierde.
#   Exec form  → uvicorn es directamente PID 1.
#                SIGTERM llega a uvicorn → graceful shutdown: espera que las
#                peticiones en vuelo terminen antes de cerrar conexiones.
#
# --workers 1: uvicorn con múltiples workers usa fork() de proceso.
#   Prophet/Stan usa estado global de C++; fork después de importar Prophet
#   puede corromper ese estado. Un worker por contenedor es la práctica segura.
#   Para escalar, se lanzan más réplicas del contenedor (horizontal scaling).
ENTRYPOINT ["sh", "-c", "exec uvicorn main:app --host 0.0.0.0 --port ${PORT:-8000} --workers 1"]
