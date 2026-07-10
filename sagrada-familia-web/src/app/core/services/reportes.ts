import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface HistoriaClinicaReporteRequest {
  ninoId: number;
  titulo?: string;
  usuario?: string;
  graficaCrecimientoBase64?: string | null;
  graficaImcBase64?: string | null;
}

export interface PrediccionReporteRequest {
  ninoId: number;
  titulo?: string;
  usuario?: string;
  graficaPrediccionBase64?: string | null;
  graficaPrecisionBase64?: string | null;
}

@Injectable({
  providedIn: 'root',
})
export class Reportes {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = environment.apiUrl;

  descargarReportePdf(endpoint: string, params: any): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${endpoint}`, {
      params,
      responseType: 'blob'
    });
  }

  descargarHistoriaClinicaPdf(request: HistoriaClinicaReporteRequest): Observable<Blob> {
    return this.http.post(`${this.baseUrl}/reportes/historia-clinica-pdf`, request, {
      responseType: 'blob'
    });
  }

  descargarPrediccionPdf(request: PrediccionReporteRequest): Observable<Blob> {
    return this.http.post(`${this.baseUrl}/reportes/prediccion-pdf`, request, {
      responseType: 'blob'
    });
  }
}
