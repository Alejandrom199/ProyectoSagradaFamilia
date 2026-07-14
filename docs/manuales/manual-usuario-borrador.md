# Manual de Usuario — Sistema Sagrada Familia
### Borrador de contenido (Fase 2 del plan) — sin maquetar todavía

> Este es un borrador en texto plano, igual que se hizo con el Manual Técnico. No incluye portada, índice ni numeración de página — eso se arma en la fase final con `python-docx`. Los lugares donde va una captura de pantalla real están marcados como `[AQUÍ VA FOTO: ...]`; se reemplazan recién cuando apruebes el contenido y tengamos credenciales de prueba para navegar cada pantalla.
>
> Redactado en lenguaje simple, pensado para personal del centro de salud sin conocimientos de informática (Administrador, Médico, Padre/Representante). Se evitan palabras técnicas de programación; los términos clínicos (percentil, IMC, etc.) sí se mantienen en la sección del Médico porque son parte de su vocabulario normal de trabajo.

---

## 0. Introducción

**Sagrada Familia** es el sistema donde el centro de salud lleva el control del crecimiento y la atención médica de los niños que atiende. Permite registrar el peso y la talla de cada niño a lo largo del tiempo, agendar y llevar el historial de sus citas médicas, guardar las recetas que el médico indica, ver hacia dónde va el crecimiento del niño en los próximos meses, y recomendar alimentos según su edad.

El sistema tiene tres tipos de usuario, y cada uno ve una pantalla distinta al ingresar:

| Usuario | Para qué usa el sistema |
|---|---|
| **Administrador** | Da de alta y gestiona las cuentas de acceso (médicos, representantes, otros administradores), configura qué puede ver cada rol, define el horario de atención y los correos automáticos, y revisa la actividad del sistema. **No ve información clínica de los niños** (eso es exclusivo del Médico). |
| **Médico** | Es quien usa el sistema en el día a día: registra pacientes, mide su peso y talla, agenda y atiende sus citas, emite recetas, revisa hacia dónde proyecta ir el crecimiento del niño, y recomienda alimentos. |
| **Padre / Representante** | Solo consulta la información de sus propios hijos: su progreso de crecimiento, sus citas, sus recetas y qué alimentos se le recomiendan. No puede editar nada — es una vista de solo lectura pensada para que cualquier familia la entienda sin necesidad de conocimientos médicos. |

Cada sección de este manual está organizada por rol. Si tu usuario es de Médico, puedes saltar directamente a la sección 3; si eres Administrador, a la sección 2; si eres Padre/Representante, a la sección 4. La sección 1 (Antes de empezar) aplica a los tres roles por igual.

---

## 1. Antes de empezar (para los tres roles)

### 1.1 Cómo ingresar al sistema (iniciar sesión)

1. Abre el sistema en tu navegador.
2. Ingresa tu **correo electrónico** y tu **contraseña**.
3. Presiona "Ingresar".

`[AQUÍ VA FOTO: pantalla de inicio de sesión]`

Si el correo o la contraseña no son correctos, el sistema te mostrará el mensaje "Credenciales incorrectas" sin decirte cuál de los dos datos falló — es una medida de seguridad, no un error del sistema.

Si tu cuenta fue desactivada por un Administrador, verás el mensaje "La cuenta está desactivada". En ese caso, comunícate con el centro de salud para que revisen tu situación.

Al ingresar correctamente, cada rol es llevado a una pantalla distinta:
- **Administrador y Médico** → Panel principal (Dashboard).
- **Padre/Representante** → directamente a "Mis Pequeños" (el listado de tus hijos).

### 1.2 Activar tu cuenta por primera vez

Cuando te dan de alta en el sistema (como Médico, Padre o Administrador), recibirás un **correo de bienvenida** con un enlace para activar tu cuenta y definir tu propia contraseña. Ese enlace es válido por **48 horas** — si pasa ese tiempo sin usarlo, pide que te generen uno nuevo.

1. Abre el enlace del correo.
2. Escribe la contraseña que quieres usar y repítela para confirmarla.
3. La contraseña debe tener al menos 8 caracteres, con al menos una mayúscula, un número y un carácter especial (por ejemplo `#`, `@`, `!`).
4. Presiona "Activar cuenta".

