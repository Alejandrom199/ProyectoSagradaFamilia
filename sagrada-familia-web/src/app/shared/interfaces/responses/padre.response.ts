export interface PadreResponse {
    id: number;
    nombre: string;
    apellido: string;
    email: string;
    telefono: string | null;
    totalHijos: number;
    fechaCreacion: string;
}
