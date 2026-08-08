export interface CatalogoValorResponse {
    id: number;
    tipo: string;
    codigo: string;
    valor: string;
    descripcion?: string;
    activo: boolean;
    fechaCreacion: string;
    fechaActualizacion?: string;
}

export interface CatalogoValorCreate {
    tipo: string;
    codigo: string;
    valor: string;
    descripcion?: string;
}

export interface CatalogoValorUpdate {
    valor: string;
    descripcion?: string;
    activo: boolean;
}
