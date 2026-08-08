import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { CatalogoValorCreate, CatalogoValorResponse, CatalogoValorUpdate } from '../../shared/interfaces/catalogo-valor.interface';

@Injectable({ providedIn: 'root' })
export class CatalogoValoresService {
    private readonly url = `${environment.apiUrl}/catalogovalores`;

    constructor(private http: HttpClient) { }

    obtenerTodos(): Observable<ApiResponse<CatalogoValorResponse[]>> {
        return this.http.get<ApiResponse<CatalogoValorResponse[]>>(this.url, { withCredentials: true });
    }

    obtenerPorTipo(tipo: string): Observable<ApiResponse<CatalogoValorResponse[]>> {
        return this.http.get<ApiResponse<CatalogoValorResponse[]>>(`${this.url}/tipo/${tipo}`, { withCredentials: true });
    }

    obtenerPorTipoYCodigo(tipo: string, codigo: string): Observable<ApiResponse<CatalogoValorResponse | null>> {
        return this.http.get<ApiResponse<CatalogoValorResponse | null>>(
            `${this.url}/tipo/${tipo}/codigo/${codigo}`,
            { withCredentials: true }
        );
    }

    crear(request: CatalogoValorCreate): Observable<ApiResponse<CatalogoValorResponse>> {
        return this.http.post<ApiResponse<CatalogoValorResponse>>(this.url, request, { withCredentials: true });
    }

    actualizar(id: number, request: CatalogoValorUpdate): Observable<ApiResponse<CatalogoValorResponse>> {
        return this.http.put<ApiResponse<CatalogoValorResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
    }

    eliminar(id: number): Observable<ApiResponse<null>> {
        return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
    }
}
