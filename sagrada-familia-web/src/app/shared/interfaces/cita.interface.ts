export interface CitaResponse {
    id: number;
    ninoId: number;
    nombreNino: string;
    medicoId: number;
    nombreMedico: string;
    fechaHora: string;
    motivo?: string;
    estado: string;
    fechaCreacion: string;
}

export interface CitaCreate {
    ninoId: number;
    medicoId: number;
    fechaHora: string;
    motivo?: string;
}

export interface CitaUpdate {
    medicoId: number;
    fechaHora: string;
    motivo?: string;
}

export enum EstadoCita {
    Pendiente = 1,
    Completada = 2,
    Cancelada = 3,
    NoAsistio = 4,
}