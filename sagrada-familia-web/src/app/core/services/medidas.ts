import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
import { MedidaCreate, MedidaResponse, MedidaUpdate } from '../../shared/interfaces/medida.interface';
import { ImportResult } from '../../shared/interfaces/import.interface';

@Injectable({
  providedIn: 'root',
})
export class MedidasService {
  private readonly url = `${environment.apiUrl}/medidas`;

  constructor(private http: HttpClient) { }

  obtenerPaginadoPorNino(ninoId: number, page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<MedidaResponse>> {
    const params = new HttpParams()
      .set('page', page).set('pageSize', pageSize)
      .set('search', search).set('sortBy', sortBy).set('asc', asc);
    return this.http.get<PagedResponse<MedidaResponse>>(`${this.url}/nino/${ninoId}/paginado`, { params, withCredentials: true });
  }

  obtenerPorNino(ninoId: number): Observable<ApiResponse<MedidaResponse[]>> {
    return this.http.get<ApiResponse<MedidaResponse[]>>(`${this.url}/nino/${ninoId}`, { withCredentials: true });
  }

  crear(request: MedidaCreate): Observable<ApiResponse<MedidaResponse>> {
    return this.http.post<ApiResponse<MedidaResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: MedidaUpdate): Observable<ApiResponse<MedidaResponse>> {
    return this.http.put<ApiResponse<MedidaResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
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