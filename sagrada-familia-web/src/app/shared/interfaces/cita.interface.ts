import { PrescripcionResumen } from "./prescripcion.interface";

export interface CitaResponse {
    id: number;
    ninoId: number;
    nombreNino: string;
    medicoId: number;
    nombreMedico: string;
    fechaHora:    string;
    fechaHoraFin: string | null;
    motivo?: string;
    notasConsulta?: string;
    estado: EstadoCita;
    fechaCreacion: string;
    tienePrescripcion: boolean;
    prescripcion?: PrescripcionResumen;
}

export interface CitaCreate {
    ninoId:       number;
    medicoId:     number;
    fechaHora:    string;
    fechaHoraFin: string;
    motivo?:      string;
}

export interface CitaUpdate {
    medicoId:     number;
    fechaHora:    string;
    fechaHoraFin: string;
    motivo?:      string;
}

export enum EstadoCita {
    Pendiente = 'Pendiente',
    Completada = 'Completada',
    EnCurso = 'EnCurso',
    NoAsistio = 'NoAsistio',
    Cancelada = 'Cancelada'
}