`[AQUÍ VA FOTO: pantalla de activación de cuenta]`

Una vez activada, ya puedes iniciar sesión normalmente (paso 1.1).

### 1.3 Si olvidaste tu contraseña

**Esta opción de "olvidé mi contraseña" es exclusiva para el rol Médico.** Si eres Médico:

1. En la pantalla de inicio de sesión, presiona "¿Olvidaste tu contraseña?".
2. Escribe tu correo.
3. El sistema siempre te mostrará el mismo mensaje ("si el correo corresponde a una cuenta médica activa, recibirás un enlace en unos minutos"), exista o no esa cuenta — es intencional, por seguridad.
4. Si tu correo es válido, te llegará un enlace para definir una nueva contraseña, válido por 24 horas.

**Si eres Padre/Representante o Administrador**, no existe esta opción de autoservicio: debes pedirle a tu Médico responsable (si eres Padre) o a un Administrador (si eres Médico o Administrador) que restablezca tu contraseña desde su propia pantalla de gestión de usuarios. Te llegará un correo con el enlace para definirla (ver secciones 2.3 y 3.5).

### 1.4 Definir una nueva contraseña (desde el enlace del correo)

1. Abre el enlace recibido por correo (ya sea porque lo pediste tú o porque un Médico/Administrador lo generó por ti).
2. Escribe la nueva contraseña y su confirmación (mismos requisitos que en la activación: 8 caracteres, mayúscula, número, carácter especial).
3. Presiona "Guardar".

Si el enlace ya fue usado antes o ya expiró, el sistema te lo indicará con el mensaje "El enlace de restablecimiento no es válido o ya expiró" — en ese caso debes pedir que te generen uno nuevo.

### 1.5 Tu perfil

Desde el menú, en la esquina donde aparece tu nombre, puedes entrar a **"Mi Perfil"** para ver y editar tus propios datos.

`[AQUÍ VA FOTO: pantalla de perfil]`

Si tu rol es **Médico**, en esta misma pantalla encontrarás además el panel de **firma digital** (ver sección 3.3) — es exclusivo del Médico, ni el Administrador ni el Padre lo tienen.

### 1.6 Cerrar sesión

Presiona el botón "Cerrar sesión" (ícono de salida, junto a tu nombre en el menú lateral). Se cerrará tu sesión y volverás a la pantalla de inicio de sesión.

---

## 2. Manual del rol Administrador

### 2.1 Qué puede hacer el Administrador (resumen)

El Administrador es responsable de la parte "administrativa" del sistema: quién tiene acceso, qué puede ver cada rol, cómo se configuran los correos automáticos y el horario de atención, y de revisar que todo funcione correctamente. **No participa en la parte clínica** — no registra pacientes, no ve historias clínicas, no agenda citas en nombre propio, no emite recetas ni ve el catálogo de alimentos.

### 2.2 Panel principal (Dashboard)

Es la pantalla de inicio al ingresar. Muestra un resumen general del sistema.

`[AQUÍ VA FOTO: dashboard del Administrador]`

### 2.3 Usuarios (cuentas del sistema)

Esta es la pantalla central del Administrador: el listado unificado de **todas** las cuentas del sistema (Administradores, Médicos y Padres/Representantes), con las acciones que aplican a cualquiera de ellas.

`[AQUÍ VA FOTO: listado de usuarios]`

**Crear una cuenta nueva.** Presiona "Crear usuario" y elige el tipo de cuenta:
- **Administrador**: solo pide el correo. Se le envía el correo de activación (sección 1.2).
- **Médico**: pide nombre, apellido, especialidad, teléfono y correo.
- **Representante (Padre)**: pide nombre, apellido, teléfono, correo, y **a qué Médico responsable queda asignado** (obligatorio elegirlo).

`[AQUÍ VA FOTO: formulario de crear usuario]`

En los tres casos, la cuenta queda inactiva hasta que la persona reciba el correo y active su cuenta (sección 1.2). Si el correo ya está registrado, el sistema te avisará que ya existe.

