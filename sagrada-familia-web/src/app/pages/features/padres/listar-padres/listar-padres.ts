import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { DatatableAction, DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';

import { formatearFecha } from '../../../../shared/utils/date.utils';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { ConfirmModal } from "../../../../shared/components/confirm-modal/confirm-modal";
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';
import { Reportes } from '../../../../core/services/reportes';
import { PadresService } from '../../../../core/services/padres';
import { AuthService } from '../../../../core/services/auth';
import { PadreResponse } from '../../../../shared/interfaces/padre.interface';

@Component({
  selector: 'app-listar-padres',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, ConfirmModal, Button, Breadcrumb],

  templateUrl: './listar-padres.html',
  styleUrl: './listar-padres.css',
})
export class ListarPadres implements OnInit {
  private padresService = inject(PadresService);
  private reportesService = inject(Reportes);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  readonly auth = inject(AuthService);

  padres = signal<PadreResponse[]>([]);
  totalPadres = signal(0);
  padreAEliminar = signal<PadreResponse | null>(null);

  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc' };

  columnas: DatatableColumn<PadreResponse>[] = [
    {
      key: 'nombre',
      label: 'Padre/Madre',
      sortable: true,
      filterable: true,
      render: (row) => generarAvatarHtml(row.nombre, row.apellido),
      exportValue: (row) => row.nombre
    },
    {
      key: 'telefono',
      label: 'Teléfono',
      sortable: true,
      filterable: true,
      render: (row) => row.telefono || '<span class="text-gray-400">Sin registrar</span>'
    },
    {
      key: 'totalHijos',
      label: 'Hijos',
      sortable: true,
      filterable: true,
      render: (row) => `<span class="badge badge-primary">${row.totalHijos ?? 0}</span>`
    },
    {
      key: 'fechaCreacion',
      label: 'Registro',
      sortable: true,
      filterable: true,
      render: (row) => formatearFecha(row.fechaCreacion)
    },
    // {
    //   key: 'activo',
    //   label: 'ESTADO',
    //   render: (row) => row.activo === 'ACTIVO'
    //     ? `<span class="bg-green-100 text-green-700 px-3 py-1 rounded-full text-xs font-bold">ACTIVO</span>`
    //     : `<span class="bg-red-100 text-red-700 px-3 py-1 rounded-full text-xs font-bold">INACTIVO</span>`
    // }
  ];

  acciones: DatatableAction<PadreResponse>[] = [
    {
      type: 'ver',
      label: 'Ver hijos',
      onClick: (row) => this.router.navigate(['/padres', row.id, 'hijos'])
    },
    {
      type: 'editar',
      onClick: (row) => this.router.navigate(['/padres', row.id, 'editar'])
    },
    {
      type: 'eliminar',
      onClick: (row) => this.padreAEliminar.set(row)
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Padres' },
  ];

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
    this.padresService.eliminar(padre.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.padres.update(actuales => actuales.filter(p => p.id !== padre.id));
        } else {
          console.error(response.message);
        }
        this.padreAEliminar.set(null);
        this.loadingBar.complete();
      },
      error: (err) => {
        console.error('Error al eliminar padre', err);
        this.padreAEliminar.set(null);
        this.loadingBar.complete();
      }
    });
  }

  descargarPadresPdf(filtros: any) {
    this.loadingBar.show();
    const params = { titulo: 'Listado de Padres', filtro: JSON.stringify(filtros) };

    this.reportesService.descargarReportePdf('reportes/padres-pdf', params).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `padres_${new Date().toISOString().split('T')[0]}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }
}