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
import { AlimentoResponse } from '../../../../shared/interfaces/alimento.interface';

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

  private queryActual: ServerQuery = {
    page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc'
  };

  readonly importarFn = (file: File) => this.alimentosService.importar(file);

  columnas: DatatableColumn<AlimentoResponse>[] = [
    { key: 'nombre',          label: 'Alimento',             sortable: true, filterable: true, render: (row) => `<p class="font-medium">${row.nombre}</p>` },
    { key: 'categoriaNombre', label: 'Categoría',            sortable: true, filterable: true, render: (row) => `<span class="badge badge-primary">${row.categoriaNombre}</span>` },
    { key: 'edadMinimaMeses', label: 'Edad mínima (meses)',  sortable: true, render: (row) => `${row.edadMinimaMeses} meses` },
    { key: 'recomendacion',   label: 'Recomendación',        render: (row) => row.recomendacion ? `<p class="text-xs text-[var(--color-text-secondary)] max-w-xs truncate">${row.recomendacion}</p>` : '—' }
  ];

  acciones: DatatableAction<AlimentoResponse>[] = [
    { type: 'editar',   onClick: (row) => this.router.navigate(['/alimentos', row.id, 'editar']) },
    { type: 'eliminar', onClick: (row) => this.alimentoAEliminar.set(row) }
  ];

  migajas: BreadcrumbItem[] = [{ label: 'Alimentos' }];

  ngOnInit() { this.cargarDatos(); }

  cargarDatos() {
    const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
    this.loadingBar.show();
    this.alimentosService.obtenerPaginado(page, pageSize, search, sortBy, sortDir === 'asc').subscribe({
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
