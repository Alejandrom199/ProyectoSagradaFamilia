import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
import {
  UsuarioCreate,
  UsuarioDetailResponse,
  UsuarioResponse,
  UsuarioUpdate
} from '../../shared/interfaces/usuario.interface';
import { ImportResult } from '../../shared/interfaces/import.interface';

@Injectable({
  providedIn: 'root',
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

  obtenerPerfilActual(): Observable<ApiResponse<UsuarioDetailResponse>> {
    return this.http.get<ApiResponse<UsuarioDetailResponse>>(`${this.url}/perfil`, { withCredentials: true });
  }

  crearAdmin(request: UsuarioCreate): Observable<ApiResponse<UsuarioDetailResponse>> {
    return this.http.post<ApiResponse<UsuarioDetailResponse>>(this.url, request, { withCredentials: true });
  }

  obtenerPaginado(page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<UsuarioResponse>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('asc', asc);
    if (search) params = params.set('search', search);
    if (sortBy) params = params.set('sortBy', sortBy);
    return this.http.get<PagedResponse<UsuarioResponse>>(`${this.url}/paginado`, { params, withCredentials: true });
  }

  actualizarEstado(id: number, activo: boolean): Observable<ApiResponse<null>> {
    return this.http.patch<ApiResponse<null>>(`${this.url}/${id}/estado`, activo, { withCredentials: true });
  }

  exportarExcel(): Observable<Blob> {
    return this.http.get(`${this.url}/exportar`, { responseType: 'blob', withCredentials: true });
  }

  descargarPlantilla(): Observable<Blob> {
    return this.http.get(`${this.url}/plantilla`, { responseType: 'blob', withCredentials: true });
  }

  importar(archivo: File): Observable<ApiResponse<ImportResult>> {
    const formData = new FormData();
    formData.append('archivo', archivo);
    return this.http.post<ApiResponse<ImportResult>>(`${this.url}/importar`, formData, { withCredentials: true });
  }

  /*
  // Descomentar cuando se agregue el endpoint [HttpPut("{id:int}")] en UsuariosController.cs
  actualizar(id: number, request: UsuarioUpdate): Observable<ApiResponse<UsuarioDetailResponse>> {
    return this.http.put<ApiResponse<UsuarioDetailResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }
  */
}
