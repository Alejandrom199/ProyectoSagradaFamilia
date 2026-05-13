export interface MedicoResponse {
    id: number;
    nombre: string;
    apellido: string;
    email: string;
    telefono: string | null;
    especialidad: string | null;
    numeroColegiatura: string | null;
    fechaCreacion: string;
}

export interface MedicoCreate {
    nombre: string;
    apellido: string;
    email: string;
    password: string;
    telefono?: string;
    especialidad?: string;
    numeroColegiatura?: string;
}

export interface MedicoUpdate {
    nombre: string;
    apellido: string;
    telefono?: string;
    especialidad?: string;
    numeroColegiatura?: string;
}