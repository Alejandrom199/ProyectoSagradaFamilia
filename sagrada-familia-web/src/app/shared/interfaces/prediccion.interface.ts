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

export interface PuntoOms {
    edadMeses:   number;
    percentil3:  number;
    percentil15: number;
    percentil50: number;
    percentil85: number;
    percentil97: number;
}

export interface CurvasOmsResponse {
    sexo:   string;
    tipo:   string;
    curvas: PuntoOms[];
}