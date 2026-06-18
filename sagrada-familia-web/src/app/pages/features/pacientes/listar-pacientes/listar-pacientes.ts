import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';

import { formatearEdad, formatearFecha } from '../../../../shared/utils/date.utils';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Datatable, DatatableAction, DatatableColumn, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ImportModal } from '../../../../shared/components/import-modal/import-modal';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from "../../../../shared/components/breadcrumb/breadcrumb";
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';
import { NinosService } from '../../../../core/services/ninos';
import { Reportes } from '../../../../core/services/reportes';

@Component({
  selector: 'listar-pacientes',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgIcon,
    RouterLink,
    Datatable,
    ConfirmModal,
    ImportModal,
    Button,
    Breadcrumb
  ],

  templateUrl: './listar-pacientes.html',
  styleUrl: './listar-pacientes.css',
})
export class ListarPacientes implements OnInit {
  private ninosService = inject(NinosService);
  private reportesService = inject(Reportes);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  ninos = signal<NinoResponse[]>([]);
  totalNinos = signal(0);
  ninoAEliminar = signal<NinoResponse | null>(null);
  mostrarModalImport = signal(false);
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {} };

  readonly importarFn = (file: File) => this.ninosService.importar(file);

  columnas: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre',
      label: 'Paciente',
      sortable: true,
      filterable: true,
      render: (row: NinoResponse) => generarAvatarHtml(row.nombre, row.apellido, row.sexo),
      exportValue: (row: NinoResponse) => `${row.nombre} ${row.apellido}`
    },
    {
      key: 'edadMeses',
      label: 'Edad Actual',
      sortable: true,
      render: (row: NinoResponse) => `<span class="font-medium text-gray-700">${formatearEdad(row.edadMeses)}</span>`,
      exportValue: (row: NinoResponse) => formatearEdad(row.edadMeses)
    },
    {
      key: 'nombrePadre',
      label: 'Representante',
      sortable: true,
      filterable: true,
      exportValue: (row: NinoResponse) => row.nombrePadre
    },
    {
      key: 'fechaNacimiento',
      label: 'Nacimiento',
      sortable: true,
      render: (row: NinoResponse) => formatearFecha(row.fechaNacimiento),
      exportValue: (row: NinoResponse) => formatearFecha(row.fechaNacimiento)
    }
  ];

  acciones: DatatableAction<NinoResponse>[] = [
    { type: 'ver', onClick: (row: NinoResponse) => this.router.navigate(['/pacientes', row.id]) },
    { type: 'medidas', onClick: (row: NinoResponse) => this.router.navigate(['/medidas', row.id]) },
    { type: 'editar', onClick: (row: NinoResponse) => this.router.navigate(['/pacientes', row.id, 'editar']) },
    { type: 'eliminar', onClick: (row: NinoResponse) => this.ninoAEliminar.set(row) }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
  ];

  ngOnInit(): void {
    this.cargarNinos();
  }

  cargarNinos(): void {
    const q = this.queryActual;
    this.loadingBar.show();
    this.ninosService.obtenerPaginado(q.page, q.pageSize, q.search, q.sortBy, q.sortDir === 'asc').subscribe({
      next: (res) => {
        this.ninos.set(res.data);
        this.totalNinos.set(res.totalItems);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarNinos();
  }

  confirmarEliminar(): void {
    const nino = this.ninoAEliminar();
    if (!nino) return;

    this.loadingBar.show();
    this.ninosService.eliminar(nino.id).subscribe({
      next: (res: ApiResponse<null>) => {
        if (res.success) {
          this.cargarNinos();
        }
        this.ninoAEliminar.set(null);
      },
      complete: () => this.loadingBar.complete(),
      error: () => {
        this.ninoAEliminar.set(null);
        this.loadingBar.complete();
      }
    });
  }

  descargarPacientesPdf(filtros: any) {
    this.loadingBar.show();
    const params = { titulo: "Listado de Pacientes", filtro: JSON.stringify(filtros) }

    this.reportesService.descargarReportePdf('reportes/ninos-pdf', params).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `pacientes_${new Date().toISOString().split('T')[0]}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel(): void {
    this.loadingBar.show();
    this.ninosService.exportarExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `pacientes-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  descargarPlantilla(): void {
    this.loadingBar.show();
    this.ninosService.descargarPlantilla().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `plantilla-pacientes-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }
}