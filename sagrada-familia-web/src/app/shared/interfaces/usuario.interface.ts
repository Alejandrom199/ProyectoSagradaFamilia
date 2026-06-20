export interface UsuarioResponse {
    id: number;
    email: string;
    rolId: number;
    rolNombre: string;
    activo: boolean;
    fechaCreacion: string;
    esMedico: boolean;
    esPadre: boolean;
}

export interface UsuarioDetailResponse {
    id: number;
    email: string;
    rolId: number;
    rolNombre: string;
    activo: boolean;
    fechaCreacion: string;
    medicoId?: number;
    padreId?: number;
}

export interface UsuarioCreate {
    email: string;
    rolId: number;
}

export interface UsuarioUpdate {
    rolId: number;
    activo: boolean;
}