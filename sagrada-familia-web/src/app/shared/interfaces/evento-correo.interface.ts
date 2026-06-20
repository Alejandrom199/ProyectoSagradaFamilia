export interface EventoCorreoResponse {
    id:                number;
    codigo:            string;
    nombre:            string;
    descripcion:       string;
    variables:         string[];
    plantillaCorreoId: number | null;
    plantillaNombre:   string | null;
    plantillaActiva:   boolean;
}

export interface EventoCorreoAsignar {
    plantillaCorreoId: number | null;
}
