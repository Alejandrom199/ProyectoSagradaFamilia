import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
import { PrescripcionCreate, PrescripcionResponse, PrescripcionUpdate } from '../../shared/interfaces/prescripcion.interface';

@Injectable({ providedIn: 'root' })
export class PrescripcionesService {
  private readonly url = `${environment.apiUrl}/prescripciones`;

  constructor(private http: HttpClient) { }

  obtenerPaginadoPorNino(ninoId: number, page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<PrescripcionResponse>> {
    const params = new HttpParams()
      .set('page', page).set('pageSize', pageSize)
      .set('search', search).set('sortBy', sortBy).set('asc', asc);
    return this.http.get<PagedResponse<PrescripcionResponse>>(`${this.url}/nino/${ninoId}/paginado`, { params, withCredentials: true });
  }

  // Historial de un niño (médico y padre)
  obtenerHistorialPorNino(ninoId: number): Observable<ApiResponse<PrescripcionResponse[]>> {
    return this.http.get<ApiResponse<PrescripcionResponse[]>>(`${this.url}/nino/${ninoId}`, { withCredentials: true });
  }

  // Prescripciones emitidas por el médico logueado
  obtenerMisPrescripciones(): Observable<ApiResponse<PrescripcionResponse[]>> {
    return this.http.get<ApiResponse<PrescripcionResponse[]>>(`${this.url}/mis-prescripciones`, { withCredentials: true });
  }

  exportarExcelPorNino(ninoId: number): Observable<Blob> {
    return this.http.get(`${this.url}/nino/${ninoId}/exportar`, { responseType: 'blob', withCredentials: true });
  }

  exportarExcelMisPrescripciones(): Observable<Blob> {
    return this.http.get(`${this.url}/mis-prescripciones/exportar`, { responseType: 'blob', withCredentials: true });
  }

  // Detalle de una prescripción
  obtenerPorId(id: number): Observable<ApiResponse<PrescripcionResponse>> {
    return this.http.get<ApiResponse<PrescripcionResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  // Crear desde una consulta (requiere consultaId)
  crear(request: PrescripcionCreate): Observable<ApiResponse<PrescripcionResponse>> {
    return this.http.post<ApiResponse<PrescripcionResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: PrescripcionUpdate): Observable<ApiResponse<PrescripcionResponse>> {
    return this.http.put<ApiResponse<PrescripcionResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
}