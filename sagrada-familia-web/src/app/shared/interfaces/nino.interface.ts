export interface NinoResponse {
    id: number;
    padreId: number;
    nombrePadre: string;
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: string;
    edadMeses: number;
    fechaCreacion: string;
}

export interface NinoCreate {
    padreId: number;
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
}