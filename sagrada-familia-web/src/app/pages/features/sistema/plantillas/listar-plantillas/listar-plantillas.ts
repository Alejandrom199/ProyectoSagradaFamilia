import { Component, OnInit, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { BreadcrumbItem, Breadcrumb } from '../../../../../shared/components/breadcrumb/breadcrumb';
import { Datatable, DatatableColumn, DatatableAction } from '../../../../../shared/components/datatable/datatable';
import { LoadingBar } from '../../../../../core/services/loading-bar';
import { PlantillasService } from '../../../../../core/services/plantillas';
import { PlantillaResponse } from '../../../../../shared/interfaces/plantilla.interface';
import { formatearFecha } from '../../../../../shared/utils/date.utils';

@Component({
  selector: 'app-listar-plantillas',
  imports: [Breadcrumb, Datatable],
  templateUrl: './listar-plantillas.html',
})
export class ListarPlantillas implements OnInit {
  private plantillasService = inject(PlantillasService);
  private loadingBar = inject(LoadingBar);
  private router = inject(Router);

  plantillas = signal<PlantillaResponse[]>([]);

  migajas: BreadcrumbItem[] = [
    { label: 'Sistema' },
    { label: 'Plantillas de correo' },
  ];

  columnas: DatatableColumn<PlantillaResponse>[] = [
    {
      key: 'codigo', label: 'Código',
      render: (row) => {
        const cls = this.etiquetaCodigo(row.codigo);
        return `<span class="px-2 py-0.5 rounded text-xs font-mono font-semibold ${cls}">${row.codigo}</span>`;
      }
    },
    { key: 'nombre', label: 'Nombre', sortable: true },
    { key: 'asunto', label: 'Asunto', sortable: true },
    {
      key: 'activo', label: 'Estado', sortable: true,
      render: (row) => row.activo
        ? `<span class="px-2 py-0.5 rounded-full text-xs font-semibold bg-emerald-50 text-emerald-700">Activa</span>`
        : `<span class="px-2 py-0.5 rounded-full text-xs font-semibold bg-slate-100 text-slate-500">Inactiva</span>`
    },
    {
      key: 'fechaActualizacion', label: 'Última edición', sortable: true,
      render: (row) => `<span class="text-slate-400 text-xs">${formatearFecha(row.fechaActualizacion ?? row.fechaCreacion)}</span>`
    }
  ];

  acciones: DatatableAction<PlantillaResponse>[] = [
    {
      type: 'editar',
      onClick: (row) => this.router.navigate(['/sistema/plantillas', row.id, 'editar'])
    }
  ];

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.loadingBar.show();
    this.plantillasService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) this.plantillas.set(r.data);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  private etiquetaCodigo(codigo: string): string {
    const mapa: Record<string, string> = {
      'CAMBIO_CLAVE': 'bg-blue-50 text-blue-700',
      'CUENTA_PADRE': 'bg-emerald-50 text-emerald-700',
      'CUENTA_MEDICO': 'bg-violet-50 text-violet-700',
    };
    return mapa[codigo] ?? 'bg-slate-100 text-slate-600';
  }
}
