import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { ParametroCreate, ParametroResponse, ParametroUpdate } from '../../shared/interfaces/parametro.interface';

@Injectable({ providedIn: 'root' })
export class ParametrosService {
    private readonly url = `${environment.apiUrl}/parametros`;

    constructor(private http: HttpClient) { }

    obtenerTodos(): Observable<ApiResponse<ParametroResponse[]>> {
        return this.http.get<ApiResponse<ParametroResponse[]>>(this.url, { withCredentials: true });
    }

    obtenerPorGrupo(grupo: string): Observable<ApiResponse<ParametroResponse[]>> {
        return this.http.get<ApiResponse<ParametroResponse[]>>(`${this.url}/grupo/${grupo}`, { withCredentials: true });
    }

    obtenerPorGrupoYCodigo(grupo: string, codigo: string): Observable<ApiResponse<ParametroResponse | null>> {
        return this.http.get<ApiResponse<ParametroResponse | null>>(
            `${this.url}/grupo/${grupo}/codigo/${codigo}`,
            { withCredentials: true }
        );
    }

    crear(request: ParametroCreate): Observable<ApiResponse<ParametroResponse>> {
        return this.http.post<ApiResponse<ParametroResponse>>(this.url, request, { withCredentials: true });
    }

    actualizar(id: number, request: ParametroUpdate): Observable<ApiResponse<ParametroResponse>> {
        return this.http.put<ApiResponse<ParametroResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
    }

    eliminar(id: number): Observable<ApiResponse<null>> {
        return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
    }
}
