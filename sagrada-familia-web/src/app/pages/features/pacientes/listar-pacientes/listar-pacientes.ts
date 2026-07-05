import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';
import { formatearEdad, formatearFecha } from '../../../../shared/utils/date.utils';
import { Datatable, DatatableAction, DatatableColumn, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ImportModal } from '../../../../shared/components/import-modal/import-modal';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';
import { NinosService } from '../../../../core/services/ninos';
import { Reportes } from '../../../../core/services/reportes';
import { AuthService } from '../../../../core/services/auth';
import { MenuService } from '../../../../core/services/menu';
import { Accion } from '../../../../shared/enums/accion.enum';
import { RutaApp } from '../../../../shared/enums/ruta-app.enum';

@Component({
  selector: 'listar-pacientes',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIcon, RouterLink, Datatable, ConfirmModal, ImportModal, Button, Breadcrumb],
  templateUrl: './listar-pacientes.html',
  styleUrl: './listar-pacientes.css',
})
export class ListarPacientes implements OnInit {
  private readonly ninosService = inject(NinosService);
  private readonly reportesService = inject(Reportes);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly loadingBar = inject(LoadingBar);

  readonly menu = inject(MenuService);
  protected readonly Accion = Accion;
  protected readonly RutaApp = RutaApp;

  ninos = signal<NinoResponse[]>([]);
  totalNinos = signal(0);
  ninoAEliminar = signal<NinoResponse | null>(null);
  errorEliminar = signal<string | null>(null);
  mostrarModalImport = signal(false);

  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {} };

  readonly importarFn = (file: File) => this.ninosService.importar(file);

  columnas: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre', label: 'Paciente', sortable: true, filterable: false,
      render: (row) => generarAvatarHtml(row.nombre, row.apellido, row.sexo),
      exportValue: (row) => `${row.nombre} ${row.apellido}`
    },
    {
      key: 'edadMeses', label: 'Edad Actual', sortable: true, filterable: true,
      render: (row) => `<span class="font-medium text-gray-700">${formatearEdad(row.edadMeses)}</span>`,
      exportValue: (row) => formatearEdad(row.edadMeses)
    },
    {
      key: 'nombrePadre', label: 'Representante', sortable: true, filterable: true,
      exportValue: (row) => row.nombrePadre
    },
    {
      key: 'fechaNacimiento', label: 'Nacimiento', sortable: true, filterable: true,
      render: (row) => formatearFecha(row.fechaNacimiento),
      exportValue: (row) => formatearFecha(row.fechaNacimiento)
    }
  ];

  readonly acciones = computed<DatatableAction<NinoResponse>[]>(() => {
    const puede = (a: Accion) => this.menu.puedeHacer(RutaApp.Pacientes, a);
    const lista: DatatableAction<NinoResponse>[] = [
      { type: 'ver', onClick: (row) => this.router.navigate(['/pacientes', row.id]) },
      { type: 'medidas', onClick: (row) => this.router.navigate(['/medidas', row.id]) },
    ];
    if (puede(Accion.Editar)) lista.push({ type: 'editar', onClick: (row) => this.router.navigate(['/pacientes', row.id, 'editar']) });
    if (puede(Accion.Eliminar)) lista.push({ type: 'eliminar', onClick: (row) => { this.errorEliminar.set(null); this.ninoAEliminar.set(row); } });
    return lista;
  });

  migajas: BreadcrumbItem[] = [{ label: 'Pacientes', ruta: '/pacientes' }];

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
    this.errorEliminar.set(null);
    this.ninosService.eliminar(nino.id).subscribe({
      next: (res: ApiResponse<null>) => {
        if (res.success) { this.cargarNinos(); this.ninoAEliminar.set(null); }
      },
      complete: () => this.loadingBar.complete(),
      error: (err) => {
        this.errorEliminar.set(err.error?.message ?? 'No se pudo eliminar el paciente.');
        this.loadingBar.complete();
      }
    });
  }

  descargarPacientesPdf(filtros: any): void {
    this.loadingBar.show();
    const u = this.authService.currentUser();
    const params = { titulo: 'Listado de Pacientes', filtro: JSON.stringify(filtros), usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/ninos-pdf', params).subscribe({
      next: (blob) => { this.descargarBlob(blob, `pacientes_${hoy()}.pdf`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel(): void {
    this.loadingBar.show();
    this.ninosService.exportarExcel().subscribe({
      next: (blob) => { this.descargarBlob(blob, `pacientes-${hoy()}.xlsx`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarPlantilla(): void {
    this.loadingBar.show();
    this.ninosService.descargarPlantilla().subscribe({
      next: (blob) => { this.descargarBlob(blob, `plantilla-pacientes-${hoy()}.xlsx`); this.loadingBar.complete(); },
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
