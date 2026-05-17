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

  obtenerAuditoriaPorTabla(nombreTabla: string, pk: string): Observable<ApiResponse<AuditoriaResponse[]>> {
    const params = new HttpParams().set('pk', pk);
    return this.http.get<ApiResponse<AuditoriaResponse[]>>(
      `${this.url}/auditoria/tabla/${nombreTabla}`,
      { params, withCredentials: true }
    );
  }

  obtenerErroresRecientes(): Observable<ApiResponse<LogSistemaResponse[]>> {
    return this.http.get<ApiResponse<LogSistemaResponse[]>>(
      `${this.url}/logs/errores`, { withCredentials: true }
    );
  }
}
