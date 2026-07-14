export interface ActividadDiariaResponse {
    fecha:    string;
    cantidad: number;
}

export interface UsuarioPorRolResponse {
    rol:      string;
    cantidad: number;
}

export interface RegistroMensualResponse {
    mes:      string;
    cantidad: number;
}

export interface SaludSistemaResponse {
    version:    string;
    entorno:    string;
    iniciadoEn: string;
}

export interface DashboardAdminResponse {
    usuariosActivos:    number;
    cuentasPendientes:  number;
    accionesHoy:        number;
    alertasSemana:      number;
    actividadSemana:    ActividadDiariaResponse[];
    usuariosPorRol:     UsuarioPorRolResponse[];
    registrosPorMes:    RegistroMensualResponse[];
    accionesRecientes:  AuditoriaResponse[];
    alertasRecientes:   LogSistemaResponse[];
    salud:              SaludSistemaResponse;
}

export interface AuditoriaResponse {
    id: number;
    usuarioId: number;
    usuarioEmail: string;
    usuarioNombreCompleto?: string;
    fecha: string;
    accion: string;
    tabla: string;
    clavePrimaria: string;
    valoresAntiguos?: string;
    valoresNuevos?: string;
    ipAddress?: string;
}

export interface LogSistemaResponse {
    id: number;
    fechaHora: string;
    nivel: string;
    mensaje: string;
    excepcion?: string;
    endpoint?: string;
    usuarioId?: number;
}