**Activar / Desactivar una cuenta.** Desde el listado, puedes cambiar el estado de cualquier cuenta:
- No puedes desactivar tu propia cuenta.
- No puedes dejar el sistema sin ningún Administrador activo (si eres el único, no se te permite desactivarte a ti mismo ni a otro Administrador que sea el último que queda activo).
- **Para desactivar una cuenta, el sistema te pedirá obligatoriamente escribir un motivo** (mínimo 10 caracteres) en una ventana emergente. No se puede desactivar sin explicar por qué.
- Para volver a activar una cuenta no se pide motivo.

`[AQUÍ VA FOTO: ventana de motivo al desactivar una cuenta]`

Cada cambio de estado (activar o desactivar) queda guardado en un **historial**: puedes entrar al detalle de cualquier usuario y ver la lista completa de cambios de estado que tuvo esa cuenta, con la fecha, quién lo hizo y el motivo (si fue una desactivación).

`[AQUÍ VA FOTO: historial de estado de una cuenta]`

**Restablecer contraseña de otra cuenta.** Desde el detalle de un Médico o un Padre, presiona "Restablecer contraseña". Se invalida cualquier enlace anterior pendiente y se envía un correo nuevo con un enlace válido por 24 horas.

**Editar o eliminar una cuenta.** Si la fila tiene un Médico o un Padre vinculado, el sistema te lleva directo a la pantalla de edición de ese Médico o Padre (ver 2.5 y 2.6) — el listado de Usuarios en sí no tiene su propio formulario de edición, es un índice que reutiliza esas pantallas.

**Alta masiva desde Excel.** Puedes descargar una plantilla de Excel, completarla con varias filas de Médicos y/o Padres, y subirla de una sola vez. El sistema procesa fila por fila: si una fila tiene un error, no detiene el resto — al final te muestra un resumen de cuántas se crearon, cuántas se actualizaron, y el detalle de los errores.

`[AQUÍ VA FOTO: modal de importación de Excel]`

**Exportar el listado.** Puedes descargar el listado completo de usuarios como archivo Excel o como PDF, con el botón de exportar.

### 2.4 Roles y permisos

Esta pantalla controla **qué módulos y opciones ve cada rol** en su menú, y qué botones (crear/editar/eliminar) le aparecen en cada pantalla. Es una configuración de "qué se muestra", no de seguridad estricta — es decir, sirve para simplificar lo que cada rol ve en su menú diario.

1. Elige el rol a configurar: **Médico** o **Padre** (el rol Administrador está protegido y no se puede modificar).
2. Marca o desmarca las casillas de los módulos/opciones que quieres que ese rol vea.
3. Presiona "Guardar cambios" (el botón solo aparece si hiciste algún cambio).

`[AQUÍ VA FOTO: pantalla de roles y permisos]`

**Importante para tener en cuenta**: si un Médico o Padre ya tiene la sesión abierta cuando cambias sus permisos, no verá el cambio reflejado hasta que recargue la página o vuelva a iniciar sesión.

### 2.5 Médicos (a través de Usuarios)

El Administrador no tiene un listado propio de médicos para navegar libremente — gestiona a los médicos **desde la pantalla de Usuarios** (sección 2.3):
- **Registrar un médico nuevo**: "Crear usuario" → elegir tipo "Médico" (ver 2.3).
- **Editar los datos profesionales de un médico** (nombre, apellido, especialidad, teléfono): desde el detalle de su cuenta en Usuarios. El correo de acceso no se edita desde este formulario.
- **Eliminar (dar de baja) a un médico**: el sistema **no lo permite** si ese médico todavía tiene padres o niños activos asignados — primero hay que reasignarlos a otro médico (ver 2.6 y 2.7).
- **Restablecer su contraseña**: igual que cualquier cuenta (sección 2.3).

### 2.6 Padres / Representantes (a través de Usuarios)

