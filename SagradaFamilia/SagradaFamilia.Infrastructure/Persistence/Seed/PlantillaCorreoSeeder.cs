using Microsoft.EntityFrameworkCore;

using SagradaFamilia.Domain.Entities;
using SagradaFamilia.Infrastructure.Persistence.Contexts;

namespace SagradaFamilia.Infrastructure.Persistence.Seed
{
    public static class PlantillaCorreoSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            var plantillas = new List<PlantillaCorreo>
            {
                new()
                {
                    Codigo = "CAMBIO_CLAVE",
                    Nombre = "Restablecimiento de contraseña",
                    Asunto = "Restablecimiento de contraseña — Sagrada Familia",
                    Activo = true,
                    Cuerpo = """
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Restablecer contraseña</title>
</head>
<body style="margin:0;padding:0;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="padding:48px 0;">
    <tr>
      <td align="center">
        <table width="520" cellpadding="0" cellspacing="0" style="max-width:520px;background:#ffffff;border-radius:8px;">
          <tr>
            <td style="padding:32px 40px 24px;border-bottom:1px solid #e2e8f0;">
              <p style="margin:0;font-size:13px;font-weight:700;color:#2563eb;letter-spacing:0.5px;">Sagrada Familia</p>
              <p style="margin:2px 0 0;font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;">Sistema de Salud Pediátrica</p>
            </td>
          </tr>
          <tr>
            <td style="padding:32px 40px;">
              <h2 style="margin:0 0 12px;font-size:18px;font-weight:700;color:#0f172a;">Restablecer contraseña</h2>
              <p style="margin:0 0 28px;font-size:14px;color:#475569;line-height:1.7;">
                Hola <strong style="color:#0f172a;">{{NOMBRE}}</strong>, recibimos una solicitud para restablecer la contraseña de tu cuenta.
              </p>
              <a href="{{LINK}}" style="display:inline-block;background:#2563eb;color:#ffffff;text-decoration:none;font-size:14px;font-weight:600;padding:12px 28px;border-radius:6px;">
                Cambiar contraseña
              </a>
              <p style="margin:28px 0 6px;font-size:12px;color:#94a3b8;">Si el botón no funciona, copiá este enlace en tu navegador:</p>
              <p style="margin:0 0 28px;font-size:11px;color:#2563eb;word-break:break-all;">{{LINK}}</p>
              <p style="margin:0;font-size:12px;color:#94a3b8;line-height:1.6;">
                Este enlace expira en <strong style="color:#475569;">24 horas</strong>. Si no solicitaste este cambio, podés ignorar este correo.
              </p>
            </td>
          </tr>
          <tr>
            <td style="padding:20px 40px;border-top:1px solid #e2e8f0;">
              <p style="margin:0;font-size:11px;color:#cbd5e1;">Correo automático — no respondas a este mensaje.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"""
                },
                new()
                {
                    Codigo = "CUENTA_PADRE",
                    Nombre = "Activación de cuenta — Padre",
                    Asunto = "Bienvenido a Sagrada Familia — Activá tu cuenta",
                    Activo = true,
                    Cuerpo = """
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Activá tu cuenta</title>
</head>
<body style="margin:0;padding:0;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="padding:48px 0;">
    <tr>
      <td align="center">
        <table width="520" cellpadding="0" cellspacing="0" style="max-width:520px;background:#ffffff;border-radius:8px;">
          <tr>
            <td style="padding:32px 40px 24px;border-bottom:1px solid #e2e8f0;">
              <p style="margin:0;font-size:13px;font-weight:700;color:#2563eb;letter-spacing:0.5px;">Sagrada Familia</p>
              <p style="margin:2px 0 0;font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;">Sistema de Salud Pediátrica</p>
            </td>
          </tr>
          <tr>
            <td style="padding:32px 40px;">
              <h2 style="margin:0 0 12px;font-size:18px;font-weight:700;color:#0f172a;">Bienvenido, {{NOMBRE}} {{APELLIDO}}</h2>
              <p style="margin:0 0 28px;font-size:14px;color:#475569;line-height:1.7;">
                Tu cuenta en el sistema Sagrada Familia ha sido creada. Para comenzar, establecé tu contraseña haciendo clic en el siguiente botón.
              </p>
              <a href="{{LINK}}" style="display:inline-block;background:#2563eb;color:#ffffff;text-decoration:none;font-size:14px;font-weight:600;padding:12px 28px;border-radius:6px;">
                Activar mi cuenta
              </a>
              <p style="margin:28px 0 6px;font-size:12px;color:#94a3b8;">Si el botón no funciona, copiá este enlace en tu navegador:</p>
              <p style="margin:0 0 28px;font-size:11px;color:#2563eb;word-break:break-all;">{{LINK}}</p>
              <p style="margin:0;font-size:12px;color:#94a3b8;line-height:1.6;">
                Este enlace expira en <strong style="color:#475569;">48 horas</strong>. Si no esperabas este correo, podés ignorarlo.
              </p>
            </td>
          </tr>
          <tr>
            <td style="padding:20px 40px;border-top:1px solid #e2e8f0;">
              <p style="margin:0;font-size:11px;color:#cbd5e1;">Correo automático — no respondas a este mensaje.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"""
                },
                new()
                {
                    Codigo = "CUENTA_ADMIN",
                    Nombre = "Activación de cuenta — Administrador",
                    Asunto = "Bienvenido a Sagrada Familia — Activá tu cuenta",
                    Activo = true,
                    Cuerpo = """
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Activá tu cuenta</title>
</head>
<body style="margin:0;padding:0;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="padding:48px 0;">
    <tr>
      <td align="center">
        <table width="520" cellpadding="0" cellspacing="0" style="max-width:520px;background:#ffffff;border-radius:8px;">
          <tr>
            <td style="padding:32px 40px 24px;border-bottom:1px solid #e2e8f0;">
              <p style="margin:0;font-size:13px;font-weight:700;color:#2563eb;letter-spacing:0.5px;">Sagrada Familia</p>
              <p style="margin:2px 0 0;font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;">Sistema de Salud Pediátrica</p>
            </td>
          </tr>
          <tr>
            <td style="padding:32px 40px;">
              <h2 style="margin:0 0 12px;font-size:18px;font-weight:700;color:#0f172a;">Tu cuenta de administrador está lista</h2>
              <p style="margin:0 0 8px;font-size:13px;color:#64748b;">Cuenta:</p>
              <p style="margin:0 0 28px;font-size:14px;font-weight:600;color:#0f172a;">{{EMAIL}}</p>
              <p style="margin:0 0 28px;font-size:14px;color:#475569;line-height:1.7;">
                Para activar tu acceso, establecé tu contraseña haciendo clic en el siguiente botón.
              </p>
              <a href="{{LINK}}" style="display:inline-block;background:#2563eb;color:#ffffff;text-decoration:none;font-size:14px;font-weight:600;padding:12px 28px;border-radius:6px;">
                Activar mi cuenta
              </a>
              <p style="margin:28px 0 6px;font-size:12px;color:#94a3b8;">Si el botón no funciona, copiá este enlace en tu navegador:</p>
              <p style="margin:0 0 28px;font-size:11px;color:#2563eb;word-break:break-all;">{{LINK}}</p>
              <p style="margin:0;font-size:12px;color:#94a3b8;line-height:1.6;">
                Este enlace expira en <strong style="color:#475569;">48 horas</strong>. Si no esperabas este correo, podés ignorarlo.
              </p>
            </td>
          </tr>
          <tr>
            <td style="padding:20px 40px;border-top:1px solid #e2e8f0;">
              <p style="margin:0;font-size:11px;color:#cbd5e1;">Correo automático — no respondas a este mensaje.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"""
                },
                new()
                {
                    Codigo = "CUENTA_MEDICO",
                    Nombre = "Credenciales de acceso — Médico",
                    Asunto = "Credenciales de acceso — Sagrada Familia",
                    Activo = true,
                    Cuerpo = """
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Credenciales de acceso</title>
</head>
<body style="margin:0;padding:0;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="padding:48px 0;">
    <tr>
      <td align="center">
        <table width="520" cellpadding="0" cellspacing="0" style="max-width:520px;background:#ffffff;border-radius:8px;">
          <tr>
            <td style="padding:32px 40px 24px;border-bottom:1px solid #e2e8f0;">
              <p style="margin:0;font-size:13px;font-weight:700;color:#2563eb;letter-spacing:0.5px;">Sagrada Familia</p>
              <p style="margin:2px 0 0;font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;">Sistema de Salud Pediátrica</p>
            </td>
          </tr>
          <tr>
            <td style="padding:32px 40px;">
              <h2 style="margin:0 0 12px;font-size:18px;font-weight:700;color:#0f172a;">Bienvenido, Dr. {{NOMBRE}} {{APELLIDO}}</h2>
              <p style="margin:0 0 20px;font-size:14px;color:#475569;line-height:1.7;">
                Tu cuenta médica en el sistema Sagrada Familia ha sido creada. A continuación encontrás tus credenciales de acceso.
              </p>
              <table cellpadding="0" cellspacing="0" style="background:#f8fafc;border-radius:6px;padding:16px 20px;margin-bottom:28px;">
                <tr>
                  <td style="font-size:13px;color:#64748b;padding-bottom:6px;">Usuario (email)</td>
                </tr>
                <tr>
                  <td style="font-size:14px;font-weight:600;color:#0f172a;">{{EMAIL}}</td>
                </tr>
              </table>
              <p style="margin:0 0 16px;font-size:14px;color:#475569;line-height:1.7;">
                Tu contraseña temporal fue generada por el administrador. Te recomendamos cambiarla en tu primer inicio de sesión.
              </p>
              <a href="{{LINK}}" style="display:inline-block;background:#2563eb;color:#ffffff;text-decoration:none;font-size:14px;font-weight:600;padding:12px 28px;border-radius:6px;">
                Ir al sistema
              </a>
            </td>
          </tr>
          <tr>
            <td style="padding:20px 40px;border-top:1px solid #e2e8f0;">
              <p style="margin:0;font-size:11px;color:#cbd5e1;">Correo automático — no respondas a este mensaje.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"""
                },
                new()
                {
                    Codigo = "CITA_AGENDADA",
                    Nombre = "Cita agendada — Notificación al padre",
                    Asunto = "Cita pediátrica programada — Sagrada Familia",
                    Activo = true,
                    Cuerpo = """
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Cita agendada</title>
</head>
<body style="margin:0;padding:0;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="padding:48px 0;">
    <tr>
      <td align="center">
        <table width="520" cellpadding="0" cellspacing="0" style="max-width:520px;background:#ffffff;border-radius:8px;">
          <tr>
            <td style="padding:32px 40px 24px;border-bottom:1px solid #e2e8f0;">
              <p style="margin:0;font-size:13px;font-weight:700;color:#2563eb;letter-spacing:0.5px;">Sagrada Familia</p>
              <p style="margin:2px 0 0;font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;">Sistema de Salud Pediátrica</p>
            </td>
          </tr>
          <tr>
            <td style="padding:32px 40px;">
              <h2 style="margin:0 0 8px;font-size:18px;font-weight:700;color:#0f172a;">Cita programada</h2>
              <p style="margin:0 0 28px;font-size:14px;color:#475569;line-height:1.7;">
                Hola <strong style="color:#0f172a;">{{NOMBRE_PADRE}}</strong>, se ha programado una cita pediátrica para <strong style="color:#0f172a;">{{NOMBRE_NINO}}</strong>.
              </p>
              <table cellpadding="0" cellspacing="0" style="width:100%;background:#f8fafc;border-radius:6px;border:1px solid #e2e8f0;margin-bottom:28px;">
                <tr><td style="padding:16px 20px;">
                  <p style="margin:0 0 12px;font-size:11px;font-weight:700;color:#64748b;text-transform:uppercase;letter-spacing:1px;">Detalles de la cita</p>
                  <table cellpadding="0" cellspacing="0" style="width:100%;">
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;width:110px;">Paciente</td>
                      <td style="padding:5px 0;font-size:13px;font-weight:600;color:#0f172a;">{{NOMBRE_NINO}}</td>
                    </tr>
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;">Médico</td>
                      <td style="padding:5px 0;font-size:13px;font-weight:600;color:#0f172a;">{{MEDICO}}</td>
                    </tr>
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;">Fecha</td>
                      <td style="padding:5px 0;font-size:13px;font-weight:600;color:#0f172a;">{{FECHA}}</td>
                    </tr>
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;">Horario</td>
                      <td style="padding:5px 0;font-size:13px;font-weight:600;color:#0f172a;">{{HORA_INICIO}} – {{HORA_FIN}} hrs</td>
                    </tr>
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;">Motivo</td>
                      <td style="padding:5px 0;font-size:13px;color:#475569;">{{MOTIVO}}</td>
                    </tr>
                  </table>
                </td></tr>
              </table>
              <p style="margin:0;font-size:12px;color:#94a3b8;line-height:1.6;">
                Si necesitás cancelar o modificar la cita, contactá a la clínica con anticipación.
              </p>
            </td>
          </tr>
          <tr>
            <td style="padding:20px 40px;border-top:1px solid #e2e8f0;">
              <p style="margin:0;font-size:11px;color:#cbd5e1;">Correo automático — no respondas a este mensaje.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"""
                },
                new()
                {
                    Codigo = "CITA_REAGENDADA",
                    Nombre = "Cita reagendada — Notificación al padre",
                    Asunto = "Tu cita fue reagendada — Sagrada Familia",
                    Activo = true,
                    Cuerpo = """
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>Cita reagendada</title>
</head>
<body style="margin:0;padding:0;font-family:'Segoe UI',Arial,sans-serif;">
  <table width="100%" cellpadding="0" cellspacing="0" style="padding:48px 0;">
    <tr>
      <td align="center">
        <table width="520" cellpadding="0" cellspacing="0" style="max-width:520px;background:#ffffff;border-radius:8px;">
          <tr>
            <td style="padding:32px 40px 24px;border-bottom:1px solid #e2e8f0;">
              <p style="margin:0;font-size:13px;font-weight:700;color:#2563eb;letter-spacing:0.5px;">Sagrada Familia</p>
              <p style="margin:2px 0 0;font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:1px;">Sistema de Salud Pediátrica</p>
            </td>
          </tr>
          <tr>
            <td style="padding:32px 40px;">
              <h2 style="margin:0 0 8px;font-size:18px;font-weight:700;color:#0f172a;">Cita reagendada</h2>
              <p style="margin:0 0 28px;font-size:14px;color:#475569;line-height:1.7;">
                Hola <strong style="color:#0f172a;">{{NOMBRE_PADRE}}</strong>, la cita de <strong style="color:#0f172a;">{{NOMBRE_NINO}}</strong> ha sido reprogramada a una nueva fecha y hora.
              </p>
              <table cellpadding="0" cellspacing="0" style="width:100%;background:#f8fafc;border-radius:6px;border:1px solid #e2e8f0;margin-bottom:28px;">
                <tr><td style="padding:16px 20px;">
                  <p style="margin:0 0 12px;font-size:11px;font-weight:700;color:#64748b;text-transform:uppercase;letter-spacing:1px;">Nueva fecha de la cita</p>
                  <table cellpadding="0" cellspacing="0" style="width:100%;">
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;width:110px;">Paciente</td>
                      <td style="padding:5px 0;font-size:13px;font-weight:600;color:#0f172a;">{{NOMBRE_NINO}}</td>
                    </tr>
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;">Médico</td>
                      <td style="padding:5px 0;font-size:13px;font-weight:600;color:#0f172a;">{{MEDICO}}</td>
                    </tr>
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;">Nueva fecha</td>
                      <td style="padding:5px 0;font-size:13px;font-weight:600;color:#2563eb;">{{FECHA}}</td>
                    </tr>
                    <tr>
                      <td style="padding:5px 0;font-size:13px;color:#64748b;">Nuevo horario</td>
                      <td style="padding:5px 0;font-size:13px;font-weight:600;color:#2563eb;">{{HORA_INICIO}} – {{HORA_FIN}} hrs</td>
                    </tr>
                  </table>
                </td></tr>
              </table>
              <p style="margin:0;font-size:12px;color:#94a3b8;line-height:1.6;">
                Si tenés alguna consulta sobre esta reprogramación, contactá a la clínica directamente.
              </p>
            </td>
          </tr>
          <tr>
            <td style="padding:20px 40px;border-top:1px solid #e2e8f0;">
              <p style="margin:0;font-size:11px;color:#cbd5e1;">Correo automático — no respondas a este mensaje.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"""
                }
            };

            var codigos = plantillas.Select(p => p.Codigo).ToHashSet();
            var existentes = await context.PlantillasCorreo
                .Where(p => codigos.Contains(p.Codigo))
                .Select(p => p.Codigo)
                .ToListAsync();

            var nuevas = plantillas.Where(p => !existentes.Contains(p.Codigo)).ToList();
            if (nuevas.Count == 0) return;

            context.PlantillasCorreo.AddRange(nuevas);
            await context.SaveChangesAsync();
        }
    }
}
