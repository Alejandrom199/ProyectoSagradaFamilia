import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { ActualizarPermisoRolRequest, ModuloPermisoResponse } from '../../shared/interfaces/permiso.interface';

@Injectable({
  providedIn: 'root',
})
export class PermisosService {
  private readonly url = `${environment.apiUrl}/menu/permisos/rol`;

  constructor(private http: HttpClient) { }

  obtenerPermisosRol(rolId: number): Observable<ApiResponse<ModuloPermisoResponse[]>> {
    return this.http.get<ApiResponse<ModuloPermisoResponse[]>>(`${this.url}/${rolId}`, { withCredentials: true });
  }

  actualizarPermiso(rolId: number, request: ActualizarPermisoRolRequest): Observable<ApiResponse<null>> {
    return this.http.patch<ApiResponse<null>>(`${this.url}/${rolId}`, request, { withCredentials: true });
  }
}
