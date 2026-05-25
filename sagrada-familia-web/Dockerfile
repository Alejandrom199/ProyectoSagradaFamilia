# =============================================================================
# Stage 1 · builder
# Node.js para instalar dependencias y compilar Angular en modo producción.
# Esta imagen NUNCA llega a Docker Hub ni a producción.
#
# ARG BUILD_CONFIGURATION:
#   - Existe solo en tiempo de build; no queda en las capas de la imagen final.
#   - Permite seleccionar la configuración de Angular desde CI/CD:
#       docker build --build-arg BUILD_CONFIGURATION=staging ...
#   - 'production' activa: minificación, tree-shaking, hashes en nombres de
#     archivos, y usa environment.prod.ts (apiUrl: '/api').
# =============================================================================
FROM node:22-alpine AS builder

ARG BUILD_CONFIGURATION=production

WORKDIR /app

# ── Trick de caché de capas ───────────────────────────────────────────────────
# package.json y package-lock.json se copian ANTES que el código fuente.
# Si solo cambia el código (no las dependencias), Docker reutiliza la capa
# de 'npm ci' sin reinstalar node_modules (ahorra 1-3 minutos por build).
COPY package.json package-lock.json ./

# npm install (vs npm ci):
#   El lockfile fue generado en Windows; los paquetes opcionales específicos
#   de Linux (@emnapi/core, @emnapi/runtime) no quedan registrados en él.
#   npm ci exigiría lockfile perfecto cross-platform (no viable en dev Windows).
#   npm install resuelve correctamente los paquetes para la plataforma actual
#   respetando los rangos de versión de package.json.
#   --ignore-scripts: no ejecuta scripts de pre/postinstall de terceros.
RUN npm install --ignore-scripts

# Copiar el resto del código fuente una vez que node_modules ya está cacheado
COPY . .

# Compilar Angular en la configuración seleccionada.
# En 'production', el CLI de Angular usa environment.prod.ts donde
# apiUrl = '/api' → todas las peticiones HTTP van relativas al mismo origen,
# lo que permite que nginx haga el proxy transparentemente.
RUN npm run build -- --configuration ${BUILD_CONFIGURATION}


# =============================================================================
# Stage 2 · runtime
# nginx:alpine (~25 MB comprimido) sirve archivos estáticos y hace proxy.
# Sin Node.js, sin npm, sin código fuente, sin node_modules.
# Superficie de ataque mínima.
# =============================================================================
FROM nginx:alpine AS runtime

# Eliminar configuración de nginx por defecto para reemplazarla completamente
RUN rm /etc/nginx/conf.d/default.conf

# Copiar nuestra configuración de nginx
COPY nginx.conf /etc/nginx/nginx.conf

# Copiar el artefacto de Angular desde el stage builder.
# El builder '@angular/build:application' (Vite/ESBuild) genera los assets
# del browser en dist/<nombre-proyecto>/browser/ (no en dist/ directamente).
COPY --from=builder /app/dist/sagrada-familia-web/browser /usr/share/nginx/html

EXPOSE 80

# ENTRYPOINT en forma exec (JSON array) con 'daemon off':
#   - 'daemon off' mantiene nginx en primer plano (PID 1 en el contenedor).
#     Si nginx corriese en background, Docker detectaría que PID 1 terminó
#     y mataría el contenedor inmediatamente.
#   - Exec form (no shell form): SIGTERM de 'docker stop' llega directamente
#     a nginx → graceful shutdown: espera que las conexiones activas cierren
#     limpiamente antes de terminar.
ENTRYPOINT ["nginx", "-g", "daemon off;"]
