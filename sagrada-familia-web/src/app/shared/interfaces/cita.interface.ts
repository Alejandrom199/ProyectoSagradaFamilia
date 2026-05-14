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
    Pendiente = 0,
    Completada = 1,
    Cancelada = 2,
}