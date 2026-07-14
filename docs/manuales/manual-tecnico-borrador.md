# Manual Técnico — Sistema Sagrada Familia
### Borrador de contenido (Fase 1 del plan) — sin maquetar todavía

> Este es un borrador en texto plano para que confirmes el enfoque y la profundidad antes de pasarlo a Word. No incluye portada, índice ni numeración — eso se arma en la Fase 3 con `python-docx`, una vez aprobado el contenido.

---

## 1. Introducción y objetivo del sistema

Sagrada Familia es un sistema de información clínica orientado al seguimiento nutricional y médico de pacientes pediátricos. Permite a un centro de salud registrar niños (pacientes), vincularlos con un representante legal (padre/tutor) y un médico responsable, y llevar el control de:

- Medidas antropométricas (peso, talla) y su evolución en el tiempo.
- Citas médicas y consultas clínicas.
- Prescripciones derivadas de cada consulta.
- Predicciones de crecimiento mediante un modelo de machine learning.
- Orientación alimentaria según la edad del niño.

El sistema opera bajo tres roles con permisos y vistas diferenciadas: **Administrador** (gestión de cuentas, roles, configuración), **Médico** (operación clínica completa) y **Padre** (consulta de la información de sus propios hijos).

---

## 2. Arquitectura general

El sistema se compone de tres servicios independientes, cada uno en su propio contenedor Docker:

