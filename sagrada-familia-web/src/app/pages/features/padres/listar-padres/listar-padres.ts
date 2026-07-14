import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { DatatableAction, DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { formatearFecha, renderFechaHora } from '../../../../shared/utils/date.utils';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ImportModal } from '../../../../shared/components/import-modal/import-modal';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';
import { Reportes } from '../../../../core/services/reportes';
import { AuthService } from '../../../../core/services/auth';
import { PadresService } from '../../../../core/services/padres';
import { PadreResponse } from '../../../../shared/interfaces/padre.interface';
import { MenuService } from '../../../../core/services/menu';
import { Accion } from '../../../../shared/enums/accion.enum';
import { RutaApp } from '../../../../shared/enums/ruta-app.enum';

@Component({
  selector: 'app-listar-padres',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, ConfirmModal, ImportModal, Button, Breadcrumb],
  templateUrl: './listar-padres.html',
  styleUrl: './listar-padres.css',
})
export class ListarPadres implements OnInit {
  private readonly padresService = inject(PadresService);
  private readonly reportesService = inject(Reportes);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly loadingBar = inject(LoadingBar);

  readonly menu = inject(MenuService);
  protected readonly Accion = Accion;
  protected readonly RutaApp = RutaApp;

  padres = signal<PadreResponse[]>([]);
  totalPadres = signal(0);
  padreAEliminar = signal<PadreResponse | null>(null);
  errorEliminar = signal<string | null>(null);
  mostrarModalImport = signal(false);

  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {} };

  readonly importarFn = (file: File) => this.padresService.importar(file);

  columnas: DatatableColumn<PadreResponse>[] = [
    {
      key: 'nombre', label: 'Padre/Madre', sortable: true, filterable: false,
      render: (row) => generarAvatarHtml(row.nombre, row.apellido),
      exportValue: (row) => row.nombre
    },
    {
      key: 'telefono', label: 'Teléfono', sortable: true, filterable: true,
      render: (row) => row.telefono || '<span class="text-muted">Sin registrar</span>'
    },
    {
      key: 'totalHijos', label: 'Hijos', sortable: true, filterable: true,
      render: (row) => `<span class="badge badge-primary">${row.totalHijos ?? 0}</span>`
    },
    {
      key: 'fechaCreacion', label: 'Registro', sortable: true, filterable: true,
      render: (row) => renderFechaHora(row.fechaCreacion)
    },
  ];

  readonly acciones = computed<DatatableAction<PadreResponse>[]>(() => {
    const puede = (a: Accion) => this.menu.puedeHacer(RutaApp.Padres, a);
    const lista: DatatableAction<PadreResponse>[] = [
      { type: 'ver', label: 'Ver', onClick: (row) => this.router.navigate(['/padres', row.id, 'hijos']) },
    ];
    if (puede(Accion.Editar)) lista.push({ type: 'editar', onClick: (row) => this.router.navigate(['/padres', row.id, 'editar']) });
    if (puede(Accion.Eliminar)) lista.push({ type: 'eliminar', onClick: (row) => { this.errorEliminar.set(null); this.padreAEliminar.set(row); } });
    return lista;
  });

  migajas: BreadcrumbItem[] = [{ label: 'Padres' }];

  ngOnInit(): void {
    this.cargarPadres();
  }

  cargarPadres(): void {
    const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
    this.loadingBar.show();
    this.padresService.obtenerPaginado(page, pageSize, search, sortBy, sortDir === 'asc').subscribe({
      next: (r) => {
        if (r.success) {
          this.padres.set(r.data);
          this.totalPadres.set(r.totalItems);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarPadres();
  }

  confirmarEliminar(): void {
    const padre = this.padreAEliminar();
    if (!padre) return;
    this.loadingBar.show();
    this.errorEliminar.set(null);
    this.padresService.eliminar(padre.id).subscribe({
      next: (r) => {
        if (r.success) {
          this.padres.update(lista => lista.filter(p => p.id !== padre.id));
          this.padreAEliminar.set(null);
        }
        this.loadingBar.complete();
      },
      error: (err) => {
        this.errorEliminar.set(err.error?.message ?? 'No se pudo eliminar el representante.');
        this.loadingBar.complete();
      }
    });
  }

  descargarPadresPdf(filtros: any): void {
    this.loadingBar.show();
    const u = this.authService.currentUser();
    const params = { titulo: 'Listado de Padres', filtro: JSON.stringify(filtros), usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/padres-pdf', params).subscribe({
      next: (blob) => { this.descargarBlob(blob, `padres_${hoy()}.pdf`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel(): void {
    this.loadingBar.show();
    this.padresService.exportarExcel().subscribe({
      next: (blob) => { this.descargarBlob(blob, `padres-${hoy()}.xlsx`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarPlantilla(): void {
    this.loadingBar.show();
    this.padresService.descargarPlantilla().subscribe({
      next: (blob) => { this.descargarBlob(blob, `plantilla-padres-${hoy()}.xlsx`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  private descargarBlob(blob: Blob, nombre: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = nombre;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}

function hoy(): string {
  return new Date().toISOString().split('T')[0];
}
