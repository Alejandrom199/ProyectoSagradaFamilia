import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ServerTimeService {
  private offsetMs = 0;

  readonly now = signal<Date>(new Date());

  constructor() {
    setInterval(() => this.now.set(new Date(Date.now() + this.offsetMs)), 1000);
  }

  registrarFechaServidor(dateHeader: string): void {
    const fechaServidor = new Date(dateHeader);
    if (isNaN(fechaServidor.getTime())) return;

    this.offsetMs = fechaServidor.getTime() - Date.now();
    this.now.set(new Date(Date.now() + this.offsetMs));
  }
}
