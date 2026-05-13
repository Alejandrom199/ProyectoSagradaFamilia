import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { PadreCreate, PadreResponse, PadreUpdate } from '../../shared/interfaces/padre.interface';
import { ApiResponse } from '../../shared/interfaces/api.interface';

@Injectable({
  providedIn: 'root',
})
export class PadresService {
  private readonly url = `${environment.apiUrl}/padre`;

  constructor(private http: HttpClient) { }

  obtenerTodos() {
    return this.http.get<ApiResponse<PadreResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerPorId(id: number) {
    return this.http.get<ApiResponse<PadreResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crear(request: PadreCreate) {
    return this.http.post<ApiResponse<PadreResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: PadreUpdate) {
    return this.http.put<ApiResponse<PadreResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number) {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
}
