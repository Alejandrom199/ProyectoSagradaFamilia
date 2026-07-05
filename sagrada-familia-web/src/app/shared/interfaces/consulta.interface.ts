export interface ConsultaResponse {
    id: number;
    citaId: number;
    estado: string;
    motivo?: string;
    diagnostico?: string;
    indicaciones?: string;
    evolucion?: string;
    fechaCreacion: string;
}

export interface ConsultaActualizar {
    motivo?: string;
    diagnostico?: string;
    indicaciones?: string;
    evolucion?: string;
}
