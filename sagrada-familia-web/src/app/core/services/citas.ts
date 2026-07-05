import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
import { CambiarEstadoRequest, CitaCreate, CitaResponse, CitaUpdate, EstadoCita } from '../../shared/interfaces/cita.interface';
import { ConsultaResponse } from '../../shared/interfaces/consulta.interface';

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

  obtenerPaginadoPorNino(ninoId: number, page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<CitaResponse>> {
    const params = new HttpParams()
      .set('page', page).set('pageSize', pageSize)
      .set('search', search).set('sortBy', sortBy).set('asc', asc);
    return this.http.get<PagedResponse<CitaResponse>>(`${this.url}/nino/${ninoId}/paginado`, { params, withCredentials: true });
  }

  // Médico y Padre — citas de un niño específico
  obtenerPorNino(ninoId: number): Observable<ApiResponse<CitaResponse[]>> {
    return this.http.get<ApiResponse<CitaResponse[]>>(`${this.url}/nino/${ninoId}`, { withCredentials: true });
  }

  exportarExcelPorNino(ninoId: number): Observable<Blob> {
    return this.http.get(`${this.url}/nino/${ninoId}/exportar`, { responseType: 'blob', withCredentials: true });
  }

  exportarExcelHistorial(): Observable<Blob> {
    return this.http.get(`${this.url}/historial/exportar`, { responseType: 'blob', withCredentials: true });
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

  cambiarEstado(id: number, nuevoEstado: EstadoCita, motivoCancelacion?: string): Observable<ApiResponse<null>> {
    const body: CambiarEstadoRequest = {
      estado: this.estadoANumero(nuevoEstado),
      motivoCancelacion: motivoCancelacion || undefined
    };
    return this.http.patch<ApiResponse<null>>(`${this.url}/${id}/estado`, body, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }

  iniciarConsulta(citaId: number): Observable<ApiResponse<ConsultaResponse>> {
    return this.http.patch<ApiResponse<ConsultaResponse>>(`${this.url}/${citaId}/iniciar-consulta`, {}, { withCredentials: true });
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