import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import {
  UsuarioCreate,
  UsuarioDetailResponse,
  UsuarioResponse,
  UsuarioUpdate
} from '../../shared/interfaces/usuario.interface';

@Injectable({
  providedIn: 'root'
})
export class UsuariosService {
  private readonly url = `${environment.apiUrl}/usuarios`;

  constructor(private http: HttpClient) { }

  obtenerTodos(): Observable<ApiResponse<UsuarioResponse[]>> {
    return this.http.get<ApiResponse<UsuarioResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerPorId(id: number): Observable<ApiResponse<UsuarioDetailResponse>> {
    return this.http.get<ApiResponse<UsuarioDetailResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crearAdmin(request: UsuarioCreate): Observable<ApiResponse<UsuarioDetailResponse>> {
    return this.http.post<ApiResponse<UsuarioDetailResponse>>(this.url, request, { withCredentials: true });
  }

  actualizarEstado(id: number, activo: boolean): Observable<ApiResponse<null>> {
    // Se envía el booleano directamente al body tal como lo espera el [FromBody] en C#
    return this.http.patch<ApiResponse<null>>(`${this.url}/${id}/estado`, activo, { withCredentials: true });
  }

  obtenerPerfilActual(): Observable<ApiResponse<UsuarioDetailResponse>> {
    return this.http.get<ApiResponse<UsuarioDetailResponse>>(`${this.url}/perfil`, { withCredentials: true });
  }

  /*
  actualizar(id: number, request: UsuarioUpdate): Observable<ApiResponse<UsuarioDetailResponse>> {
    return this.http.put<ApiResponse<UsuarioDetailResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }
  */
}