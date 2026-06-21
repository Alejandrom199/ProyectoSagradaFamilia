import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { CurvasOmsResponse, PrediccionHealth, PrediccionResponse } from '../../shared/interfaces/prediccion.interface';

@Injectable({
  providedIn: 'root',
})
export class Predicciones {
  private readonly url = `${environment.apiUrl}/predicciones`;

  constructor(private http: HttpClient) { }

  obtenerPorNino(ninoId: number) {
    return this.http.get<ApiResponse<PrediccionResponse>>(
      `${this.url}/nino/${ninoId}`, { withCredentials: true }
    );
  }

  obtenerCurvasOms(ninoId: number) {
    return this.http.get<ApiResponse<CurvasOmsResponse>>(
      `${this.url}/nino/${ninoId}/curvas-oms`, { withCredentials: true }
    );
  }

  obtenerEstadoServicio() {
    return this.http.get<ApiResponse<PrediccionHealth>>(`${this.url}/health`, { withCredentials: true })
  }

  obtenerConteo() {
    return this.http.get<ApiResponse<number>>(`${this.url}/count`, { withCredentials: true });
  }

}
