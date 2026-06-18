import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Datatable, DatatableAction, DatatableColumn, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { Reportes } from '../../../../core/services/reportes';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ImportModal } from '../../../../shared/components/import-modal/import-modal';
import { Button } from '../../../../shared/components/button/button';
import { AlimentosService } from '../../../../core/services/alimentos';
import { AlimentoResponse, EstadoAlimento, AlimentoFiltroColumnas } from '../../../../shared/interfaces/alimento.interface';

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

  alimentos          = signal<AlimentoResponse[]>([]);
  totalAlimentos     = signal(0);
  alimentoAEliminar  = signal<AlimentoResponse | null>(null);
  mostrarModalImport = signal(false);
  filterOptions      = signal<Record<string, string[]>>({});

  private queryActual: ServerQuery = {
    page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {}
  };

  readonly importarFn = (file: File) => this.alimentosService.importar(file);

  columnas: DatatableColumn<AlimentoResponse>[] = [
    { key: 'nombre',          label: 'Alimento',            sortable: true, filterable: true, render: (row) => `<p class="font-medium">${row.nombre}</p>` },
    { key: 'categoriaNombre', label: 'Categoría',           sortable: true, filterable: true, render: (row) => `<span class="badge badge-primary">${row.categoriaNombre}</span>` },
    { key: 'edadMinimaMeses', label: 'Edad mínima (meses)', sortable: true, filterable: true, render: (row) => `${row.edadMinimaMeses} meses` },
    { key: 'descripcion',     label: 'Descripción',         render: (row) => row.descripcion ? `<span class="text-xs text-slate-500 max-w-xs truncate block">${row.descripcion}</span>` : '<span class="text-slate-300">—</span>', exportValue: (row) => row.descripcion ?? '' },
    { key: 'recomendacion',   label: 'Recomendación',       render: (row) => row.recomendacion ? `<p class="text-xs text-[var(--color-text-secondary)] max-w-xs truncate">${row.recomendacion}</p>` : '—', exportValue: (row) => row.recomendacion ?? '' },
    { key: 'activo',          label: 'Estado',              sortable: true, filterable: true, render: (row) => row.activo ? `<span class="inline-flex px-2 py-0.5 rounded text-xs font-medium bg-green-50 text-green-700">Activo</span>` : `<span class="inline-flex px-2 py-0.5 rounded text-xs font-medium bg-slate-100 text-slate-500">Inactivo</span>`, exportValue: (row) => row.activo ? 'Activo' : 'Inactivo' }
  ];

  acciones: DatatableAction<AlimentoResponse>[] = [
    { type: 'editar',   onClick: (row) => this.router.navigate(['/alimentos', row.id, 'editar']) },
    { type: 'eliminar', onClick: (row) => this.alimentoAEliminar.set(row) }
  ];

  migajas: BreadcrumbItem[] = [{ label: 'Alimentos' }];

  ngOnInit() {
    this.cargarDatos();
    this.cargarFilterOptions();
  }

  cargarFilterOptions() {
    this.alimentosService.obtenerCategorias().subscribe({
      next: (respuesta) => {
        if (respuesta.success) {
          this.filterOptions.set({
            [AlimentoFiltroColumnas.categoria]: respuesta.data.map(categoria => categoria.nombre),
            [AlimentoFiltroColumnas.estado]:    Object.values(EstadoAlimento),
          });
        }
      }
    });
  }

  cargarDatos() {
    const { page, pageSize, search, sortBy, sortDir, columnFilters } = this.queryActual;
    this.loadingBar.show();
    this.alimentosService.obtenerPaginado(page, pageSize, search, sortBy, sortDir === 'asc', columnFilters).subscribe({
      next: (respuesta) => {
        if (respuesta.success) {
          this.alimentos.set(respuesta.data);
          this.totalAlimentos.set(respuesta.totalItems);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery) {
    this.queryActual = query;
    this.cargarDatos();
  }

  confirmarEliminar() {
    const alimento = this.alimentoAEliminar();
    if (!alimento) return;
    this.loadingBar.show();
    this.alimentosService.eliminar(alimento.id).subscribe({
      next: (r) => {
        this.alimentoAEliminar.set(null);
        if (r.success) this.cargarDatos();
      },
      error: () => { this.alimentoAEliminar.set(null); this.loadingBar.complete(); }
    });
  }

  descargarAlimentosPdf(filtros: any) {
    this.loadingBar.show();
    const params = { titulo: 'Listado de Alimentos', filtro: JSON.stringify(filtros) };
    this.reportesService.descargarReportePdf('reportes/alimentos-pdf', params).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `catalogo-alimentos_${new Date().toISOString().split('T')[0]}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel() {
    this.loadingBar.show();
    this.alimentosService.exportarExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `alimentos-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  descargarPlantilla() {
    this.loadingBar.show();
    this.alimentosService.descargarPlantilla().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `plantilla-alimentos-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }
}
