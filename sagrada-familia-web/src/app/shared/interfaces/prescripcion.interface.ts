export interface MedicamentoInterface {
    id?: number;
    nombre: string;
    presentacion?: string;
    dosis: string;
    frecuencia: string;
    viaAdministracion: string;
    duracion?: string;
    cantidad?: string;
    observaciones?: string;
}

export interface PrescripcionResponse {
    id: number;
    consultaId: number;
    citaId: number;
    ninoId: number;
    nombreNino: string;
    medicoId: number;
    nombreMedico: string;
    especialidadMedico?: string;
    indicaciones?: string;
    medicamentos: MedicamentoInterface[];
    fechaCreacion: string;
}

export interface PrescripcionCreate {
    consultaId: number;
    medicamentos: MedicamentoInterface[];
    indicaciones?: string;
}

export interface PrescripcionUpdate {
    medicamentos: MedicamentoInterface[];
    indicaciones?: string;
}

export interface PrescripcionResumen {
    id: number;
    medicamentos: MedicamentoInterface[];
    indicaciones: string;
    fechaCreacion: string;
}
