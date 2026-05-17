import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { CitaCreate, CitaResponse, CitaUpdate, EstadoCita } from '../../shared/interfaces/cita.interface';

@Injectable({
  providedIn: 'root',
})
export class CitasService {
  private readonly url = `${environment.apiUrl}/citas`;

  constructor(private http: HttpClient) { }

  obtenerPorNino(ninoId: number): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(`${this.url}/nino/${ninoId}`, { withCredentials: true });
  }

  obtenerMisCitasHoy(): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(`${this.url}/hoy`, { withCredentials: true });
  }

  crear(request: CitaCreate): Observable<ApiResponse<CitaResponse>> {
    return this.http.post<ApiResponse<CitaResponse>>(this.url, request, { withCredentials: true });
  }

  cambiarEstado(id: number, nuevoEstado: EstadoCita): Observable<ApiResponse<null>> {
    return this.http.patch<ApiResponse<null>>(`${this.url}/${id}/estado`, nuevoEstado, { withCredentials: true });
  }

  /*
  // Descomentar cuando se agreguen los endpoints en CitasController.cs
  obtenerTodos(): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerPorId(id: number): Observable<ApiResponse<CitaResponse>> {
    return this.http.get<ApiResponse<CitaResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  actualizar(id: number, request: CitaUpdate): Observable<ApiResponse<CitaResponse>> {
    return this.http.put<ApiResponse<CitaResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
  */
}
