# Requerimientos Funcionales y No Funcionales — Sistema Sagrada Familia

> Documento derivado de:
> - `docs/CAPITULO 1 - FINAL.docx` → sección **"Metodología de Desarrollo del Proyecto"** (metodología incremental, 4 fases por incremento, tabla de incrementos y justificación de orden).
> - `diagrams/flows_md/01` a `16` → flujos reales implementados en el código (backend .NET 8, frontend Angular, microservicio Python/FastAPI).
>
> Cada requerimiento indica primero **a qué incremento pertenece**, para que puedas ubicarlo directamente en el Capítulo III (Análisis de cada incremento) o en la matriz de trazabilidad que necesites armar.

---

## 1. Recordatorio de los 4 incrementos (Capítulo 1)

| Incremento | Módulo (oficial, Cap. 1) | Módulos técnicos reales que se ubican aquí (`flows_md`) |
|---|---|---|
| **Incremento 1** | Módulo de usuarios | 01-Autenticación y Sesión, 02-Roles y Permisos, 03-Usuarios, 04-Médicos, 05-Padres/Representantes, 06-Niños/Pacientes, 07-Citas Médicas, 08-Consultas y Prescripciones |
| **Incremento 2** | Módulo de gestión de crecimiento | 09-Medidas Antropométricas |
| **Incremento 3** | Módulo de análisis predictivo (Machine Learning) | 10-Predicciones de Crecimiento |
| **Incremento 4** | Módulo de orientación alimentaria | 11-Catálogo de Alimentos |
| **Transversal** *(no es un incremento formal del Cap. 1, pero agrupa funcionalidad que atraviesa a los 4)* | — | 12-Auditoría del Sistema, 13-Logs del Sistema, 14-Plantillas de Correo y Notificaciones, 15-Parámetros del Sistema, 16-Reportes y Exportación |

**Criterio de ubicación usado:**
- **Citas Médicas** y **Consultas/Prescripciones** dependen directamente de Usuario + Niño + Médico ya existentes → se ubicaron en el **Incremento 1**, como parte del núcleo clínico-administrativo base.
- **Auditoría, Logs, Plantillas de Correo y Parámetros del Sistema** no pertenecen a un incremento de negocio puntual: operan sobre *todas* las entidades del sistema (auditoría/logs) o son consumidos por más de un incremento (parámetros de horario → citas; plantillas → todos los correos) → se agrupan como **Transversal**.
- **Reportes/Exportación** no es una entidad propia: se reparte según la entidad que exporta cada reporte (ver RF-TR-09).

---

## 2. Nomenclatura

- **RF-I1-XX** / **RF-I2-XX** / **RF-I3-XX** / **RF-I4-XX** → Requerimiento Funcional del Incremento 1/2/3/4.
- **RF-TR-XX** → Requerimiento Funcional Transversal (no exclusivo de un incremento).
- **RNF-XX** → Requerimiento No Funcional, aplicable a todo el sistema (no se repite por incremento, salvo excepción marcada explícitamente).
- Granularidad: un RF por **capacidad de negocio**, no por cada endpoint CRUD individual (ej. "gestionar médicos" agrupa crear/editar/eliminar/consultar, no son 4 RF separados).

---

## 3. Requerimientos Funcionales

### Incremento 1 — Módulo de Usuarios (Autenticación, Roles, Usuarios, Médicos, Padres, Niños, Citas, Consultas)

