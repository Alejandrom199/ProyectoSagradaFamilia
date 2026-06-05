import { NinoResponse } from "./nino.interface";

export interface PadreResponse {
    id: number;
    nombre: string;
    apellido: string;
    email: string;
    telefono?: string;
    medicoId: number;
    nombreMedico: string;
    totalHijos: number;
    activo: boolean;
    fechaCreacion: string;
}

export interface PadreDetailResponse {
    id: number;
    usuarioId: number;
    nombre: string;
    apellido: string;
    email: string;
    telefono?: string;
    activo: boolean;
    fechaCreacion: string;
    medicoId: number;
    medicoNombreCompleto: string;
    hijos: NinoResponse[];
}

export interface PadreCreate {
    email: string;
    medicoId: number;
    nombre: string;
    apellido: string;
    telefono?: string;
}

export interface PadreUpdate {
    nombre: string;
    apellido: string;
    telefono?: string;
    medicoId: number;
}

export interface PadreCambiarEmail {
    email: string;
}