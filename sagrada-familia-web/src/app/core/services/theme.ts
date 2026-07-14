import { Injectable, signal, effect } from '@angular/core';

const CLAVE_STORAGE = 'theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly _oscuro = signal<boolean>(this.leerEstadoInicial());
  readonly oscuro = this._oscuro.asReadonly();

  constructor() {
    // El estado inicial ya lo aplica el script inline en index.html (evita el
    // flash de tema claro antes de que arranque Angular); este effect solo
    // reacciona a cambios posteriores (toggle del usuario).
    effect(() => {
      const esOscuro = this._oscuro();
      document.documentElement.classList.toggle('dark', esOscuro);
      localStorage.setItem(CLAVE_STORAGE, esOscuro ? 'dark' : 'light');
    });
  }

  toggle(): void {
    this._oscuro.update(v => !v);
  }

  private leerEstadoInicial(): boolean {
    return document.documentElement.classList.contains('dark');
  }
}
