import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import {
  MedicoCreate,
  MedicoDetailResponse,
  MedicoResponse,
  MedicoUpdate
} from '../../shared/interfaces/medico.interface';

@Injectable({
  providedIn: 'root',
})
export class MedicosService {
  private readonly url = `${environment.apiUrl}/medico`;

  constructor(private http: HttpClient) { }

  obtenerTodos(): Observable<ApiResponse<MedicoResponse[]>> {
    return this.http.get<ApiResponse<MedicoResponse[]>>(this.url, { withCredentials: true });
  }

  crear(request: MedicoCreate): Observable<ApiResponse<MedicoDetailResponse>> {
    return this.http.post<ApiResponse<MedicoDetailResponse>>(this.url, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }

  /*
  // Descomentar cuando se agreguen los endpoints [HttpGet("{id:int}")] y [HttpPut("{id:int}")] en MedicoController.cs
  obtenerPorId(id: number): Observable<ApiResponse<MedicoDetailResponse>> {
    return this.http.get<ApiResponse<MedicoDetailResponse>>(`${this.url}/${id}`, { withCredentials: true });
  }

  actualizar(id: number, request: MedicoUpdate): Observable<ApiResponse<MedicoDetailResponse>> {
    return this.http.put<ApiResponse<MedicoDetailResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }
  */
}
