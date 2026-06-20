import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { EventoCorreoAsignar, EventoCorreoResponse } from '../../shared/interfaces/evento-correo.interface';

@Injectable({ providedIn: 'root' })
export class EventosCorreoService {
    private readonly url = `${environment.apiUrl}/eventoscorreo`;

    constructor(private http: HttpClient) { }

    obtenerTodos(): Observable<ApiResponse<EventoCorreoResponse[]>> {
        return this.http.get<ApiResponse<EventoCorreoResponse[]>>(this.url, { withCredentials: true });
    }

    asignarPlantilla(eventoId: number, request: EventoCorreoAsignar): Observable<ApiResponse<EventoCorreoResponse>> {
        return this.http.patch<ApiResponse<EventoCorreoResponse>>(
            `${this.url}/${eventoId}/plantilla`,
            request,
            { withCredentials: true }
        );
    }
}
