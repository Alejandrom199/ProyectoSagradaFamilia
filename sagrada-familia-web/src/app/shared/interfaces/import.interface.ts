export interface ImportError {
    fila: number;
    mensaje: string;
}

export interface ImportDetalle {
    fila: number;
    email: string;
    nombre: string;
    rol?: string;
    accion: string;
}

export interface ImportResult {
    totalProcesadas: number;
    importados: number;
    actualizados: number;
    errores: ImportError[];
    detalle?: ImportDetalle[];
}
