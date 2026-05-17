export interface AuditoriaResponse {
    id: number;
    usuarioId: number;
    usuarioEmail: string;
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
