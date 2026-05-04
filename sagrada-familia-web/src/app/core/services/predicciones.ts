import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/responses/api-response';
import { PrediccionHealth, PrediccionResponse } from '../../shared/interfaces/responses/prediccion.response';

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

  obtenerEstadoServicio() {
    return this.http.get<ApiResponse<PrediccionHealth>>(`${this.url}/health`, { withCredentials: true })
  }

}
