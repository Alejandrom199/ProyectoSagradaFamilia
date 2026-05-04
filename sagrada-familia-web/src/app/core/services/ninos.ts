import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/responses/api-response';
import { NinoResponse } from '../../shared/interfaces/responses/nino.response';
import { ActualizarNinoRequest, CrearNinoRequest } from '../../shared/interfaces/requests/crear-nino.request';

@Injectable({
  providedIn: 'root',
})
export class Ninos {
  private readonly url = `${environment.apiUrl}/ninos`;

  constructor(private http: HttpClient) { }

  obtenerTodos() {
    return this.http.get<ApiResponse<NinoResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerMisNinos() {
    return this.http.get<ApiResponse<NinoResponse[]>>(`${this.url}/mis-ninos`, { withCredentials: true });
  }

  obtenerPorId(id: number) {
    return this.http.get<ApiResponse<NinoResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crear(request: CrearNinoRequest) {
    return this.http.post<ApiResponse<NinoResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: ActualizarNinoRequest) {
    return this.http.put<ApiResponse<NinoResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number) {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
}
