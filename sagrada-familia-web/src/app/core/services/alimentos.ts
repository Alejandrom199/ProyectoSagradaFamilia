import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/responses/api-response';
import { AlimentoResponse, CategoriaResponse } from '../../shared/interfaces/responses/alimento.response';

@Injectable({
  providedIn: 'root',
})
export class Alimentos {
  private readonly url = `${environment.apiUrl}/alimentos`;

  constructor(private http: HttpClient) { }

  obtenerTodos() {
    return this.http.get<ApiResponse<AlimentoResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerPorEdad(edadMeses: number) {
    return this.http.get<ApiResponse<AlimentoResponse[]>>(
      `${this.url}/por-edad/${edadMeses}`, { withCredentials: true }
    );
  }

  obtenerCategorias() {
    return this.http.get<ApiResponse<CategoriaResponse[]>>(
      `${this.url}/categorias`, { withCredentials: true }
    );
  }

  crear(request: object) {
    return this.http.post<ApiResponse<AlimentoResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: object) {
    return this.http.put<ApiResponse<AlimentoResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number) {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crearCategoria(request: object) {
    return this.http.post<ApiResponse<CategoriaResponse>>(
      `${this.url}/categorias`, request, { withCredentials: true }
    );
  }
}
