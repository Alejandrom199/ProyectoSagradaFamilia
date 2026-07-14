import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';

import { Datatable, DatatableAction, DatatableColumn, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ImportModal } from '../../../../shared/components/import-modal/import-modal';
import { Button } from '../../../../shared/components/button/button';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Reportes } from '../../../../core/services/reportes';
import { AuthService } from '../../../../core/services/auth';
import { AlimentosService } from '../../../../core/services/alimentos';
import { AlimentoResponse, EstadoAlimento, AlimentoFiltroColumnas } from '../../../../shared/interfaces/alimento.interface';
import { MenuService } from '../../../../core/services/menu';
import { Accion } from '../../../../shared/enums/accion.enum';
import { RutaApp } from '../../../../shared/enums/ruta-app.enum';

@Component({
  selector: 'listar-alimentos',
  standalone: true,
  imports: [CommonModule, NgIcon, RouterLink, Datatable, Breadcrumb, ConfirmModal, ImportModal, Button],
  templateUrl: './listar-alimentos.html'
})
export class ListarAlimentos implements OnInit {
  private readonly alimentosService = inject(AlimentosService);
  private readonly router = inject(Router);
  private readonly loadingBar = inject(LoadingBar);
  private readonly reportesService = inject(Reportes);
  private readonly authService = inject(AuthService);

  readonly menu = inject(MenuService);
  protected readonly Accion = Accion;
  protected readonly RutaApp = RutaApp;

  alimentos = signal<AlimentoResponse[]>([]);
  totalAlimentos = signal(0);
  alimentoAEliminar = signal<AlimentoResponse | null>(null);
  mostrarModalImport = signal(false);
  filterOptions = signal<Record<string, string[]>>({});

  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {} };

  readonly importarFn = (file: File) => this.alimentosService.importar(file);

  columnas: DatatableColumn<AlimentoResponse>[] = [
    { key: 'nombre', label: 'Alimento', sortable: true, render: (row) => `<p class="font-medium">${row.nombre}</p>` },
    { key: 'categoriaNombre', label: 'Categoría', sortable: true, filterable: true, render: (row) => `<span class="badge badge-primary">${row.categoriaNombre}</span>` },
    { key: 'edadMinimaMeses', label: 'Edad mínima (meses)', sortable: true, render: (row) => `${row.edadMinimaMeses} meses` },
    { key: 'descripcion', label: 'Descripción', render: (row) => row.descripcion ? `<span class="text-xs text-[var(--color-text-secondary)] max-w-xs truncate block">${row.descripcion}</span>` : '<span class="text-muted">—</span>', exportValue: (row) => row.descripcion ?? '' },
    { key: 'recomendacion', label: 'Recomendación', render: (row) => row.recomendacion ? `<p class="text-xs text-[var(--color-text-secondary)] max-w-xs truncate">${row.recomendacion}</p>` : '—', exportValue: (row) => row.recomendacion ?? '' },
    { key: 'activo', label: 'Estado', sortable: true, filterable: true, render: (row) => row.activo ? `<span class="inline-flex px-2 py-0.5 rounded text-xs font-medium bg-success-soft text-success">Activo</span>` : `<span class="inline-flex px-2 py-0.5 rounded text-xs font-medium bg-[var(--color-surface-alt)] text-muted">Inactivo</span>`, exportValue: (row) => row.activo ? 'Activo' : 'Inactivo' }
  ];

  readonly acciones = computed<DatatableAction<AlimentoResponse>[]>(() => {
    const puede = (a: Accion) => this.menu.puedeHacer(RutaApp.Alimentos, a);
    const lista: DatatableAction<AlimentoResponse>[] = [];
    if (puede(Accion.Editar)) lista.push({ type: 'editar', onClick: (row) => this.router.navigate(['/alimentos', row.id, 'editar']) });
    if (puede(Accion.Eliminar)) lista.push({ type: 'eliminar', onClick: (row) => this.alimentoAEliminar.set(row) });
    return lista;
  });

  migajas: BreadcrumbItem[] = [{ label: 'Alimentos' }];

  ngOnInit(): void {
    this.cargarDatos();
    this.cargarFilterOptions();
  }

  cargarFilterOptions(): void {
    this.alimentosService.obtenerCategorias().subscribe({
      next: (r) => {
        if (r.success) {
          this.filterOptions.update(opts => ({
            ...opts,
            [AlimentoFiltroColumnas.categoria]: r.data.map(c => c.nombre),
            [AlimentoFiltroColumnas.estado]: Object.values(EstadoAlimento),
          }));
        }
      }
    });

    // Edad mínima: se necesitan las edades de TODO el catálogo (no solo la página cargada),
    // igual que categoría/estado, para que el desplegable no cambie según la página actual.
    this.alimentosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          const edades = [...new Set(r.data.map(a => a.edadMinimaMeses))]
            .sort((a, b) => a - b)
            .map(String);
          this.filterOptions.update(opts => ({ ...opts, [AlimentoFiltroColumnas.edadMinima]: edades }));
        }
      }
    });
  }

  cargarDatos(): void {
    const { page, pageSize, search, sortBy, sortDir, columnFilters } = this.queryActual;
    this.loadingBar.show();
    this.alimentosService.obtenerPaginado(page, pageSize, search, sortBy, sortDir === 'asc', columnFilters).subscribe({
      next: (r) => {
        if (r.success) {
          this.alimentos.set(r.data);
          this.totalAlimentos.set(r.totalItems);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarDatos();
  }

  confirmarEliminar(): void {
    const alimento = this.alimentoAEliminar();
    if (!alimento) return;
    this.loadingBar.show();
    this.alimentosService.eliminar(alimento.id).subscribe({
      next: (r) => { this.alimentoAEliminar.set(null); if (r.success) this.cargarDatos(); },
      error: () => { this.alimentoAEliminar.set(null); this.loadingBar.complete(); }
    });
  }

  descargarAlimentosPdf(filtros: any): void {
    this.loadingBar.show();
    const u = this.authService.currentUser();
    const params = { titulo: 'Listado de Alimentos', filtro: JSON.stringify(filtros), usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/alimentos-pdf', params).subscribe({
      next: (blob) => { this.descargarBlob(blob, `catalogo-alimentos_${hoy()}.pdf`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel(): void {
    this.loadingBar.show();
    this.alimentosService.exportarExcel().subscribe({
      next: (blob) => { this.descargarBlob(blob, `alimentos-${hoy()}.xlsx`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarPlantilla(): void {
    this.loadingBar.show();
    this.alimentosService.descargarPlantilla().subscribe({
      next: (blob) => { this.descargarBlob(blob, `plantilla-alimentos-${hoy()}.xlsx`); this.loadingBar.complete(); },
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
