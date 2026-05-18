export interface PrescripcionResponse {
    id: number;
    citaId: number;
    ninoId: number;
    nombreNino: string;
    medicoId: number;
    nombreMedico: string;
    especialidadMedico?: string;
    diagnostico?: string;
    detalleMedicamentos: string;
    indicaciones?: string;
    fechaCreacion: string;
}

export interface PrescripcionCreate {
    citaId: number;
    detalleMedicamentos: string;
    indicaciones?: string;
    diagnostico?: string;
}

export interface PrescripcionUpdate {
    detalleMedicamentos: string;
    indicaciones?: string;
}