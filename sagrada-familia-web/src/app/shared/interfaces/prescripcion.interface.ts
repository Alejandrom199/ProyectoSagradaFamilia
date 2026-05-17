export interface PrescripcionResponse {
    id: number;
    ninoId: number;
    nombreNino: string;
    medicoId: number;
    nombreMedico: string;
    especialidadMedico?: string;
    detalleMedicamentos: string;
    indicaciones?: string;
    fechaCreacion: string;
}

export interface PrescripcionCreate {
    ninoId: number;
    detalleMedicamentos: string;
    indicaciones?: string;
}

export interface PrescripcionUpdate {
    detalleMedicamentos: string;
    indicaciones?: string;
}
