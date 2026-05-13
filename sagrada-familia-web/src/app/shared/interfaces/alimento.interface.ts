export interface AlimentoResponse {
    id: number;
    categoriaId: number;
    categoriaNombre: string;
    nombre: string;
    descripcion: string | null;
    edadMinimaIntro: number;
    edadMaxima: number | null;
    recomendacion: string | null;
}

export interface AlimentoCreate {
    categoriaId: number;
    nombre: string;
    descripcion?: string;
    edadMinimaIntro: number;
    edadMaxima?: number;
    recomendacion?: string;
}

export interface AlimentoUpdate {
    categoriaId: number;
    nombre: string;
    descripcion?: string;
    edadMinimaIntro: number;
    edadMaxima?: number;
    recomendacion?: string;
}

// Sub-interfaces para categorías
export interface CategoriaResponse {
    id: number;
    nombre: string;
    descripcion: string | null;
    totalAlimentos: number;
}

export interface CategoriaCreate {
    nombre: string;
    descripcion?: string;
}