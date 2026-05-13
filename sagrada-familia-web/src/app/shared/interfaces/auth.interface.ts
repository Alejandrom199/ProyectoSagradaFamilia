export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    accessToken: string;
    refreshToken: string;
    id: number;
    nombre: string;
    rol: string;
    expiracion: string;
}

export interface SessionUser {
    id: number;
    nombre: string;
    rol: string;
    accessToken?: string;
}