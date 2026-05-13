export interface PadreResponse {
    id: number;
    nombre: string;
    apellido: string;
    email: string;
    telefono: string | null;
    totalHijos: number;
    fechaCreacion: string;
}

export interface PadreCreate {
    medicoId: number;
    nombre: string;
    apellido: string;
    email: string;
    password: string;
    telefono?: string;
}

export interface PadreUpdate {
    nombre: string;
    apellido: string;
    telefono?: string;
}