export interface CrearNinoRequest {
    representanteId: number;
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: string;
}

export interface ActualizarNinoRequest {
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: string;
}