import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { MedidaCreate, MedidaResponse, MedidaUpdate } from '../../shared/interfaces/medida.interface';
import { PrediccionResponse } from '../../shared/interfaces/prediccion.interface';

@Injectable({
  providedIn: 'root',
})
export class MedidasService {
  private readonly url = `${environment.apiUrl}/medidas`;
  constructor(private http: HttpClient) { }

  obtenerPorNino(ninoId: number): Observable<ApiResponse<MedidaResponse[]>> {
    return this.http.get<ApiResponse<MedidaResponse[]>>(`${this.url}/nino/${ninoId}`, { withCredentials: true });
  }

  crear(request: MedidaCreate): Observable<ApiResponse<MedidaResponse>> {
    return this.http.post<ApiResponse<MedidaResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: MedidaUpdate): Observable<ApiResponse<MedidaResponse>> {
    return this.http.put<ApiResponse<MedidaResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
}