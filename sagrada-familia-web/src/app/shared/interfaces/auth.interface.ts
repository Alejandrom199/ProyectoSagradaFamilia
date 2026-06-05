export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    id: number;
    medicoId?: number;
    nombre: string;
    apellido: string;
    rol: string;
    expiracion: string;
}

export interface SessionUser {
    id: number;
    medicoId?: number;
    nombre: string;
    apellido: string;
    rol: string;
}