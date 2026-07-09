import { PrescripcionResumen } from "./prescripcion.interface";
import { ConsultaResponse } from "./consulta.interface";

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
    motivoCancelacion?: string;
    estado: EstadoCita;
    citaOrigenId?: number;
    fechaCreacion: string;
    tienePrescripcion: boolean;
    prescripcion?: PrescripcionResumen;
    consulta?: ConsultaResponse;
}

export interface CambiarEstadoRequest {
    estado: number;
    motivoCancelacion?: string;
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
    Cancelada = 'Cancelada',
    Reagendada = 'Reagendada'
}