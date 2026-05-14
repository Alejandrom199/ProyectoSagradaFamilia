export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    id: number;
    nombre: string;
    rol: string;
    expiracion: string;
}

export interface SessionUser {
    id: number;
    nombre: string;
    rol: string;
}