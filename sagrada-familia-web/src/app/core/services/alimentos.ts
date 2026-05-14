import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import {
  AlimentoResponse,
  CategoriaResponse,
  AlimentoCreate,
  AlimentoUpdate,
  CategoriaCreate
} from '../../shared/interfaces/alimento.interface';

@Injectable({ providedIn: 'root' })
export class AlimentosService {
  private readonly url = `${environment.apiUrl}/alimentos`;

  constructor(private http: HttpClient) { }

  obtenerTodos(): Observable<ApiResponse<AlimentoResponse[]>> {
    return this.http.get<ApiResponse<AlimentoResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerPorEdad(edadMeses: number): Observable<ApiResponse<AlimentoResponse[]>> {
    return this.http.get<ApiResponse<AlimentoResponse[]>>(
      `${this.url}/por-edad/${edadMeses}`, { withCredentials: true }
    );
  }

  obtenerCategorias(): Observable<ApiResponse<CategoriaResponse[]>> {
    return this.http.get<ApiResponse<CategoriaResponse[]>>(`${this.url}/categorias`, { withCredentials: true });
  }

  crear(request: AlimentoCreate): Observable<ApiResponse<AlimentoResponse>> {
    return this.http.post<ApiResponse<AlimentoResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: AlimentoUpdate): Observable<ApiResponse<AlimentoResponse>> {
    return this.http.put<ApiResponse<AlimentoResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crearCategoria(request: CategoriaCreate): Observable<ApiResponse<CategoriaResponse>> {
    return this.http.post<ApiResponse<CategoriaResponse>>(
      `${this.url}/categorias`, request, { withCredentials: true }
    );
  }
}