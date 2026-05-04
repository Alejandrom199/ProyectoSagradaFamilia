import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Auth } from './auth';
import { Router } from '@angular/router';
import { MenuResponse } from '../../shared/interfaces/responses/menu.response';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../../shared/interfaces/responses/api-response';

@Injectable({
  providedIn: 'root',
})
export class Menu {
  private readonly url = `${environment.apiUrl}/menu`;
  private menu = signal<MenuResponse[]>([]);

  constructor(private http: HttpClient, private auth: Auth, private router: Router) { }

  obtenerMenu() {
    return this.http.get<ApiResponse<MenuResponse[]>>(this.url, { withCredentials: true });
  }

  private cargarMenu() {
    this.obtenerMenu().subscribe({
      next: (response) => {
        if (response.success) this.menu.set(response.data);
      },
      error: () => {
        // Si falla el menú, no romper el layout
        // Usar menú estático de fallback
        this.menu.set(this.menuFallback());
      }
    });
  }

  private menuFallback(): any[] {
    const esMedico = this.auth.esMedico();
    return [
      {
        id: 1,
        nombre: 'Principal',
        icono: 'home',
        orden: 1,
        opciones: [
          { id: 1, nombre: 'Dashboard', ruta: '/dashboard', icono: 'home', orden: 1, acciones: ['Ver'] },
          { id: 2, nombre: 'Pacientes', ruta: '/pacientes', icono: 'users', orden: 2, acciones: ['Ver'] },
          ...(esMedico ? [
            { id: 3, nombre: 'Padres', ruta: '/padres', icono: 'users', orden: 3, acciones: ['Ver'] },
            { id: 4, nombre: 'Alimentos', ruta: '/alimentos', icono: 'food', orden: 4, acciones: ['Ver'] },
          ] : []),
          { id: 5, nombre: 'Predicciones', ruta: '/predicciones', icono: 'chart-line', orden: 5, acciones: ['Ver'] },
        ]
      }
    ];
  }
}
