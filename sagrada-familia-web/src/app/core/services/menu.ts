import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MenuResponse } from '../../shared/interfaces/menu.interface';
import { ApiResponse } from '../../shared/interfaces/api.interface';
import { AuthService } from './auth';
import { Accion } from '../../shared/enums/accion.enum';
import { RutaApp } from '../../shared/enums/ruta-app.enum';

@Injectable({ providedIn: 'root' })
export class MenuService {
  private readonly url = `${environment.apiUrl}/menu`;

  private readonly _menuItems = signal<MenuResponse[]>([]);
  readonly menuItems = this._menuItems.asReadonly();

  private readonly _permisosMap = computed(() => {
    const mapa = new Map<string, Set<string>>();
    for (const modulo of this._menuItems()) {
      for (const opcion of modulo.opciones) {
        mapa.set(opcion.ruta, new Set(opcion.acciones));
      }
    }
    return mapa;
  });

  puedeHacer(ruta: RutaApp, accion: Accion): boolean {
    return this._permisosMap().get(ruta)?.has(accion) ?? false;
  }

  constructor(private http: HttpClient, private auth: AuthService) { }

  obtenerMenu(): Observable<ApiResponse<MenuResponse[]>> {
    return this.http.get<ApiResponse<MenuResponse[]>>(this.url, { withCredentials: true });
  }

  cargarMenu(): void {
    this.obtenerMenu().subscribe({
      next: (res: ApiResponse<MenuResponse[]>) => {
        if (res.success) {
          this._menuItems.set(res.data);
        } else {
          this._menuItems.set(this.menuFallback());
        }
      },
      error: () => {
        this._menuItems.set(this.menuFallback());
      }
    });
  }

  private menuFallback(): MenuResponse[] {
    const esMedico = this.auth.esMedico();

    return [
      {
        id: 1,
        nombre: 'Principal',
        icono: 'home',
        orden: 1,
        opciones: [
          {
            id: 1,
            nombre: 'Dashboard',
            ruta: '/dashboard',
            icono: 'dashboard',
            orden: 1,
            acciones: ['Ver']
          },
          {
            id: 2,
            nombre: 'Pacientes',
            ruta: '/pacientes',
            icono: 'child_care',
            orden: 2,
            acciones: ['Ver', 'Crear']
          },
          ...(esMedico ? [
            {
              id: 4,
              nombre: 'Alimentos',
              ruta: '/alimentos',
              icono: 'restaurant',
              orden: 4,
              acciones: ['Ver', 'Crear', 'Editar']
            }
          ] : []),
          {
            id: 5,
            nombre: 'Predicciones',
            ruta: '/predicciones',
            icono: 'trending_up',
            orden: 5,
            acciones: ['Ver']
          }
        ]
      }
    ];
  }
}