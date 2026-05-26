import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
import { NinoCreate, NinoDetailResponse, NinoResponse, NinoUpdate } from '../../shared/interfaces/nino.interface';

@Injectable({ providedIn: 'root' })
export class NinosService {
  private readonly url = `${environment.apiUrl}/ninos`;

  constructor(private http: HttpClient) { }

  obtenerPaginado(page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<NinoResponse>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('asc', asc);
    if (search) params = params.set('search', search);
    if (sortBy) params = params.set('sortBy', sortBy);
    return this.http.get<PagedResponse<NinoResponse>>(`${this.url}/paginado`, { params, withCredentials: true });
  }

  // Médico — sus pacientes (GET /ninos → ahora filtra por médico en el backend)
  obtenerTodos(): Observable<ApiResponse<NinoResponse[]>> {
    return this.http.get<ApiResponse<NinoResponse[]>>(this.url, { withCredentials: true });
  }

  // Padre — sus hijos
  obtenerMisNinos(): Observable<ApiResponse<NinoResponse[]>> {
    return this.http.get<ApiResponse<NinoResponse[]>>(`${this.url}/mis-ninos`, { withCredentials: true });
  }

  // Médico/Admin — hijos de un padre específico
  obtenerPorPadre(padreId: number): Observable<ApiResponse<NinoResponse[]>> {
    return this.http.get<ApiResponse<NinoResponse[]>>(`${this.url}/padre/${padreId}`, { withCredentials: true });
  }

  obtenerPorId(id: number): Observable<ApiResponse<NinoDetailResponse>> {
    return this.http.get<ApiResponse<NinoDetailResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crear(request: NinoCreate): Observable<ApiResponse<NinoDetailResponse>> {
    return this.http.post<ApiResponse<NinoDetailResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: NinoUpdate): Observable<ApiResponse<NinoDetailResponse>> {
    return this.http.put<ApiResponse<NinoDetailResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
}