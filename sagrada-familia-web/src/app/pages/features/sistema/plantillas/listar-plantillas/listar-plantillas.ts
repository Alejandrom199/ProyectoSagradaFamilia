import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { BreadcrumbItem, Breadcrumb } from '../../../../../shared/components/breadcrumb/breadcrumb';
import { Datatable, DatatableColumn, DatatableAction } from '../../../../../shared/components/datatable/datatable';
import { Button } from '../../../../../shared/components/button/button';
import { ConfirmModal } from '../../../../../shared/components/confirm-modal/confirm-modal';
import { LoadingBar } from '../../../../../core/services/loading-bar';
import { PlantillasService } from '../../../../../core/services/plantillas';
import { PlantillaResponse } from '../../../../../shared/interfaces/plantilla.interface';
import { formatearFecha, renderFechaHora } from '../../../../../shared/utils/date.utils';
import { MenuService } from '../../../../../core/services/menu';
import { Accion } from '../../../../../shared/enums/accion.enum';
import { RutaApp } from '../../../../../shared/enums/ruta-app.enum';

@Component({
  selector: 'app-listar-plantillas',
  imports: [RouterLink, NgIcon, Breadcrumb, Datatable, Button, ConfirmModal],
  templateUrl: './listar-plantillas.html',
})
export class ListarPlantillas implements OnInit {
  private readonly plantillasService = inject(PlantillasService);
  private readonly loadingBar        = inject(LoadingBar);
  private readonly router            = inject(Router);

  readonly menu   = inject(MenuService);
  protected readonly Accion  = Accion;
  protected readonly RutaApp = RutaApp;

  plantillas          = signal<PlantillaResponse[]>([]);
  plantillaAEliminar  = signal<PlantillaResponse | null>(null);
  errorEliminar       = signal('');

  migajas: BreadcrumbItem[] = [
    { label: 'Sistema' },
    { label: 'Plantillas de correo' },
  ];

  columnas: DatatableColumn<PlantillaResponse>[] = [
    {
      key: 'nombre', label: 'Nombre', sortable: true,
    },
    {
      key: 'codigo', label: 'Código',
      render: (row) => {
        if (!row.codigo) return '<span class="text-slate-400 text-xs">—</span>';
        const cls = this.etiquetaCodigo(row.codigo);
        return `<span class="px-2 py-0.5 rounded text-xs font-mono font-semibold ${cls}">${row.codigo}</span>`;
      }
    },
    { key: 'asunto', label: 'Asunto', sortable: true },
    {
      key: 'activo', label: 'Estado', sortable: true,
      render: (row) => row.activo
        ? `<span class="px-2 py-0.5 rounded-full text-xs font-semibold bg-emerald-50 dark:bg-emerald-500/10 text-emerald-700">Activa</span>`
        : `<span class="px-2 py-0.5 rounded-full text-xs font-semibold bg-slate-100 text-slate-500">Inactiva</span>`
    },
    {
      key: 'fechaActualizacion', label: 'Última edición', sortable: true,
      render: (row) => renderFechaHora(row.fechaActualizacion ?? row.fechaCreacion)
    }
  ];

  readonly acciones = computed<DatatableAction<PlantillaResponse>[]>(() => {
    const puede = (a: Accion) => this.menu.puedeHacer(RutaApp.Plantillas, a);
    const lista: DatatableAction<PlantillaResponse>[] = [];
    if (puede(Accion.Editar))   lista.push({ type: 'editar',   onClick: (row) => this.router.navigate(['/sistema/plantillas', row.id, 'editar']) });
    if (puede(Accion.Eliminar)) lista.push({ type: 'eliminar', onClick: (row) => this.plantillaAEliminar.set(row) });
    return lista;
  });

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.loadingBar.show();
    this.plantillasService.obtenerTodos().subscribe({
      next: (r) => { if (r.success) this.plantillas.set(r.data); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  confirmarEliminar(): void {
    const plantilla = this.plantillaAEliminar();
    if (!plantilla) return;
    this.errorEliminar.set('');
    this.loadingBar.show();
    this.plantillasService.eliminar(plantilla.id).subscribe({
      next: (r) => {
        if (r.success) {
          this.plantillas.update(lista => lista.filter(p => p.id !== plantilla.id));
          this.plantillaAEliminar.set(null);
        } else {
          this.errorEliminar.set(r.message);
          this.plantillaAEliminar.set(null);
        }
        this.loadingBar.complete();
      },
      error: (err) => {
        this.errorEliminar.set(err?.error?.message ?? 'No se pudo eliminar la plantilla.');
        this.plantillaAEliminar.set(null);
        this.loadingBar.complete();
      }
    });
  }

  private etiquetaCodigo(codigo: string): string {
    const mapa: Record<string, string> = {
      'CAMBIO_CLAVE':  'bg-blue-50 dark:bg-blue-500/10 text-blue-700',
      'CUENTA_PADRE':  'bg-emerald-50 dark:bg-emerald-500/10 text-emerald-700',
      'CUENTA_MEDICO': 'bg-violet-50 dark:bg-violet-500/10 text-violet-700',
      'CUENTA_ADMIN':  'bg-purple-50 dark:bg-purple-500/10 text-purple-700',
    };
    return mapa[codigo] ?? 'bg-slate-100 text-slate-600';
  }
}
