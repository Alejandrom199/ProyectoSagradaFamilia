import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class BuildInfoService {
  private http = inject(HttpClient);
  readonly sha = signal<string | null>(null);

  constructor() {
    this.http.get<{ sha: string }>('/assets/build-info.json').subscribe({
      next: (r) => { if (r.sha && r.sha !== 'dev') this.sha.set(r.sha); },
      error: () => {}
    });
  }
}
