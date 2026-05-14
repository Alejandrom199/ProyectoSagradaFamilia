import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroPlus, heroPencil, heroTrash } from '@ng-icons/heroicons/outline';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Datatable, DatatableAction, DatatableColumn } from '../../../../shared/components/datatable/datatable';
import { Reportes } from '../../../../core/services/reportes';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { ConfirmModal } from "../../../../shared/components/confirm-modal/confirm-modal";
import { Button } from '../../../../shared/components/button/button';
import { AlimentosService } from '../../../../core/services/alimentos';
import { AlimentoResponse } from '../../../../shared/interfaces/alimento.interface';

@Component({
  selector: 'listar-alimentos',
  standalone: true,
  imports: [CommonModule, NgIcon, RouterLink, Datatable, Breadcrumb, ConfirmModal, Button],
  viewProviders: [provideIcons({ heroPlus, heroPencil, heroTrash })],
  templateUrl: './listar-alimentos.html'
})
export class ListarAlimentos implements OnInit {
  private readonly alimentosService = inject(AlimentosService);
  private readonly router = inject(Router);
  private readonly loadingBar = inject(LoadingBar);
  private readonly reportesService = inject(Reportes);

  alimentos = signal<AlimentoResponse[]>([]);
  alimentoAEliminar = signal<AlimentoResponse | null>(null);

  columnas: DatatableColumn<AlimentoResponse>[] = [
    { key: 'nombre', label: 'Alimento', sortable: true, filterable: true, render: (row) => `<p class="font-medium">${row.nombre}</p>` },
    { key: 'categoriaNombre', label: 'Categoría', sortable: true, filterable: true, render: (row) => `<span class="badge badge-primary">${row.categoriaNombre}</span>` },
    // 💡 Actualizado a edadMinimaMeses
    { key: 'edadMinimaMeses', label: 'Edad mínima (meses)', sortable: true, render: (row) => `${row.edadMinimaMeses} meses` },
    // 💡 Nueva columna para el estado activo/inactivo
    { key: 'activo', label: 'Estado', sortable: true, render: (row) => row.activo ? `<span class="px-2 py-1 rounded-full text-xs font-bold bg-green-100 text-green-700">Activo</span>` : `<span class="px-2 py-1 rounded-full text-xs font-bold bg-red-100 text-red-700">Inactivo</span>` },
    { key: 'recomendacion', label: 'Recomendación', render: (row) => row.recomendacion ? `<p class="text-xs text-[var(--color-text-secondary)] max-w-xs truncate">${row.recomendacion}</p>` : '—' }
  ];

  acciones: DatatableAction<AlimentoResponse>[] = [
    { type: 'editar', onClick: (row) => this.router.navigate(['/alimentos', row.id, 'editar']) },
    { type: 'eliminar', onClick: (row) => this.alimentoAEliminar.set(row) }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Alimentos' },
  ];

  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.loadingBar.show();
    this.alimentosService.obtenerTodos().subscribe({
      next: (r) => { if (r.success) this.alimentos.set(r.data); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  confirmarEliminar() {
    const alimento = this.alimentoAEliminar();
    if (!alimento) return;

    this.loadingBar.show();
    this.alimentosService.eliminar(alimento.id).subscribe({
      next: (r) => { if (r.success) this.cargarDatos(); this.alimentoAEliminar.set(null); },
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
}