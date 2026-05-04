import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/responses/api-response';
import { PadreResponse } from '../../shared/interfaces/responses/padre.response';
import { CrearPadreRequest } from '../../shared/interfaces/requests/crear-padre.request';

@Injectable({
  providedIn: 'root',
})
export class Padres {
  private readonly url = `${environment.apiUrl}/auth/padres`;

  constructor(private http: HttpClient) { }

  obtenerTodos() {
    return this.http.get<ApiResponse<PadreResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerPorId(id: number) {
    return this.http.get<ApiResponse<PadreResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crear(request: CrearPadreRequest) {
    return this.http.post<ApiResponse<PadreResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: Partial<CrearPadreRequest>) {
    return this.http.put<ApiResponse<PadreResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number) {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
}