De la misma manera, el Administrador gestiona a los padres desde Usuarios:
- **Registrar un padre nuevo**: "Crear usuario" → tipo "Representante", indicando obligatoriamente el médico responsable.
- **Editar sus datos generales** (nombre, apellido, teléfono).
- **Cambiar el correo de acceso**: es una acción aparte, con confirmación explícita — no se cambia desde el formulario general de edición.
- **Reasignar el médico responsable de un padre**: acción **exclusiva del Administrador** (ni el Médico puede hacerlo). Sirve, por ejemplo, para poder eliminar a un médico que tenía padres asignados.
- **Eliminar (dar de baja) a un padre**: el sistema **no lo permite** si ese padre tiene hijos activos — primero hay que reasignar o eliminar a esos niños.

`[AQUÍ VA FOTO: pantalla de reasignar médico de un padre]`

### 2.7 Pacientes (niños)

El Administrador puede ver el **listado completo** de todos los niños registrados en el sistema (los médicos solo ven los suyos). Puede editar datos generales de un niño, y tiene una acción exclusiva: **reasignar el médico responsable de un niño puntual** — sirve para destrabar la eliminación de un médico. El Administrador **no** registra ni elimina niños directamente; eso es tarea del Médico (sección 3.4).

### 2.8 Citas médicas (agendar en nombre de un médico)

El Administrador puede **agendar o reagendar** una cita en nombre de cualquier médico (por ejemplo, si alguien llama por teléfono y el médico no está disponible en ese momento para hacerlo él mismo). No puede iniciar consultas ni cambiar el estado de una cita — eso es exclusivo del Médico.

### 2.9 Parámetros del sistema

Aquí se configura el **horario de atención** que usa el sistema para validar que las citas se agenden dentro de un rango permitido:
- Hora de inicio (por defecto 08:00)
- Hora de fin (por defecto 18:00)
- Días hábiles (por defecto Lunes a Viernes)

`[AQUÍ VA FOTO: pantalla de parámetros del sistema]`

Puedes crear, editar o eliminar estos valores. **Ten cuidado al eliminar uno**: el sistema no te avisará si algún otro módulo lo está usando en ese momento — si borras la hora de inicio o fin, el sistema simplemente dejará de validar ese límite al agendar citas hasta que lo vuelvas a crear.

### 2.10 Plantillas de correo y eventos de correo

El sistema envía correos automáticos en ciertos momentos (alta de una cuenta, cambio de contraseña, cita agendada o reagendada). Desde aquí puedes personalizar el **contenido** de esos correos:

**Plantillas de correo**: crea o edita el asunto y el cuerpo (con formato) de un correo, usando variables como `{{NOMBRE}}`, `{{APELLIDO}}`, `{{LINK}}` que el sistema reemplaza automáticamente al enviarlo. Puedes activar o desactivar una plantilla.

`[AQUÍ VA FOTO: editor de plantilla de correo]`

**Eventos de correo**: son los 6 "momentos" fijos del sistema que disparan un correo (cuenta de padre creada, cuenta de médico creada, cuenta de administrador creada, cambio de contraseña, cita agendada, cita reagendada). No puedes crear eventos nuevos, pero sí decides **qué plantilla usa cada evento**. Cada evento muestra si tiene una plantilla activa asignada, una inactiva, o ninguna (en ese último caso, ese correo no se podrá enviar correctamente).

`[AQUÍ VA FOTO: pantalla de eventos de correo]`

**No se puede eliminar una plantilla que esté asignada a un evento activo** — primero hay que reasignarle otra plantilla al evento.

### 2.11 Actividad del sistema (auditoría)

Registra automáticamente cada creación, edición o eliminación que ocurre en el sistema (pacientes, citas, recetas, usuarios, etc.), sin que nadie tenga que activarlo — corre siempre en segundo plano. Como Administrador, ves **todo** el historial de todos los usuarios, con la posibilidad de filtrar por fecha, tipo de acción, usuario y módulo, y de ver el detalle de qué cambió exactamente en cada registro (valor anterior vs. valor nuevo).

`[AQUÍ VA FOTO: pantalla de actividad del sistema]`

### 2.12 Eventos del sistema (logs)

Es un registro más técnico, exclusivo del Administrador, que guarda los eventos de inicio de sesión, restablecimiento de contraseña y activación de cuenta (de Médicos y Administradores). Sirve para revisar quién entró al sistema y cuándo.

