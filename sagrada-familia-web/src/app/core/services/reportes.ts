import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

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
}