| Código | Requerimiento |
|---|---|
| RF-I1-01 | El sistema debe permitir el inicio de sesión mediante un formulario único de email y contraseña para los tres roles (Administrador, Médico, Padre), emitiendo un token de acceso (JWT) y un refresh token almacenados en cookies HttpOnly. |
| RF-I1-02 | El sistema debe renovar automáticamente la sesión del usuario (renovación silenciosa del access token) mediante rotación de un solo uso del refresh token, sin intervención del usuario, mientras el refresh token no esté revocado ni expirado. |
| RF-I1-03 | El sistema debe permitir el cierre de sesión (logout), eliminando las cookies de sesión del navegador. |
| RF-I1-04 | El sistema debe permitir al rol Médico solicitar la recuperación de su contraseña de forma autoservicio ("olvidé mi contraseña"), respondiendo siempre con un mensaje neutro que no revele si el correo existe o no en el sistema. |
| RF-I1-05 | El sistema debe permitir que un Administrador (para Médico o Padre) o un Médico (para Padre) dispare el restablecimiento de la contraseña de otro usuario, invalidando cualquier enlace de restablecimiento anterior no utilizado. |
| RF-I1-06 | El sistema debe permitir la activación de cuenta y definición de la contraseña inicial mediante un enlace de un solo uso enviado por correo, como paso final del alta de cualquier Administrador, Médico o Padre. |
| RF-I1-07 | El sistema debe permitir al Administrador configurar, para los roles Médico y Padre, qué módulos/opciones/acciones puede ver y usar cada rol, protegiendo al rol Administrador de ser modificado. |
| RF-I1-08 | El sistema debe resolver dinámicamente el menú de navegación y los botones de acción (ver/crear/editar/eliminar) de cada usuario según los permisos vigentes de su rol. |
| RF-I1-09 | El sistema debe permitir al Administrador gestionar las cuentas de usuario del sistema (crear cuentas de Administrador, listar de forma unificada cuentas de Administrador/Médico/Padre, activar/desactivar cuentas), impidiendo que un administrador se autodesactive o que el sistema quede sin administradores activos. |
| RF-I1-10 | El sistema debe permitir el alta masiva de usuarios (Médico/Padre) mediante la carga de un archivo Excel, procesando el archivo fila por fila, actualizando registros existentes y reportando errores por fila sin detener el resto del proceso. |
| RF-I1-11 | El sistema debe permitir al Administrador gestionar médicos (registrar, editar datos profesionales, dar de baja, restablecer contraseña, alta masiva, consultar y exportar), bloqueando la baja de un médico que tenga padres o niños activos asignados. |
| RF-I1-12 | El sistema debe permitir a Médico y Administrador gestionar padres/representantes (registrar con asignación de médico responsable, editar, cambiar el correo de acceso con confirmación explícita, dar de baja, alta masiva, consultar y exportar), reservando al Administrador la reasignación del médico responsable de un padre y bloqueando la baja de un padre con niños activos asignados. |
| RF-I1-13 | El sistema debe proveer al rol Padre un portal de autogestión de solo lectura sobre sus propios hijos, citas y prescripciones, validando en cada consulta que el recurso solicitado le pertenezca. |
| RF-I1-14 | El sistema debe permitir al Médico registrar y gestionar el perfil del niño/paciente pediátrico (alta, edición, baja), reservando al Administrador la reasignación del médico responsable y al Médico la reasignación del representante (padre), y cancelando automáticamente las citas pendientes del niño al darlo de baja. |
| RF-I1-15 | El sistema debe permitir la consulta de niños/pacientes con alcance diferenciado por rol: el Médico ve sus propios pacientes, el Administrador ve el listado completo, y el Padre ve únicamente a sus propios hijos. |
| RF-I1-16 | El sistema debe permitir a Médico y Administrador agendar citas médicas, validando que el horario esté dentro del rango de atención y días hábiles configurados y que no se solape con otra cita del mismo médico, notificando por correo al padre con una invitación de calendario adjunta. |
| RF-I1-17 | El sistema debe permitir reagendar una cita médica existente (en estado Pendiente, Cancelada o No Asistió), regresándola automáticamente al estado Pendiente cuando provenía de Cancelada o No Asistió, y notificando el cambio al padre por correo. |
| RF-I1-18 | El sistema debe permitir cambiar el estado de una cita a Cancelada (en cualquier momento) o a No Asistió (solo una vez transcurrida su fecha/hora programada), registrando opcionalmente el motivo de cancelación. |
| RF-I1-19 | El sistema debe permitir al Médico iniciar una consulta médica desde una cita en estado Pendiente, generando la consulta asociada y cambiando el estado de la cita a En Curso. |
| RF-I1-20 | El sistema debe permitir la consulta de citas médicas según el rol y la vista solicitada (agenda del día, próximas, historial del médico, citas por paciente, citas de los propios hijos para el Padre), validando la pertenencia del recurso cuando el solicitante es Padre. |
| RF-I1-21 | El sistema debe permitir al Médico registrar y editar el motivo, diagnóstico, indicaciones y evolución de una consulta médica mientras esta se encuentre en estado En Curso. |
| RF-I1-22 | El sistema debe permitir al Médico emitir y editar prescripciones médicas dentro de una consulta en curso, exigiendo al menos un medicamento con nombre, dosis, frecuencia y vía de administración obligatorios. |
| RF-I1-23 | El sistema debe permitir al Médico completar una consulta médica, sincronizando automáticamente el estado de la cita asociada a Completada e impidiendo ediciones posteriores de diagnóstico o prescripciones sobre esa consulta. |
| RF-I1-24 | El sistema debe permitir la consulta del historial de prescripciones con presentación diferenciada por rol: información clínica completa para el Médico, y lenguaje simple (solo medicamentos e indicaciones) para el Padre. |