### 2.13 Descargar reportes e información

En la mayoría de los listados que administras (Usuarios, Médicos, Padres, Pacientes, Auditoría, Eventos del sistema) vas a encontrar un botón para **exportar** la información que estás viendo, en dos formatos:
- **Excel** (.xlsx): para trabajar los datos en una hoja de cálculo.
- **PDF**: listo para imprimir o compartir, con el logo del sistema y la fecha de generación.

---

## 3. Manual del rol Médico

### 3.1 Qué puede hacer el Médico (resumen)

El Médico es quien opera el sistema día a día: registra a sus pacientes, mide su crecimiento, agenda y atiende sus citas, emite recetas, revisa predicciones de crecimiento y recomienda alimentos. También gestiona a los representantes (padres) de sus propios pacientes.

### 3.2 Panel principal

Al ingresar, ves un resumen general (por ejemplo, tus próximas citas del día).

`[AQUÍ VA FOTO: dashboard del Médico]`

### 3.3 Tu firma digital

Desde tu perfil (sección 1.5), puedes registrar tu **firma manuscrita** para que se incluya automáticamente en los reportes de historia clínica que generes.

1. Entra a "Mi Perfil".
2. En el panel de firma, dibuja tu firma con el mouse (o con el dedo, si usas una pantalla táctil) sobre el recuadro en blanco.
3. Presiona "Guardar". El sistema guarda la imagen de tu firma junto con la fecha en que la registraste.

`[AQUÍ VA FOTO: panel de firma digital en el perfil]`

Si ya tienes una firma guardada, la verás en pantalla; puedes borrarla y volver a dibujarla cuando quieras. Si intentas guardar una imagen inválida o demasiado pesada, el sistema te lo indicará.

**¿Para qué sirve?** Cuando generas el reporte PDF de la Historia Clínica de un paciente (sección 3.12), tu firma se incrusta automáticamente al final del documento, debajo de la tabla de recetas — si no tienes una firma guardada, el reporte se genera igual, simplemente sin esa sección.

### 3.4 Pacientes (niños)

Es tu listado principal de trabajo: todos los niños que tienes asignados como médico responsable.

`[AQUÍ VA FOTO: listado de pacientes]`

**Registrar un paciente nuevo.** Presiona "Crear paciente" y completa nombre, apellido, fecha de nacimiento, sexo, el representante (padre) al que pertenece, y sus datos clínicos iniciales.

`[AQUÍ VA FOTO: formulario de crear paciente]`

**Ver el detalle de un paciente.** Desde aquí accedes a toda su información: datos generales, historial de medidas (con gráficas de evolución), historial de citas y prescripciones, y el botón para generar su reporte de Historia Clínica en PDF.

`[AQUÍ VA FOTO: detalle de un paciente]`

**Editar datos.** Puedes actualizar la información general del niño en cualquier momento.

**Reasignar el representante (padre) de un niño.** Es una acción tuya (no del Administrador) — sirve, por ejemplo, cuando cambia el tutor legal de un niño.

**Eliminar (dar de baja) a un paciente.** A diferencia de médicos y padres, un niño se puede dar de baja sin restricciones. Al hacerlo, todas sus citas pendientes se cancelan automáticamente (las citas ya completadas o pasadas se mantienen intactas, como historial).

**Alta masiva y exportación.** Igual que en las demás pantallas: puedes importar varios pacientes desde una plantilla de Excel, y exportar tu listado a Excel o PDF.

### 3.5 Padres / Representantes

Tu listado de representantes (los padres de tus propios pacientes).

**Registrar un padre nuevo.** Al crearlo, queda automáticamente asignado a ti como médico responsable (no puedes asignarlo a otro médico — eso lo hace el Administrador si hace falta, sección 2.6).

**Editar datos generales, cambiar su correo de acceso, restablecer su contraseña.** Igual que describe la sección 2.6, pero desde tu propio listado.

**Eliminar a un padre.** El sistema no lo permite si tiene hijos activos asignados — primero hay que reasignarlos o eliminarlos.

