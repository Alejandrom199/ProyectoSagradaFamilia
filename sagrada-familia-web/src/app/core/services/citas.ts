import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { CitaCreate, CitaResponse, CitaUpdate, EstadoCita } from '../../shared/interfaces/cita.interface';

@Injectable({ providedIn: 'root' })
export class CitasService {
  private readonly url = `${environment.apiUrl}/citas`;

  constructor(private http: HttpClient) { }

  // Médico — agenda del día
  obtenerMisCitasHoy(): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(`${this.url}/hoy`, { withCredentials: true });
  }

  obtenerHistorial(): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(`${this.url}/historial`, { withCredentials: true });
  }

  // Médico — citas futuras/pendientes
  obtenerProximas(): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(`${this.url}/proximas`, { withCredentials: true });
  }

  // Médico y Padre — citas de un niño específico
  obtenerPorNino(ninoId: number): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(`${this.url}/nino/${ninoId}`, { withCredentials: true });
  }

  // Padre — citas de todos sus hijos
  obtenerCitasMisHijos(): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(`${this.url}/mis-hijos`, { withCredentials: true });
  }

  // Detalle de una cita (para la pantalla de consulta)
  obtenerPorId(id: number): Observable<ApiResponse<CitaResponse>> {
    return this.http.get<ApiResponse<CitaResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crear(request: CitaCreate): Observable<ApiResponse<CitaResponse>> {
    return this.http.post<ApiResponse<CitaResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: CitaUpdate): Observable<ApiResponse<CitaResponse>> {
    return this.http.put<ApiResponse<CitaResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  cambiarEstado(id: number, nuevoEstado: EstadoCita): Observable<ApiResponse<null>> {
    return this.http.patch<ApiResponse<null>>(
      `${this.url}/${id}/estado`,
      JSON.stringify(this.estadoANumero(nuevoEstado)),
      {
        withCredentials: true,
        headers: { 'Content-Type': 'application/json' }
      }
    );
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }

  private estadoANumero(estado: EstadoCita): number {
    const mapa: Record<EstadoCita, number> = {
      [EstadoCita.Pendiente]: 1,
      [EstadoCita.Completada]: 2,
      [EstadoCita.EnCurso]: 3,
      [EstadoCita.NoAsistio]: 4,
      [EstadoCita.Cancelada]: 5,
    };
    return mapa[estado];
  }
}