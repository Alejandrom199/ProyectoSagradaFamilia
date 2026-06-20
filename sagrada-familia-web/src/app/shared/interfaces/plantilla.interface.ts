export interface PlantillaResponse {
    id:     number;
    codigo: string | null; // null para plantillas creadas por el administrador
    nombre: string;
    asunto: string;
    cuerpo: string;
    activo: boolean;
    fechaCreacion: string;
    fechaActualizacion?: string;
}

export interface PlantillaCreate {
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
