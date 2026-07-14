import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { environment } from '../../../environments/environment';

interface VersionDto {
  version: string;
}

// Servicio raíz (singleton): se instancia una sola vez en toda la app, por lo
// que la llamada HTTP del constructor se ejecuta una única vez y su resultado
// queda cacheado en el signal `version` para todos los componentes que lo inyecten.
@Injectable({ providedIn: 'root' })
export class AppVersionService {
  private http = inject(HttpClient);
  readonly version = signal<string | null>(null);

  constructor() {
    this.http.get<ApiResponse<VersionDto>>(`${environment.apiUrl}/sistema/version`).subscribe({
      next: (r) => { if (r.success && r.data?.version) this.version.set(r.data.version); },
      error: () => {}
    });
  }
}
