export interface ImportError {
    fila: number;
    mensaje: string;
}

export interface ImportResult {
    totalProcesadas: number;
    importados: number;
    actualizados: number;
    errores: ImportError[];
}
