import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { PlantillaCreate, PlantillaResponse, PlantillaUpdate } from '../../shared/interfaces/plantilla.interface';

@Injectable({ providedIn: 'root' })
export class PlantillasService {
    private readonly url = `${environment.apiUrl}/plantillas`;

    constructor(private http: HttpClient) { }

    obtenerTodos(): Observable<ApiResponse<PlantillaResponse[]>> {
        return this.http.get<ApiResponse<PlantillaResponse[]>>(this.url, { withCredentials: true });
    }

    obtenerPorId(id: number): Observable<ApiResponse<PlantillaResponse>> {
        return this.http.get<ApiResponse<PlantillaResponse>>(`${this.url}/${id}`, { withCredentials: true });
    }

    crear(request: PlantillaCreate): Observable<ApiResponse<PlantillaResponse>> {
        return this.http.post<ApiResponse<PlantillaResponse>>(this.url, request, { withCredentials: true });
    }

    actualizar(id: number, request: PlantillaUpdate): Observable<ApiResponse<PlantillaResponse>> {
        return this.http.put<ApiResponse<PlantillaResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
    }

    eliminar(id: number): Observable<ApiResponse<null>> {
        return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
    }
}
