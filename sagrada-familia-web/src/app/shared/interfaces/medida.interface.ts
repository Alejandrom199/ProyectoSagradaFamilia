export interface MedidaResponse {
    id: number;
    ninoId: number;
    nombreNino: string;
    medicoId: number;
    nombreMedico: string;
    fechaMedicion: string;
    peso: number;
    talla: number;
    estadoNutricional: string;
    percentilPeso: number;
    percentilTalla: number;
    fechaRegistro: string;
}

export interface MedidaCreate {
    ninoId: number;
    fechaMedicion: string;
    peso: number;
    talla: number;
}

export interface MedidaUpdate {
    fechaMedicion: string;
    peso: number;
    talla: number;
}