import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResponse } from '../../shared/interfaces/api.interface';
import {
  AlimentoResponse,
  CategoriaResponse,
  AlimentoCreate,
  AlimentoUpdate,
  CategoriaCreate,
  ImportResultado,
  EstadoAlimento,
  AlimentoFiltroColumnas,
} from '../../shared/interfaces/alimento.interface';

@Injectable({ providedIn: 'root' })
export class AlimentosService {
  private readonly url = `${environment.apiUrl}/alimentos`;

  constructor(private http: HttpClient) { }

  obtenerTodos(): Observable<ApiResponse<AlimentoResponse[]>> {
    return this.http.get<ApiResponse<AlimentoResponse[]>>(this.url, { withCredentials: true });
  }

  obtenerPaginado(
    page: number,
    pageSize: number,
    search: string,
    sortBy: string,
    ascending: boolean,
    columnFilters: Record<string, string[]> = {}
  ): Observable<PagedResponse<AlimentoResponse>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize)
      .set('asc', ascending);
    if (search) params = params.set('search', search);
    if (sortBy) params = params.set('sortBy', sortBy);

    const categoriasSeleccionadas = columnFilters[AlimentoFiltroColumnas.categoria] ?? [];
    categoriasSeleccionadas.forEach(categoria => params = params.append('categorias', categoria));

    const estadosSeleccionados = columnFilters[AlimentoFiltroColumnas.estado] ?? [];
    const soloUnEstadoSeleccionado = estadosSeleccionados.length === 1;
    if (soloUnEstadoSeleccionado) {
      const estaActivo = estadosSeleccionados[0] === EstadoAlimento.Activo;
      params = params.set('activo', String(estaActivo));
    }

    const edadesSeleccionadas = columnFilters[AlimentoFiltroColumnas.edadMinima] ?? [];
    edadesSeleccionadas.forEach(edad => params = params.append('edades', edad));

    return this.http.get<PagedResponse<AlimentoResponse>>(
      `${this.url}/paginado`, { params, withCredentials: true }
    );
  }

  obtenerPorEdad(edadMeses: number): Observable<ApiResponse<AlimentoResponse[]>> {
    return this.http.get<ApiResponse<AlimentoResponse[]>>(
      `${this.url}/por-edad/${edadMeses}`, { withCredentials: true }
    );
  }

  obtenerCategorias(): Observable<ApiResponse<CategoriaResponse[]>> {
    return this.http.get<ApiResponse<CategoriaResponse[]>>(`${this.url}/categorias`, { withCredentials: true });
  }

  crear(request: AlimentoCreate): Observable<ApiResponse<AlimentoResponse>> {
    return this.http.post<ApiResponse<AlimentoResponse>>(this.url, request, { withCredentials: true });
  }

  actualizar(id: number, request: AlimentoUpdate): Observable<ApiResponse<AlimentoResponse>> {
    return this.http.put<ApiResponse<AlimentoResponse>>(`${this.url}/${id}`, request, { withCredentials: true });
  }

  eliminar(id: number): Observable<ApiResponse<null>> {
    return this.http.delete<ApiResponse<null>>(`${this.url}/${id}`, { withCredentials: true });
  }

  crearCategoria(request: CategoriaCreate): Observable<ApiResponse<CategoriaResponse>> {
    return this.http.post<ApiResponse<CategoriaResponse>>(
      `${this.url}/categorias`, request, { withCredentials: true }
    );
  }

  exportarExcel(): Observable<Blob> {
    return this.http.get(`${this.url}/exportar`, {
      responseType: 'blob',
      withCredentials: true
    });
  }

  descargarPlantilla(): Observable<Blob> {
    return this.http.get(`${this.url}/plantilla`, {
      responseType: 'blob',
      withCredentials: true
    });
  }

  importar(archivo: File): Observable<ApiResponse<ImportResultado>> {
    const formData = new FormData();
    formData.append('archivo', archivo);
    return this.http.post<ApiResponse<ImportResultado>>(
      `${this.url}/importar`, formData, { withCredentials: true }
    );
  }
}