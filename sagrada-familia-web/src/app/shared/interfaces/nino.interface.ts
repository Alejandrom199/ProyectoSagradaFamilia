export interface NinoResponse {
    id: number;
    padreId: number;
    nombrePadre: string;
    medicoId: number;
    nombreMedico: string;
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: 'M' | 'F';
    edadMeses: number;
    fechaCreacion: string;

    padreId: number;
    padreNombreCompleto: string;
    padreEmail: string;
    padreTelefono: string;

    medicoId: number;
    medicoNombreCompleto: string;
    medicoEspecialidad?: string;
}

export interface NinoDetailResponse {
    id: number;
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: 'M' | 'F';
    edadMeses: number;
    fechaCreacion: string;

    padreId: number;
    padreNombreCompleto: string;
    padreEmail: string;
    padreTelefono: string;

    medicoId: number;
    medicoNombreCompleto: string;
    medicoEspecialidad?: string;
}

export interface NinoCreate {
    padreId: number;
    medicoId: number;
    medicoId: number;
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: 'M' | 'F';
}

export interface NinoUpdate {
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: 'M' | 'F';
    medicoId?: number;
    medicoId?: number;
}