| Servicio | Tecnología | Responsabilidad |
|---|---|---|
| `api` | .NET 8 (C#) | Lógica de negocio, persistencia, autenticación, API REST |
| `predict` | Python + Prophet | Microservicio de predicción de crecimiento, consumido por `api` vía HTTP |
| `web` | Angular 21 + nginx | Interfaz de usuario (SPA), servida como estáticos con nginx haciendo de proxy inverso hacia `api` |

La base de datos es **PostgreSQL**, gestionada como servicio externo en **Supabase** (no se levanta un contenedor de base de datos en producción; en desarrollo local sí se usa un contenedor Postgres aparte).

```
Navegador ──▶ nginx (web) ──┬──▶ archivos estáticos Angular
                             └──▶ /api/* ──▶ api (.NET 8) ──┬──▶ PostgreSQL (Supabase)
                                                              └──▶ predict (Python/Prophet)
```

El backend sigue **Clean Architecture** en 4 capas, y el frontend es una **Single Page Application Angular** con componentes standalone (sin NgModules).

---

## 3. Backend — Clean Architecture

| Capa | Contenido | Responsabilidad |
|---|---|---|
| `SagradaFamilia.Domain` | Entidades, Enums, interfaces de repositorios, excepciones | Núcleo del negocio, sin dependencias externas |
| `SagradaFamilia.Application` | Servicios de aplicación, DTOs, validadores (FluentValidation), mapeos (AutoMapper), reportes | Casos de uso / lógica de negocio |
| `SagradaFamilia.Infrastructure` | Entity Framework Core, repositorios, seeders, envío de correo, integración con el microservicio de predicción, logging | Implementación técnica de los contratos definidos en Domain |
| `SagradaFamilia.API` | Controladores, middlewares, extensiones de configuración, `Program.cs` | Punto de entrada HTTP, exposición de la API REST |

La regla de dependencia es estricta: `API → Application → Domain`, con `Infrastructure` implementando interfaces definidas en `Domain` e inyectada en tiempo de ejecución (Dependency Injection). Ninguna capa interna depende de una externa.

### 3.1 Controladores de la API (18)

`AuthController`, `UsuariosController`, `MedicoController`, `PadreController`, `NinosController`, `CitasController`, `ConsultasController`, `PrescripcionesController`, `MedidasController`, `PrediccionesController`, `AlimentosController`, `ParametrosController`, `PlantillasController`, `EventosCorreoController`, `MenuController`, `SistemaController`, `ReportesController`, y `BaseController` (clase base abstracta de la que heredan los demás, con utilidades comunes).

### 3.2 Autenticación

El sistema usa **JWT almacenado en cookies `HttpOnly`** (no en `localStorage`, para mitigar XSS):
- `access_token`: token de corta duración, usado en cada petición.
- `refresh_token`: token de larga duración (7 días por defecto), usado exclusivamente para renovar el `access_token` mediante un endpoint dedicado (`/api/auth/refresh`).
- En producción, las cookies usan `SameSite=None; Secure`; en desarrollo, `SameSite=Strict`.

La lógica vive en `AuthService` (Application) + `TokenService` (Infrastructure).

---

## 4. Modelo de dominio

El sistema tiene **29 entidades**. Las relaciones centrales giran en torno a la entidad `Nino` (paciente):

```
Padre (1) ──── (N) Nino (N) ──── (1) Medico
                    │
        ┌───────────┼───────────┬─────────────┐
        ▼           ▼           ▼             ▼
     Medida        Cita     Prediccion   Prescripcion
                     │
                     ▼
                 Consulta ──── Prescripcion
```

- Un **Padre** puede tener varios **Nino** (hijos); cada **Nino** tiene un único **Padre** y un único **Medico** responsable.
- Cada **Nino** acumula un historial de **Medida** (peso/talla en el tiempo), **Cita** (agenda médica), **Prediccion** (crecimiento estimado) y **Prescripcion** (indicaciones médicas).
- Una **Cita** puede derivar en una **Consulta**, de la cual sale una o varias **Prescripcion**.

Otras entidades de soporte: `Usuario`/`Rol`/`RolPermiso`/`UsuarioPermiso`/`Modulo`/`Opcion`/`OpcionAccion`/`Accion` (control de acceso), `Alimento`/`CategoriaAlimento`/`OmsReferencia` (catálogo nutricional y curvas de referencia OMS), `ParametroSistema` (configuración), `PlantillaCorreo`/`EventoCorreo` (notificaciones), `Auditoria`/`LogSistema` (trazabilidad), `RefreshToken`/`PasswordResetToken` (sesión y recuperación de clave), `Medicamento` (catálogo de medicamentos para prescripciones), `ReporteGenerado` (trazabilidad de reportes PDF emitidos: UID correlativo `HC-AAAA-NNNNNN`/`PR-AAAA-NNNNNN`, tipo, niño, usuario generador y fecha), `UsuarioEstadoHistorial` (historial acumulativo de cada activación/desactivación de cuenta, con motivo obligatorio y quién lo hizo).

La entidad `Medico` además guarda `FirmaImagen` (imagen PNG en base64 de la firma manuscrita capturada en el perfil) y `FirmaActualizadaEn`, que se incrusta automáticamente al final del reporte PDF de Historia Clínica cuando está disponible.

---

## 5. Frontend — Angular 21

Estructura de carpetas (`sagrada-familia-web/src/app/`):

```
core/        → guards, interceptors, servicios de datos (uno por entidad/dominio)
shared/      → componentes reutilizables, interfaces, pipes, directivas, utils
pages/
  auth/      → login, activar cuenta, recuperar clave
  dashboard/ → panel principal (Admin/Médico)
  features/  → un folder por módulo de negocio (pacientes, citas, padres, etc.)
environments/
```

### 5.1 Componentes compartidos reutilizables

`searchable-select` (combo con búsqueda, reemplaza los `<select>` nativos en todo el sistema), `datatable` (tabla server-side con paginación/orden/filtros/exportación), `avatar` (iniciales + color por sexo/rol), `admin-layout` + `header` + `sidebar` (layout autenticado, con reloj de servidor y badge de versión de build), `build-badge` (muestra el SHA del commit desplegado), `breadcrumb`, `confirm-modal`, `import-modal`, `status-badge`, `growth-chart` / `prediction-chart` (gráficas clínicas con ApexCharts), `explorador-visual`, `role-redirect` (redirección post-login según rol), `firma-pad` (lienzo `<canvas>` para capturar la firma manuscrita del médico con mouse o touch, en su perfil), `desactivar-cuenta-modal` (exige un motivo de mínimo 10 caracteres antes de confirmar la desactivación de una cuenta).

### 5.2 Control de acceso en el frontend

6 guards en `core/guards/`: `authGuard` (exige sesión), `adminGuard`, `medicoGuard`, `padreGuard`, `noPadreGuard`, `roleGuard` (Admin o Médico). El menú lateral se arma dinámicamente consultando `/api/menu`, que devuelve solo las opciones permitidas según el rol (tabla `RolPermiso`).

---

## 6. Despliegue

**Mecanismo vigente: Docker self-hosted.**

1. Al hacer push/merge a la rama `main`, el workflow `.github/workflows/docker-publish.yml` (GitHub Actions) construye y publica **3 imágenes Docker** (`sagrada-familia-api`, `sagrada-familia-predict`, `sagrada-familia-web`) a Docker Hub, etiquetadas `latest` y `sha-<commit-corto>`.
2. En el servidor de producción, `docker-compose.prod.yml` levanta los 3 contenedores a partir de esas imágenes ya publicadas (no se compila nada en el servidor).
3. La base de datos **no** corre en un contenedor propio: es una instancia de **PostgreSQL gestionada por Supabase**, referenciada por variable de entorno (`ConnectionStrings__PostgresConnection`).

> Nota para el manual: el repositorio conserva archivos de intentos de despliegue anteriores (`render.yaml` ya no existe pero se mencionó en commits, `netlify.toml`, `setup-railway.sh`) correspondientes a una iteración de ~3 días en junio que fue descartada. **No deben documentarse como mecanismo vigente** — el pipeline activo y mantenido es el de Docker Hub descrito arriba.

---

## 7. Guía de instalación / entorno local

Variables requeridas en `.env` (ver `.env.example` en la raíz):

| Variable | Uso |
|---|---|
| `POSTGRES_PASSWORD` | Contraseña del Postgres local (contenedor de desarrollo) |
| `JWT_SECRET_KEY` | Clave de firma de los JWT (mínimo 32 caracteres) |
| `DOCKERHUB_USER` | Solo necesario para levantar `docker-compose.prod.yml` con imágenes precompiladas |
| `SMTP_FROM_EMAIL` / `SMTP_USERNAME` / `SMTP_PASSWORD` | Envío de correo (activación de cuenta, recuperación de clave) vía Gmail App Password |

En desarrollo local se usa `docker-compose.yml` (raíz del repo), que sí levanta un contenedor Postgres propio, a diferencia de producción.

---

## 8. Anexos (pendiente de definir alcance)

Posibles anexos a incluir — decisión pendiente contigo:
- Diccionario completo de datos (las 27 entidades con todos sus campos).
- Listado completo de endpoints de la API con verbos HTTP.
- Diagrama entidad-relación completo (requeriría generarlo, no lo tengo hecho).

---

## Preguntas para tu revisión antes de pasar a Word

1. ¿La profundidad de las secciones 3-6 es la que necesitas, o quieres más/menos detalle técnico (ej. listar los ~18 controladores con sus endpoints)?
2. ¿Incluyo el diccionario de datos completo (27 entidades con campos) como anexo, o alcanza con el diagrama de relaciones central que ya está en la sección 4?
3. ¿Quieres un diagrama de arquitectura como imagen (lo generaría yo) o el bloque de texto tipo ASCII de la sección 2 es suficiente?
