import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PadreCambiarEmail, PadreCreate, PadreDetailResponse, PadreResponse, PadreUpdate } from '../../shared/interfaces/padre.interface';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
import { ImportResult } from '../../shared/interfaces/import.interface';

@Injectable({
  providedIn: 'root',
})
export class PadresService {
  private readonly url = `${environment.apiUrl}/padre`;

  constructor(private http: HttpClient) { }

  obtenerTodos(): Observable<ApiResponse<PadreResponse[]>> {
    return this.http.get<ApiResponse<PadreResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerPaginado(page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<PadreResponse>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize).set('asc', asc);
    if (search) params = params.set('search', search);
    if (sortBy) params = params.set('sortBy', sortBy);
    return this.http.get<PagedResponse<PadreResponse>>(`${this.url}/paginado`, { params, withCredentials: true });
  }

  crear(request: PadreCreate): Observable<ApiResponse<PadreDetailResponse>> {
    return this.http.post<ApiResponse<PadreDetailResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: PadreUpdate): Observable<ApiResponse<PadreDetailResponse>> {
    return this.http.put<ApiResponse<PadreDetailResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }

  obtenerPorId(id: number): Observable<ApiResponse<PadreDetailResponse>> {
    return this.http.get<ApiResponse<PadreDetailResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  cambiarEmail(id: number, request: PadreCambiarEmail): Observable<ApiResponse<null>> {
    return this.http.patch<ApiResponse<null>>(`${this.url}/${id}/email`, request, { withCredentials: true });
  }

  restablecerPassword(id: number): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(`${this.url}/${id}/reset-password`, {}, { withCredentials: true });
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
}