export interface PrediccionResponse {
    puedePredecir: boolean;
    mensaje: string | null;
    ninoId: number;
    predicciones: PuntoPrediccion[];
}

export interface PuntoPrediccion {
    meses: number;
    fechaObjetivo: string;
    pesoPredicho: number;
    pesoMinimo: number;
    pesoMaximo: number;
    pesoReal: number | null;
}

export interface PrediccionHealth {
    status: string;
    service: string;
    version: string;
}