### Incremento 2 — Módulo de Gestión de Crecimiento (Medidas Antropométricas)

| Código | Requerimiento |
|---|---|
| RF-I2-01 | El sistema debe permitir al Médico registrar una medida antropométrica (peso y talla) de un niño, validando que la fecha no sea futura, que los valores estén dentro de rangos pediátricos válidos, y que no exista ya una medida registrada para ese niño en el mismo mes calendario. |
| RF-I2-02 | El sistema debe calcular, a demanda y sin persistirlo, el percentil de peso y de talla del niño según su sexo y edad en meses, contrastando contra la tabla de referencia de la Organización Mundial de la Salud (OMS). |
| RF-I2-03 | El sistema debe determinar automáticamente el estado nutricional del niño (Bajo peso severo, Bajo peso, Normal, Sobrepeso, Obesidad) a partir del percentil de peso calculado. |
| RF-I2-04 | El sistema debe representar gráficamente la evolución del peso y la talla del niño en el tiempo mediante una curva de crecimiento con dos ejes verticales independientes. |
| RF-I2-05 | El sistema debe presentar las medidas antropométricas con dos niveles de detalle según el rol: vista clínica completa con percentiles y estado nutricional técnico para el Médico, y vista amigable con mensaje y recomendación en lenguaje simple para el Padre, sin tecnicismos. |
| RF-I2-06 | El sistema debe permitir al Médico editar y eliminar (baja lógica) una medida antropométrica previamente registrada, reaplicando las validaciones de fecha y rango. |
| RF-I2-07 | El sistema debe actualizar automáticamente el valor real de una predicción previamente generada cuando se registra una nueva medida cuyo mes coincide con la fecha objetivo de dicha predicción. |
| RF-I2-08 | El sistema debe permitir el alta masiva de medidas mediante archivo Excel y la exportación del listado de medidas a Excel real y PDF. |

### Incremento 3 — Módulo de Análisis Predictivo (Machine Learning)

| Código | Requerimiento |
|---|---|
| RF-I3-01 | El sistema debe permitir al Médico generar, a demanda, una predicción del peso de un niño a 3, 6 y 12 meses, mediante un modelo de series temporales con crecimiento logístico acotado (Prophet). |
| RF-I3-02 | El sistema debe validar que el niño cuente con un mínimo de tres medidas antropométricas históricas registradas antes de intentar generar una predicción, tanto en el backend .NET como en el microservicio de predicción. |
| RF-I3-03 | El sistema debe calcular los límites fisiológicos superior e inferior (percentiles 97 y 3 de la tabla OMS correspondiente a la edad proyectada) que acotan el modelo predictivo, utilizando valores de respaldo fijos cuando la referencia no esté disponible. |
| RF-I3-04 | El sistema debe persistir cada predicción generada (valor predicho, valor mínimo, valor máximo, fecha objetivo, proyección en meses), actualizando el registro si ya existía una predicción previa para el mismo niño y fecha objetivo. |
| RF-I3-05 | El sistema debe permitir comparar visualmente el valor predicho contra el valor real medido posteriormente, cuando este último se encuentre disponible. |
| RF-I3-06 | El sistema debe mostrar al Médico un indicador del estado de disponibilidad (en línea / fuera de línea) del microservicio de predicción, consultado de forma independiente a la generación de predicciones. |
| RF-I3-07 | El sistema debe degradarse de forma controlada (mensaje explicativo, sin afectar el resto del sistema) cuando el microservicio de predicción no esté disponible o responda con error. |

### Incremento 4 — Módulo de Orientación Alimentaria

