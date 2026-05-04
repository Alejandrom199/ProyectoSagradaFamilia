export interface MenuResponse {
    id: number;
    nombre: string;
    icono: string;
    orden: number;
    opciones: OpcionResponse[];
}

export interface OpcionResponse {
    id: number;
    nombre: string;
    ruta: string;
    icono: string;
    orden: number;
    acciones: string[];
}