`[AQUÍ VA FOTO: listado de padres del médico]`

### 3.6 Medidas (peso y talla)

Aquí registras el crecimiento de cada paciente a lo largo del tiempo.

1. Entra al paciente y a su sección de Medidas.
2. Presiona "Registrar medida", ingresa la fecha, el peso (kg) y la talla (cm).
3. El sistema no permite fechas futuras, ni valores fuera de un rango razonable para un niño, ni **más de una medida por niño en el mismo mes calendario**.

`[AQUÍ VA FOTO: formulario de registrar medida]`

Cada medida que ves en pantalla se muestra junto con:
- El **percentil** de peso y de talla, calculado según la edad y sexo del niño contra las tablas de referencia de la Organización Mundial de la Salud (OMS).
- El **estado nutricional** (Bajo peso severo, Bajo peso, Normal, Sobrepeso, Obesidad), calculado a partir del percentil de peso.

También verás gráficas de la evolución del peso y la talla en el tiempo, y del Índice de Masa Corporal (IMC).

`[AQUÍ VA FOTO: gráficas de evolución clínica]`

Puedes editar o eliminar una medida ya registrada (reaplicando las mismas validaciones de fecha y rango).

### 3.7 Citas médicas

**Tu agenda del día**, con las citas de hoy.

`[AQUÍ VA FOTO: agenda del día]`

**Agendar una cita nueva.** Elige el paciente, la fecha y hora de inicio y fin, y un motivo opcional.
- El sistema valida que el horario esté dentro del rango de atención configurado (sección 2.9) y que no se cruce con otra cita tuya ya agendada.
- Al confirmarse, se envía automáticamente un correo al representante del niño, con una invitación de calendario adjunta.

`[AQUÍ VA FOTO: formulario de agendar cita]`

**Reagendar una cita.** Solo se puede reagendar una cita que esté Pendiente, Cancelada o No Asistió (no una que ya está En Curso o Completada). Si estaba Cancelada o No Asistió, al reagendarla vuelve automáticamente a Pendiente. Se envía un nuevo correo al representante avisando el cambio.

**Cambiar el estado de una cita** (Cancelar o marcar No Asistió), con un motivo opcional:
- Puedes **cancelar** una cita en cualquier momento, incluso antes de que llegue su fecha.
- Solo puedes marcar **"No Asistió"** (o cualquier otro cambio que no sea cancelar) **después** de que ya pasó la fecha y hora programada.

**Iniciar una consulta** desde una cita Pendiente: la cita pasa a estado "En Curso" y se abre la pantalla de la consulta médica (ver sección 3.8). Solo se puede iniciar una consulta por cita.

**Ver tu historial completo, o las citas de mis hijos por paciente.**

`[AQUÍ VA FOTO: historial de citas]`

### 3.8 Consultas y prescripciones (recetas)

Una vez que iniciaste una consulta desde una cita (sección 3.7):

**Registrar el diagnóstico.** Completa Motivo, Diagnóstico, Indicaciones y Evolución del paciente. Puedes seguir editando estos campos mientras la consulta siga "En Curso".

`[AQUÍ VA FOTO: pantalla de consulta médica]`

**Emitir una receta (prescripción).** Agrega uno o más medicamentos, cada uno con nombre, dosis, frecuencia y vía de administración (obligatorios), y opcionalmente presentación, duración, cantidad y observaciones. Una consulta puede tener varias recetas si las vas emitiendo en distintos momentos.

`[AQUÍ VA FOTO: formulario de nueva receta]`

**Editar una receta ya emitida** (mientras la consulta siga en curso).

**Completar la consulta.** Al presionar "Completar consulta", tanto la consulta como su cita asociada pasan a estado "Completada" — a partir de ahí ya no puedes editar el diagnóstico ni agregar nuevas recetas a esa consulta.

**Consultar el historial.** Puedes ver todas tus recetas emitidas ("Mis prescripciones") o las de un paciente puntual, y exportarlas a Excel o PDF.

### 3.9 Predicciones de crecimiento

