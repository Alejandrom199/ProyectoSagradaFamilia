import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/responses/api-response';
import { MedidaResponse } from '../../shared/interfaces/responses/medida.response';
import { CrearMedidaRequest } from '../../shared/interfaces/requests/crear-medida.request';

@Injectable({
  providedIn: 'root',
})
export class Medidas {
  private readonly url = `${environment.apiUrl}/medidas`;

  constructor(private http: HttpClient) { }

  obtenerPorNino(ninoId: number) {
    return this.http.get<ApiResponse<MedidaResponse[]>>(
      `${this.url}/nino/${ninoId}`, { withCredentials: true }
    );
  }

  crear(request: CrearMedidaRequest) {
    return this.http.post<ApiResponse<MedidaResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: Partial<CrearMedidaRequest>) {
    return this.http.put<ApiResponse<MedidaResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number) {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
}
