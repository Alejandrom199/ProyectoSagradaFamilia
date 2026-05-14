import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { NinoResponse, NinoDetailResponse, NinoCreate, NinoUpdate } from '../../shared/interfaces/nino.interface';

@Injectable({ providedIn: 'root' })
export class NinosService {
  private readonly url = `${environment.apiUrl}/ninos`;

  constructor(private http: HttpClient) { }

  obtenerTodos(): Observable<ApiResponse<NinoResponse[]>> {
    return this.http.get<ApiResponse<NinoResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerMisNinos(): Observable<ApiResponse<NinoResponse[]>> {
    return this.http.get<ApiResponse<NinoResponse[]>>(`${this.url}/mis-ninos`, { withCredentials: true });
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