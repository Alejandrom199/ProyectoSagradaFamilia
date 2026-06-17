# =============================================================================
# Stage 1 · base
# Runtime puro de ASP.NET 8. Esta imagen es la base de la imagen final.
# No contiene SDK, compiladores ni código fuente — superficie mínima.
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

# Solo HTTP dentro de la red Docker. TLS termina en el reverse proxy (nginx).
# No se expone el puerto 443 para evitar gestión de certificados en contenedor.
EXPOSE 8080


# =============================================================================
# Stage 2 · build
# SDK completo: restore NuGet (capa cacheada) + compile + publish en un solo
# stage. Esta imagen NUNCA llega a Docker Hub ni a producción.
#
# ARG BUILD_CONFIGURATION:
#   - Existe únicamente en tiempo de construcción de la imagen.
#   - NO queda almacenado en las capas de la imagen final (a diferencia de ENV).
#   - Permite construir con `--build-arg BUILD_CONFIGURATION=Debug` en CI/CD.
#   - Valor por defecto: Release (optimización de producción).
#
# Por qué se colapsaron build y publish en un solo stage:
#   dotnet build --output <ruta> mueve los artefactos fuera de la ruta por
#   defecto (bin/<config>/<tfm>/). dotnet publish --no-build los busca
#   exactamente ahí, por lo que los dos stages separados causan MSB3030.
#   El patrón oficial de Microsoft es restore → publish (que compila y publica
#   en un solo paso), usando --no-restore para reutilizar la capa cacheada.
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# ── Trick de caché de capas ───────────────────────────────────────────────────
# Se copian primero solo los .csproj. Si el código fuente cambia pero los
# descriptores de proyecto no, Docker reutiliza la capa de 'dotnet restore'
# sin re-descargar todos los paquetes NuGet (ahorra minutos en cada build).
COPY SagradaFamilia.Domain/SagradaFamilia.Domain.csproj                   SagradaFamilia.Domain/
COPY SagradaFamilia.Application/SagradaFamilia.Application.csproj         SagradaFamilia.Application/
COPY SagradaFamilia.Infrastructure/SagradaFamilia.Infrastructure.csproj   SagradaFamilia.Infrastructure/
COPY SagradaFamilia.API/SagradaFamilia.API.csproj                         SagradaFamilia.API/

RUN dotnet restore "SagradaFamilia.API/SagradaFamilia.API.csproj"

# Copiar el código fuente una vez que el restore ya está cacheado.
COPY . .

# dotnet publish compila Y publica en un solo paso.
# --no-restore: reutiliza los paquetes ya restaurados en la capa anterior.
# /p:UseAppHost=false: no genera ejecutable nativo (.exe); en Linux siempre
#   se invoca como 'dotnet <dll>', el AppHost solo añade peso innecesario.
RUN dotnet publish "SagradaFamilia.API/SagradaFamilia.API.csproj" \
        --configuration ${BUILD_CONFIGURATION} \
        --no-restore \
        --output /app/publish \
        /p:UseAppHost=false


# =============================================================================
# Stage 3 · final  (la única imagen que llega a Docker Hub)
# Runtime + artefacto. Sin SDK, sin código fuente, sin caché de NuGet,
# sin credenciales de build. Imagen lista para push al registro.
# =============================================================================
FROM base AS final

WORKDIR /app

# --chown garantiza que el proceso sin privilegios pueda leer su propio artefacto
COPY --from=build --chown=app:app /app/publish .

