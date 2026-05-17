export interface AlimentoResponse {
    id: number;
    categoriaId: number;
    categoriaNombre: string;
    nombre: string;
    descripcion?: string;
    edadMinimaMeses: number;
    recomendacion?: string;
    activo: boolean;
}

export interface AlimentoCreate {
    categoriaId: number;
    nombre: string;
    descripcion?: string;
    edadMinimaMeses: number;
    recomendacion?: string;
}

export interface AlimentoUpdate {
    categoriaId: number;
    nombre: string;
    descripcion?: string;
    edadMinimaMeses: number;
    recomendacion?: string;
    activo: boolean;
}

export interface CategoriaResponse {
    id: number;
    nombre: string;
    descripcion?: string;
    totalAlimentos: number;
    activo: boolean;
}

export interface CategoriaCreate {
    nombre: string;
    descripcion?: string;
}

export interface CategoriaUpdate {
    nombre: string;
    descripcion?: string;
    activo: boolean;
}