Esta pantalla te muestra hacia dónde proyecta ir el **peso** de un niño en los próximos meses, calculado por un modelo estadístico especializado en curvas de crecimiento infantil.

1. Selecciona un paciente.
2. Si tiene **menos de 3 medidas registradas**, el sistema te avisará que hacen falta al menos 3 mediciones históricas para poder calcular una predicción confiable — no podrás generarla todavía.
3. Si tiene suficientes medidas, el sistema calcula y muestra tres tarjetas: la predicción de peso a **3, 6 y 12 meses**, cada una con un rango de confianza (un mínimo y un máximo esperado, no un número exacto).

`[AQUÍ VA FOTO: pantalla de predicciones]`

También verás una gráfica con la curva de crecimiento proyectada, y — si ya pasó suficiente tiempo y existe una medida real para comparar — una gráfica de precisión histórica (cuánto se acercó lo predicho a lo que realmente ocurrió).

En la parte superior de la pantalla hay un indicador de si el motor de predicciones está disponible en ese momento ("en línea" / "fuera de línea"). Si está fuera de línea, verás un mensaje explicativo y podrás seguir usando con normalidad el resto del sistema — este módulo es el único que depende de un servicio externo, y su caída no afecta a nada más.

**Nota**: hoy el sistema solo predice **peso**, no talla.

### 3.10 Catálogo de alimentos

Aquí administras las recomendaciones de alimentación que luego ven los padres (según la edad de sus hijos).

**Crear un alimento.** Nombre, categoría, edad mínima recomendada (en meses) y una recomendación/descripción.

**Editar o eliminar un alimento.**

**Gestionar categorías.** Puedes crear nuevas categorías para organizar el catálogo (por ejemplo, Frutas, Verduras, Lácteos).

`[AQUÍ VA FOTO: catálogo de alimentos]`

**Listado con filtros** por categoría, estado (activo/inactivo) y rango de edad; alta masiva desde Excel y exportación a Excel/PDF.

### 3.11 Mi actividad

Muestra el historial de auditoría, pero **acotado únicamente a las acciones que tú realizaste** (a diferencia del Administrador, que ve las de todos). Sirve para revisar tu propio historial de creaciones, ediciones y eliminaciones.

### 3.12 Reportes (PDF/Excel)

Puedes descargar en PDF o Excel casi cualquier listado que uses habitualmente: tus pacientes, tus padres/representantes, tus medidas, tus citas, tus recetas, tu actividad, el catálogo de alimentos.

Hay además **dos reportes especiales**, más completos que un simple listado:

**Reporte de Historia Clínica** (por paciente): incluye un código único de folio (por ejemplo `HC-2026-000123`), los datos del paciente y su representante, su edad calculada en años y meses, el historial completo de consultas y recetas, las gráficas de evolución de peso/talla e IMC, y — si la registraste (sección 3.3) — tu firma digital al final del documento.

`[AQUÍ VA FOTO: reporte de historia clínica en PDF]`

**Reporte de Predicciones** (por paciente): también con su propio código de folio (por ejemplo `PR-2026-000045`), tarjetas resumen de la predicción a 3/6/12 meses, las gráficas correspondientes, y una tabla de desglose mes a mes.

`[AQUÍ VA FOTO: reporte de predicciones en PDF]`

Cada vez que generas uno de estos dos reportes especiales, el sistema guarda un registro interno con su código de folio — es la forma en que el sistema mantiene trazabilidad de qué reportes se generaron, cuándo y quién lo hizo.

---

## 4. Manual del rol Padre / Representante

### 4.1 Qué puede hacer el Padre/Representante (resumen)

Tu acceso al sistema es de **solo lectura** sobre tus propios hijos — no puedes crear, editar ni eliminar nada. Todo lo que ves está pensado en lenguaje simple, sin tecnicismos médicos, para que cualquier familia lo entienda sin dificultad.

### 4.2 Mis Pequeños

Es la pantalla a la que llegas apenas inicias sesión (sección 1.1). Muestra una tarjeta por cada uno de tus hijos registrados en el sistema.

`[AQUÍ VA FOTO: pantalla "Mis Pequeños"]`

