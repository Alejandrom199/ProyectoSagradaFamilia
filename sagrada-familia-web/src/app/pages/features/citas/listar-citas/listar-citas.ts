import { Component, OnInit, Input, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { DatatableAction, DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { CitasService } from '../../../../core/services/citas';
import { NinosService } from '../../../../core/services/ninos';
import { MedicosService } from '../../../../core/services/medicos';
import { AuthService } from '../../../../core/services/auth';
import { Reportes } from '../../../../core/services/reportes';
import { CitaResponse, EstadoCita } from '../../../../shared/interfaces/cita.interface';
import { NinoDetailResponse, NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { formatearFecha, formatearHora } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'listar-citas',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, Button, Breadcrumb],

  templateUrl: './listar-citas.html',
  styleUrl: './listar-citas.css',
})
export class ListarCitas implements OnInit {
  @Input() ninoId!: string;

  private citasService = inject(CitasService);
  private ninosService = inject(NinosService);
  private medicosService = inject(MedicosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  private authService = inject(AuthService);
  private reportesService = inject(Reportes);

  nino = signal<NinoDetailResponse | null>(null);
  citas = signal<CitaResponse[]>([]);
  totalCitas = signal(0);
  filterOptions = signal<Record<string, string[]>>({});
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'desc', columnFilters: {} };

  formatearFecha = formatearFecha;

  columnas: DatatableColumn<CitaResponse>[] = [
    {
      key: 'fechaHora', label: 'Fecha y hora', sortable: true,
      render: (row) => {
        const horaInicio = formatearHora(row.fechaHora);
        const horaFin = row.fechaHoraFin ? formatearHora(row.fechaHoraFin) : null;
        return `
          <div>
            <p class="font-medium text-gray-800 text-sm">${formatearFecha(row.fechaHora)}</p>
            <p class="text-xs text-gray-500">${horaInicio}${horaFin ? ' – ' + horaFin : ''}</p>
          </div>`;
      },
      exportValue: (row) => `${formatearFecha(row.fechaHora)} ${formatearHora(row.fechaHora)}`
    },
    {
      key: 'nombreMedico', label: 'Médico', sortable: true, filterable: true,
      render: (row) => `<span class="text-gray-700">Dr(a). ${row.nombreMedico}</span>`
    },
    {
      key: 'motivo', label: 'Motivo',
      render: (row) => row.motivo || '<span class="text-gray-400">Sin motivo</span>'
    },
    {
      key: 'estado', label: 'Estado', sortable: true,
      render: (row) => this.badgeEstado(row.estado)
    }
  ];

  acciones: DatatableAction<CitaResponse>[] = [
    {
      type: 'ver',
      label: 'Ver consulta',
      onClick: (row) => this.router.navigate(['/citas', row.id])
    },
    {
      type: 'ver',
      label: 'Reagendar',
      icon: 'matEditCalendarOutline',
      class: 'text-amber-600 hover:bg-amber-50',
      visible: (row) => row.estado === 'Pendiente' || row.estado === 'Cancelada' || row.estado === 'NoAsistio',
      onClick: (row) => this.router.navigate(['/citas', row.id, 'editar'])
    },
    {
      type: 'eliminar',
      label: 'Cancelar',
      icon: 'matCancelOutline',
      class: 'text-red-600 hover:bg-red-50',
      visible: (row) => row.estado === 'Pendiente',
      onClick: (row) => this.router.navigate(['/citas', row.id])
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Citas del paciente' },
  ];

  ngOnInit(): void {
    const id = parseInt(this.ninoId);
    this.loadingBar.show();

    this.ninosService.obtenerPorId(id).subscribe({
      next: (r) => {
        if (r.success) {
          this.nino.set(r.data);
          this.migajas = [
            { label: 'Pacientes', ruta: '/pacientes' },
            { label: `${r.data.nombre} ${r.data.apellido}`, ruta: `/pacientes/${this.ninoId}` },
            { label: 'Citas' },
          ];
        }
      }
    });

    this.cargarCitas();
    this.cargarFilterOptions();
  }

  cargarFilterOptions(): void {
    this.medicosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          this.filterOptions.set({
            nombreMedico: r.data.map(m => `${m.nombre} ${m.apellido}`),
          });
        }
      }
    });
  }

  cargarDatos(): void {
    const id = parseInt(this.ninoId);
    this.loadingBar.show();

    this.ninosService.obtenerPorId(id).subscribe({
      next: (r) => { if (r.success) this.nino.set(r.data); }
    });

    this.cargarCitas();
  }

  cargarCitas(): void {
    const id = parseInt(this.ninoId);
    const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
    this.loadingBar.show();
    this.citasService.obtenerPaginadoPorNino(id, page, pageSize, search, sortBy, sortDir === 'asc').subscribe({
      next: (r) => {
        if (r.success) {
          this.citas.set(r.data);
          this.totalCitas.set(r.totalItems);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarCitas();
  }

  cambiarEstado(citaId: number, nuevoEstado: EstadoCita): void {
    this.loadingBar.show();
    this.citasService.cambiarEstado(citaId, nuevoEstado).subscribe({
      next: (response) => {
        if (response.success) this.cargarCitas();
        else this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  exportarPdf(): void {
    const n = this.nino();
    const u = this.authService.currentUser();
    const titulo = n ? `Citas — ${n.nombre} ${n.apellido}` : 'Historial de Citas';
    const params = { ninoId: this.ninoId, titulo, usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/citas-nino-pdf', params).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `citas-${new Date().toISOString().split('T')[0]}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => { }
    });
  }

  descargarExcel(): void {
    this.citasService.exportarExcelPorNino(parseInt(this.ninoId)).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `citas-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => { }
    });
  }

  private badgeEstado(estado: string): string {
    const mapa: Record<string, string> = {
      'Pendiente': 'bg-yellow-100 text-yellow-700',
      'Completada': 'bg-green-100 text-green-700',
      'Cancelada': 'bg-red-100 text-red-700',
      'NoAsistio': 'bg-gray-100 text-gray-700'
    };
    const clase = mapa[estado] || 'bg-gray-100 text-gray-700';
    return `<span class="px-2 py-1 rounded-full text-xs font-bold ${clase}">${estado}</span>`;
  }
}