| Código | Requerimiento |
|---|---|
| RF-I4-01 | El sistema debe permitir al Médico gestionar el catálogo de alimentos (crear, editar, dar de baja) y sus categorías. |
| RF-I4-02 | El sistema debe permitir consultar los alimentos recomendados filtrados automáticamente según la edad del niño, expresada en meses. |
| RF-I4-03 | El sistema debe presentar una vista diferenciada por rol para el catálogo de alimentos: administración completa para el Médico, y exploración visual sin funciones de edición para el Padre. |
| RF-I4-04 | El sistema debe permitir listar el catálogo de alimentos de forma paginada, con filtros por categoría, estado (activo/inactivo) y rango de edades. |
| RF-I4-05 | El sistema debe permitir el alta masiva de alimentos mediante archivo Excel y la exportación del catálogo a Excel real y PDF. |

### Requerimientos Transversales (aplican a más de un incremento)

| Código | Requerimiento |
|---|---|
| RF-TR-01 | El sistema debe registrar automáticamente en un módulo de auditoría toda creación, actualización y eliminación lógica de datos ocurrida en cualquier entidad del sistema, excluyendo entidades técnicas o sensibles (Auditoría, Logs, Refresh Token, Token de restablecimiento, Referencias OMS). |
| RF-TR-02 | El sistema debe permitir consultar el historial de auditoría con alcance diferenciado por rol: alcance total (todas las entidades y usuarios) para el Administrador, y alcance restringido a la propia actividad para el Médico. |
| RF-TR-03 | El sistema debe registrar eventos técnicos (logs) asociados a acciones de autenticación (inicio de sesión exitoso de Médico/Administrador, restablecimiento de contraseña, activación de cuenta, solicitud de reset), consultables exclusivamente por el Administrador. |
| RF-TR-04 | El sistema debe permitir al Administrador gestionar plantillas de correo (crear, editar asunto y cuerpo HTML con variables, activar/desactivar, eliminar), bloqueando la eliminación de una plantilla mientras esté asignada a un evento de correo activo. |
| RF-TR-05 | El sistema debe permitir al Administrador asignar qué plantilla de correo utiliza cada uno de los eventos fijos del sistema (creación de cuenta de Padre/Médico/Administrador, cambio de clave, cita agendada, cita reagendada). |
| RF-TR-06 | El sistema debe enviar automáticamente notificaciones por correo electrónico ante los eventos configurados (altas de cuenta, cambios de clave, citas agendadas/reagendadas), adjuntando una invitación de calendario en el caso de citas. |
| RF-TR-07 | El sistema debe permitir al Administrador gestionar parámetros configurables del sistema (crear, editar, eliminar) organizados por grupo y código, con lectura de un parámetro puntual abierta a cualquier rol autenticado. |
| RF-TR-08 | El sistema debe aplicar el parámetro configurable de horario de atención (hora de inicio, hora de fin, días hábiles) como regla de negocio al validar el agendamiento de citas médicas. |
| RF-TR-09 | El sistema debe permitir exportar a PDF y a Excel real las principales entidades del sistema (usuarios, médicos, padres, niños/pacientes, medidas, citas, prescripciones, alimentos, auditoría, logs), respetando el mismo alcance de datos que corresponde al rol del usuario que exporta. |
| RF-TR-10 | El sistema debe ofrecer, como mecanismo reutilizable, el alta masiva mediante archivo Excel para las entidades que lo soportan (usuarios, médicos, padres, niños, alimentos, medidas), procesando cada archivo fila por fila y reportando errores sin interrumpir el proceso completo. |

---

## 4. Requerimientos No Funcionales

*(Lista única aplicable a todo el sistema, ya que la mayoría de estos atributos de calidad — JWT, Clean Architecture, Swagger, exportación — son transversales a los 4 incrementos y no exclusivos de uno).*

