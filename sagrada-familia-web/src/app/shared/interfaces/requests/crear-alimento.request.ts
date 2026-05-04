export interface CrearAlimentoRequest {
    categoriaId: number;
    nombre: string;
    descripcion: string | null;
    edadMinimaIntro: number;
    edadMaxima: number | null;
    recomendacion: string | null;
}