Al entrar a un hijo puntual, puedes ver su progreso, sus citas y sus recetas — siempre limitado a ese hijo tuyo (el sistema no te deja ver información de niños que no son tuyos, aunque intentes cambiar la dirección en el navegador).

### 4.3 Progreso de mis hijos

Muestra el peso y la talla más recientes de tu hijo, y su historial de las últimas medidas, en un formato simple:

- Un mensaje claro sobre su estado ("¡Todo va bien!", "Peso bajo", "Peso muy bajo", "Peso elevado", "Obesidad") con una recomendación en lenguaje natural.
- Tarjetas grandes con el peso y la talla actuales.
- La lista de sus últimas 5 medidas con fecha.

`[AQUÍ VA FOTO: progreso de mis hijos]`

**No verás** percentiles ni términos clínicos técnicos — esa información está reservada a la vista del Médico (sección 3.6). Si el mensaje te genera dudas, consulta directamente con el médico responsable de tu hijo.

### 4.4 Citas de mis hijos

Consulta el historial y las próximas citas médicas de todos tus hijos.

`[AQUÍ VA FOTO: citas de mis hijos]`

### 4.5 Prescripciones de mis hijos

Consulta las recetas médicas indicadas para tus hijos, mostradas en lenguaje simple: solo los medicamentos y las indicaciones (sin el diagnóstico ni la evolución clínica detallada, que son de uso interno del médico).

`[AQUÍ VA FOTO: prescripciones de mis hijos]`

### 4.6 Alimentación recomendada

Un explorador visual de alimentos recomendados, organizados por categoría (frutas, verduras, lácteos, etc.), filtrados automáticamente según la edad de tu hijo.

`[AQUÍ VA FOTO: alimentación recomendada]`

---

## 5. Preguntas frecuentes

**¿Por qué no puedo ver los datos de un paciente que no es mío?**
Por seguridad: cada rol solo puede ver la información que le corresponde (un Médico ve solo sus propios pacientes, un Padre solo a sus propios hijos). Si necesitas ver datos de un paciente ajeno, debe reasignarse formalmente al médico o representante correspondiente.

**Registré una cuenta nueva pero la persona no recibió el correo de activación, ¿qué hago?**
Verifica primero la carpeta de spam/correo no deseado. Si sigue sin aparecer, un Administrador (o un Médico, en el caso de un Padre) puede volver a disparar el correo usando la opción de "Restablecer contraseña" sobre esa cuenta, que genera y envía un nuevo enlace.

**¿Por qué no puedo eliminar a un médico o a un padre?**
El sistema bloquea la eliminación mientras esa persona todavía tenga pacientes o hijos activos asignados. Primero hay que reasignarlos a otra persona (sección 2.6/2.7), y recién ahí se puede eliminar.

**Cambié un permiso de un rol y no veo el cambio reflejado en la otra persona que ya tenía la sesión abierta.**
Es esperado: el cambio se aplica de inmediato en la base del sistema, pero la persona que ya tenía el menú cargado no lo verá actualizado hasta que recargue la página o vuelva a iniciar sesión.

**¿Por qué no puedo generar una predicción de crecimiento para un paciente?**
El sistema exige un mínimo de 3 medidas de peso y talla ya registradas para ese niño antes de poder calcular una predicción confiable.

**El indicador de predicciones dice "fuera de línea", ¿el sistema tiene un problema?**
No necesariamente. Ese módulo depende de un servicio externo especializado; si está temporalmente fuera de línea, el resto del sistema sigue funcionando con normalidad — solo esa pantalla puntual queda sin disponibilidad hasta que se restablezca.

---

## Preguntas para tu revisión antes de pasar a Word

1. ¿La profundidad y el tono de cada sección son los que buscas, o prefieres más/menos detalle en alguna en particular?
2. ¿Confirmas el orden de las secciones por rol (Administrador → Médico → Padre), igual que se armó el inventario de pantallas la sesión pasada?
3. Cuando tengas las credenciales de prueba de Médico y Padre, ¿capturamos las pantallas en el mismo orden en que aparecen acá, reemplazando cada marcador `[AQUÍ VA FOTO: ...]`?
