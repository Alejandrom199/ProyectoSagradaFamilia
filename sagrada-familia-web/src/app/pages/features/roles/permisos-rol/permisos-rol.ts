import { Component, OnInit, inject, signal, computed, input } from '@angular/core';
import { Router } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroShieldCheck } from '@ng-icons/heroicons/outline';
import { finalize } from 'rxjs';

import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { Button } from '../../../../shared/components/button/button';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { ModuloPermisoResponse, AccionPermisoResponse } from '../../../../shared/interfaces/permiso.interface';
import { PermisosService } from '../../../../core/services/permisos';
import { AccionPorNombrePipe } from '../../../../shared/pipes/accion-por-nombre-pipe';

const ACCIONES = ['Ver', 'Crear', 'Editar', 'Eliminar'] as const;
const NOMBRES_ROL: Record<number, string> = { 2: 'Médico', 3: 'Padre' };

@Component({
  selector: 'app-permisos-rol',
  standalone: true,
  imports: [Breadcrumb, AccionPorNombrePipe],
  viewProviders: [provideIcons({ heroShieldCheck })],
  templateUrl: './permisos-rol.html',
})
export class PermisosRol implements OnInit {
  readonly rolId = input.required<string>();

  private readonly permisosService = inject(PermisosService);
  private readonly loadingBar = inject(LoadingBar);
  private readonly router = inject(Router);

  readonly modulos = signal<ModuloPermisoResponse[]>([]);
  readonly cargando = signal(false);
  readonly guardando = signal(false);
  readonly error = signal<string | null>(null);
  readonly exitoso = signal(false);
  readonly cambiosPendientes = signal<Map<number, boolean>>(new Map());

  readonly acciones = ACCIONES;

  readonly nombreRol = computed(() => NOMBRES_ROL[parseInt(this.rolId())] ?? 'Rol');

  readonly migajas = computed<BreadcrumbItem[]>(() => [
    { label: 'Roles y Permisos', ruta: '/roles' },
    { label: this.nombreRol() }
  ]);

  readonly hayCambios = computed(() => this.cambiosPendientes().size > 0);

  ngOnInit(): void {
    this.cargarPermisos();
  }

  cargarPermisos(): void {
    this.cargando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    this.permisosService.obtenerPermisosRol(parseInt(this.rolId()))
      .pipe(finalize(() => { this.cargando.set(false); this.loadingBar.complete(); }))
      .subscribe({
        next: (res) => { if (res.success) this.modulos.set(res.data); },
        error: () => this.error.set('Error al cargar los permisos.')
      });
  }

  togglePermiso(opcionAccionId: number, valorActual: boolean): void {
    const nuevoValor = !valorActual;

    this.modulos.update(modulos =>
      modulos.map(m => ({
        ...m,
        opciones: m.opciones.map(o => ({
          ...o,
          acciones: o.acciones.map(a =>
            a.opcionAccionId === opcionAccionId ? { ...a, permitido: nuevoValor } : a
          )
        }))
      }))
    );

    this.cambiosPendientes.update(mapa => new Map(mapa).set(opcionAccionId, nuevoValor));
  }

  guardar(): void {
    if (!this.hayCambios()) return;

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const rolId = parseInt(this.rolId());
    const cambios = Array.from(this.cambiosPendientes().entries());

    const peticiones = cambios.map(([opcionAccionId, permitido]) =>
      this.permisosService.actualizarPermiso(rolId, { opcionAccionId, permitido }).toPromise()
    );

    Promise.all(peticiones)
      .then(() => {
        this.cambiosPendientes.set(new Map());
        this.exitoso.set(true);
        setTimeout(() => this.exitoso.set(false), 3000);
      })
      .catch(() => this.error.set('Error al guardar los permisos.'))
      .finally(() => { this.guardando.set(false); this.loadingBar.complete(); });
  }

  cancelar(): void {
    this.cambiosPendientes.set(new Map());
    this.cargarPermisos();
  }

  trackModulo(_: number, m: ModuloPermisoResponse): number { return m.moduloId; }
  trackOpcion(_: number, o: { opcionId: number }): number { return o.opcionId; }
  trackAccion(_: number, a: AccionPermisoResponse): number { return a.opcionAccionId; }
}