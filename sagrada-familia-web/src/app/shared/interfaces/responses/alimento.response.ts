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

export interface CategoriaResponse {
    id: number;
    nombre: string;
    descripcion: string | null;
    totalAlimentos: number;
}

export interface CategoriaPadreUI extends CategoriaResponse {
    clase: string;
    icono: string;
}