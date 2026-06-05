export interface PlantillaResponse {
    id: number;
    codigo: string;
    nombre: string;
    asunto: string;
    cuerpo: string;
    activo: boolean;
    fechaCreacion: string;
    fechaActualizacion?: string;
}

export interface PlantillaCreate {
    codigo: string;
    nombre: string;
    asunto: string;
    cuerpo: string;
}

export interface PlantillaUpdate {
    nombre: string;
    asunto: string;
    cuerpo: string;
    activo: boolean;
}