# ── Variables de entorno de runtime ──────────────────────────────────────────
#
# REGLA CRÍTICA DE SEGURIDAD — leer antes de modificar:
#
#   ENV se almacena en el manifiesto de la imagen. Cualquier valor que escribas
#   aquí es visible con 'docker inspect <imagen>' y en Docker Hub.
#   Por eso las credenciales (contraseñas, claves secretas) se dejan en blanco
#   y DEBEN inyectarse desde docker-compose, un orquestador o un vault.
#   Nunca hardcodear contraseñas aquí.
#
#   ASP.NET Core lee variables de entorno automáticamente. El separador '__'
#   (doble guion bajo) mapea al ':' de appsettings.json:
#     ConnectionStrings__DefaultConnection → ConnectionStrings:DefaultConnection
#     JwtSettings__SecretKey              → JwtSettings:SecretKey
#
ENV \
    # ── Runtime de ASP.NET ────────────────────────────────────────────────────
    # Production: desactiva Swagger, activa logging de errores estructurado, etc.
    ASPNETCORE_ENVIRONMENT=Production \
    # Railway inyecta HTTP_PORTS con el puerto que le asigna al contenedor (8080).
    # No sobreescribimos ASPNETCORE_URLS para que .NET use ese puerto automáticamente.
    # Lee los headers X-Forwarded-Proto / X-Forwarded-For enviados por el proxy.
    ASPNETCORE_FORWARDEDHEADERS_ENABLED=true \
    \
    # ── Base de datos — INYECTAR en despliegue ────────────────────────────────
    # Inyectar desde Render/docker-compose. Nunca hardcodear aquí.
    ConnectionStrings__PostgresConnection="" \
    \
    # ── JWT — INYECTAR en despliegue ──────────────────────────────────────────
    # SecretKey debe tener mínimo 256 bits (32 caracteres) para HS256.
    # Generarlo con: openssl rand -base64 32
    JwtSettings__SecretKey="" \
    JwtSettings__Issuer="SagradaFamiliaAPI" \
    JwtSettings__Audience="SagradaFamiliaClient" \
    JwtSettings__ExpirationMinutes="60" \
    JwtSettings__RefreshTokenExpirationDays="7" \
    \
    # ── Microservicio de predicción ───────────────────────────────────────────
    # 'predict' es el nombre del servicio en docker-compose.
    # Docker resuelve este nombre vía su DNS interno.
    ProphetApi__BaseUrl="http://predict:8000" \
    \
    # ── CORS — URL del frontend (Netlify en prod) — INYECTAR en despliegue ────
    AppSettings__FrontendUrl="" \
    \
    # ── SMTP — INYECTAR en despliegue ─────────────────────────────────────────
    SmtpSettings__Host="smtp.gmail.com" \
    SmtpSettings__Port="587" \
    SmtpSettings__EnableSsl="true" \
    SmtpSettings__FromName="Sagrada Familia" \
    # Las tres siguientes son secretos: dejar vacías aquí, inyectar en runtime
    SmtpSettings__FromEmail="" \
    SmtpSettings__Username="" \
    SmtpSettings__Password=""

# ── Usuario sin privilegios ───────────────────────────────────────────────────
# La imagen aspnet:8.0 incluye el usuario 'app' (UID 1654).
# Correr como non-root limita el daño en caso de vulnerabilidad en la app.
# Se declara DESPUÉS del COPY para que la instrucción COPY (que siempre corre
# como root internamente) pueda escribir los archivos con --chown=app:app.
USER app

# ── ENTRYPOINT en forma exec (JSON array) ─────────────────────────────────────
# Por qué exec form y no shell form:
#   Shell form  → ENTRYPOINT dotnet SagradaFamilia.API.dll
#                 Docker invoca: /bin/sh -c "dotnet ..."
#                 El PID 1 es /bin/sh, NO el proceso .NET.
#                 Las señales SIGTERM (de 'docker stop') van al shell y se pierden.
#                 El contenedor espera el timeout completo antes de matar el proceso.
#
#   Exec form   → ENTRYPOINT ["dotnet", "SagradaFamilia.API.dll"]
#                 El proceso .NET es directamente PID 1.
#                 SIGTERM llega al runtime de .NET → IHostedService.StopAsync() →
#                 cierre limpio de conexiones a BD, flush de logs, etc.
ENTRYPOINT ["dotnet", "SagradaFamilia.API.dll"]
