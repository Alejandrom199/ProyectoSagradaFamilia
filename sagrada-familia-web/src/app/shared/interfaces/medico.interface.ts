import { NinoResponse } from './nino.interface';

export interface MedicoResponse {
    id: number;
    nombre: string;
    apellido: string;
    email: string;
    especialidad?: string;
    telefono?: string;
}

export interface MedicoDetailResponse {
    id: number;
    usuarioId: number;
    nombre: string;
    apellido: string;
    email: string;
    especialidad?: string;
    telefono?: string;
    fechaCreacion: string;
    pacientes: NinoResponse[];
}

export interface MedicoCreate {
    email: string;
    nombre: string;
    apellido: string;
    especialidad?: string;
    telefono?: string;
}

export interface MedicoUpdate {
    nombre: string;
    apellido: string;
    especialidad?: string;
    telefono?: string;
}