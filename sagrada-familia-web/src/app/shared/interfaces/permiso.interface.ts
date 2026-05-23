export interface ModuloPermisoResponse {
    moduloId: number;
    moduloNombre: string;
    opciones: OpcionPermisoResponse[];
}

export interface OpcionPermisoResponse {
    opcionId: number;
    opcionNombre: string;
    acciones: AccionPermisoResponse[];
}

export interface AccionPermisoResponse {
    opcionAccionId: number;
    accionNombre: string;
    permitido: boolean;
}

export interface ActualizarPermisoRolRequest {
    opcionAccionId: number;
    permitido: boolean;
}