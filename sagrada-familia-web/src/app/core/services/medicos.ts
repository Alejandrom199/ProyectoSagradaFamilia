import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
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

  obtenerPaginado(page: number, pageSize: number, search: string, sortBy: string, asc: boolean): Observable<PagedResponse<MedicoResponse>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize).set('asc', asc);
    if (search) params = params.set('search', search);
    if (sortBy) params = params.set('sortBy', sortBy);
    return this.http.get<PagedResponse<MedicoResponse>>(`${this.url}/paginado`, { params, withCredentials: true });
  }

  crear(request: MedicoCreate): Observable<ApiResponse<MedicoDetailResponse>> {
    return this.http.post<ApiResponse<MedicoDetailResponse>>(this.url, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }

  restablecerPassword(id: number): Observable<ApiResponse<null>> {
    return this.http.post<ApiResponse<null>>(`${this.url}/${id}/reset-password`, {}, { withCredentials: true });
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
