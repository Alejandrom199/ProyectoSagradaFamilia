import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import {
  PrescripcionCreate,
  PrescripcionResponse,
  PrescripcionUpdate
} from '../../shared/interfaces/prescripcion.interface';

@Injectable({
  providedIn: 'root',
})
export class PrescripcionesService {
  private readonly url = `${environment.apiUrl}/prescripciones`;

  constructor(private http: HttpClient) { }

  obtenerHistorialPorNino(ninoId: number): Observable<ApiResponse<PrescripcionResponse[]>> {
    return this.http.get<ApiResponse<PrescripcionResponse[]>>(
      `${this.url}/nino/${ninoId}`, { withCredentials: true }
    );
  }

  crear(request: PrescripcionCreate): Observable<ApiResponse<PrescripcionResponse>> {
    return this.http.post<ApiResponse<PrescripcionResponse>>(this.url, request, { withCredentials: true });
  }

  /*
  // Descomentar cuando se agreguen los endpoints en PrescripcionesController.cs
  obtenerPorId(id: number): Observable<ApiResponse<PrescripcionResponse>> {
    return this.http.get<ApiResponse<PrescripcionResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  actualizar(id: number, request: PrescripcionUpdate): Observable<ApiResponse<PrescripcionResponse>> {
    return this.http.put<ApiResponse<PrescripcionResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }
  */
}
