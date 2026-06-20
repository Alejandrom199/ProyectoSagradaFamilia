import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
import { AuditoriaResponse, DashboardAdminResponse, LogSistemaResponse } from '../../shared/interfaces/sistema.interface';

@Injectable({
  providedIn: 'root',
})
export class SistemaService {
  private readonly url = `${environment.apiUrl}/sistema`;

  constructor(private http: HttpClient) { }

  obtenerAuditoriaPaginado(page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<AuditoriaResponse>> {
    const params = new HttpParams()
      .set('page', page).set('pageSize', pageSize)
      .set('search', search).set('sortBy', sortBy).set('asc', asc);
    return this.http.get<PagedResponse<AuditoriaResponse>>(`${this.url}/auditoria/paginado`, { params, withCredentials: true });
  }

  obtenerLogsPaginado(page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<LogSistemaResponse>> {
    const params = new HttpParams()
      .set('page', page).set('pageSize', pageSize)
      .set('search', search).set('sortBy', sortBy).set('asc', asc);
    return this.http.get<PagedResponse<LogSistemaResponse>>(`${this.url}/logs/paginado`, { params, withCredentials: true });
  }

  obtenerAuditoriaReciente(top = 100): Observable<ApiResponse<AuditoriaResponse[]>> {
    const params = new HttpParams().set('top', top);
    return this.http.get<ApiResponse<AuditoriaResponse[]>>(
      `${this.url}/auditoria`, { params, withCredentials: true }
    );
  }

  obtenerMiActividad(): Observable<ApiResponse<AuditoriaResponse[]>> {
    return this.http.get<ApiResponse<AuditoriaResponse[]>>(
      `${this.url}/auditoria/mia`, { withCredentials: true }
    );
  }

  obtenerAuditoriaPorTabla(nombreTabla: string, pk?: string): Observable<ApiResponse<AuditoriaResponse[]>> {
    let params = new HttpParams();
    if (pk?.trim()) params = params.set('pk', pk.trim());
    return this.http.get<ApiResponse<AuditoriaResponse[]>>(
      `${this.url}/auditoria/tabla/${nombreTabla}`,
      { params, withCredentials: true }
    );
  }

  obtenerLogsRecientes(top = 100): Observable<ApiResponse<LogSistemaResponse[]>> {
    const params = new HttpParams().set('top', top);
    return this.http.get<ApiResponse<LogSistemaResponse[]>>(
      `${this.url}/logs`, { params, withCredentials: true }
    );
  }

  obtenerErroresRecientes(): Observable<ApiResponse<LogSistemaResponse[]>> {
    return this.http.get<ApiResponse<LogSistemaResponse[]>>(
      `${this.url}/logs/errores`, { withCredentials: true }
    );
  }

  obtenerDashboardAdmin(): Observable<ApiResponse<DashboardAdminResponse>> {
    return this.http.get<ApiResponse<DashboardAdminResponse>>(
      `${this.url}/dashboard`, { withCredentials: true }
    );
  }
}
