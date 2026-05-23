import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { AuditoriaResponse, LogSistemaResponse } from '../../shared/interfaces/sistema.interface';

@Injectable({
  providedIn: 'root',
})
export class SistemaService {
  private readonly url = `${environment.apiUrl}/sistema`;

  constructor(private http: HttpClient) { }

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
}