| Código | Categoría | Requerimiento |
|---|---|---|
| RNF-01 | Seguridad | El sistema debe basar la autenticación en JSON Web Tokens (JWT) almacenados en cookies HttpOnly, sin exponer los tokens en el cuerpo de las respuestas ni en almacenamiento accesible desde JavaScript. |
| RNF-02 | Seguridad | El sistema debe almacenar las contraseñas mediante una función de hash (BCrypt), sin persistirlas en texto plano ni en un formato reversible. |
| RNF-03 | Seguridad | El sistema debe restringir el acceso a cada endpoint de la API según el rol del usuario autenticado, de forma independiente al sistema de menú/permisos visual. |
| RNF-04 | Seguridad | El sistema debe generar enlaces de un solo uso con expiración definida (24 o 48 horas, según el caso) para la activación de cuenta y el restablecimiento de contraseña, invalidando los enlaces anteriores no utilizados al generar uno nuevo. |
| RNF-05 | Seguridad | El sistema debe responder con mensajes que no permitan inferir la existencia de una cuenta (anti-enumeración) en los flujos de autenticación y recuperación de contraseña. |
| RNF-06 | Seguridad | El sistema debe validar, en cada endpoint de lectura accesible por el rol Padre, que el recurso solicitado (niño, cita, prescripción, medida) pertenezca efectivamente a ese padre, antes de responder con los datos. |
| RNF-07 | Rendimiento | El sistema debe limitar a 30 segundos el tiempo de espera de la comunicación entre el backend .NET y el microservicio de predicción, evitando bloqueos prolongados de la interfaz. |
| RNF-08 | Disponibilidad | El sistema debe degradarse de forma controlada ante la indisponibilidad del microservicio de predicción, sin afectar el funcionamiento del resto de los módulos. |
| RNF-09 | Confiabilidad | El sistema debe ejecutar dentro de transacciones atómicas las operaciones que afectan múltiples entidades relacionadas (alta de cuentas con su perfil asociado, baja de un niño con cancelación de citas, inicio/finalización de una consulta), revirtiendo todos los cambios ante cualquier fallo parcial. |
| RNF-10 | Confiabilidad | El sistema debe preservar el historial de información clínica y administrativa mediante baja lógica (soft delete) en lugar de eliminación física, en las entidades relevantes del dominio. |
| RNF-11 | Mantenibilidad | El backend debe estar organizado en capas (dominio, aplicación, infraestructura y API) siguiendo el enfoque de Clean Architecture sobre .NET 8, para facilitar la incorporación de nuevos incrementos. |
| RNF-12 | Mantenibilidad | El microservicio de predicción debe integrarse al backend .NET mediante un cliente HTTP desacoplado, de modo que la tecnología del modelo predictivo pueda evolucionar sin modificar el backend principal. |
| RNF-13 | Verificabilidad / Usabilidad | El sistema debe exponer documentación interactiva de sus endpoints (Swagger en el backend .NET, documentación automática de FastAPI en el microservicio de predicción), utilizable como mecanismo de verificación funcional manual. |
| RNF-14 | Usabilidad | El sistema debe adaptar el nivel de tecnicismo de la información mostrada según el rol del usuario (ej. percentiles y estado nutricional técnico para el Médico frente a mensajes en lenguaje simple para el Padre). |
| RNF-15 | Compatibilidad | El sistema debe generar los archivos de exportación en formatos estándar reales (.xlsx mediante ClosedXML, .pdf mediante QuestPDF), evitando formatos propietarios o CSV disfrazado de Excel. |
| RNF-16 | Configurabilidad / Extensibilidad | El sistema debe permitir ajustar reglas de negocio (horario de atención de citas) y el contenido de las notificaciones automáticas (plantillas de correo) mediante configuración, sin requerir modificación del código fuente. |
| RNF-17 | Rendimiento | El sistema debe implementar paginación del lado del servidor en los listados principales (usuarios, médicos, padres, niños, medidas, citas, prescripciones, auditoría, alimentos), evitando cargar volúmenes completos de datos en el cliente. |

---

## 5. Notas para el Capítulo III (trazabilidad)

- Cada RF de esta lista corresponde a un flujo (o grupo de flujos) documentado en `diagrams/flows_md/`. Si necesitas la matriz de trazabilidad RF ↔ Caso de prueba, los casos de prueba (CP-01 a CP-04/05 por incremento) ya están definidos en la sección "Aplicación de la metodología incremental" del Capítulo 1 y pueden enlazarse 1:1 con los RF de este documento.
- Los requerimientos **Transversales (RF-TR)** no tienen tabla de casos de prueba propia en el Capítulo 1 — si el tribunal exige que todo RF tenga su CP, conviene sumarlos como una quinta tabla de pruebas ("Módulo transversal: auditoría, notificaciones y parámetros") o repartirlos dentro de las pruebas del Incremento 1 (que es donde se originan la mayoría de los correos automáticos).
