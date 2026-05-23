export interface ParametroResponse {
    id: number;
    grupo: string;
    codigo: string;
    valor: string;
    descripcion?: string;
    activo: boolean;
    fechaCreacion: string;
    fechaActualizacion?: string;
}

export interface ParametroCreate {
    grupo: string;
    codigo: string;
    valor: string;
    descripcion?: string;
}

export interface ParametroUpdate {
    valor: string;
    descripcion?: string;
    activo: boolean;
}
