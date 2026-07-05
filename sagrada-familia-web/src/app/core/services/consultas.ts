import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { ConsultaActualizar, ConsultaResponse } from '../../shared/interfaces/consulta.interface';

@Injectable({ providedIn: 'root' })
export class ConsultasService {
  private readonly url = `${environment.apiUrl}/consultas`;

  constructor(private http: HttpClient) { }

  actualizar(id: number, request: ConsultaActualizar): Observable<ApiResponse<ConsultaResponse>> {
    return this.http.put<ApiResponse<ConsultaResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  completar(id: number): Observable<ApiResponse<ConsultaResponse>> {
    return this.http.patch<ApiResponse<ConsultaResponse>>(`${this.url}/${id}/completar`, {}, { withCredentials: true });
  }
}
