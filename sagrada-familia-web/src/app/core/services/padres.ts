import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PadreCreate, PadreDetailResponse, PadreResponse, PadreUpdate } from '../../shared/interfaces/padre.interface';
import { ApiResponse } from '../../shared/interfaces/api.interface';

@Injectable({
  providedIn: 'root',
})
export class PadresService {
  private readonly url = `${environment.apiUrl}/padre`;

  constructor(private http: HttpClient) { }

  obtenerTodos(): Observable<ApiResponse<PadreResponse[]>> {
    return this.http.get<ApiResponse<PadreResponse[]>>(this.url, { withCredentials: true });
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
}