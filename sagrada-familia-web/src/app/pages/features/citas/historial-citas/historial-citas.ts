import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroEye, heroCalendarDays } from '@ng-icons/heroicons/outline';

import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { CitasService } from '../../../../core/services/citas';
import { CitaResponse, EstadoCita } from '../../../../shared/interfaces/cita.interface';

@Component({
  selector: 'historial-citas',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, Breadcrumb],
  viewProviders: [provideIcons({ heroEye, heroCalendarDays })],
  templateUrl: './historial-citas.html',
})
export class HistorialCitas implements OnInit {
  private citasService = inject(CitasService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  citas = signal<CitaResponse[]>([]);

  migajas: BreadcrumbItem[] = [
    { label: 'Citas', ruta: '/citas' },
    { label: 'Historial' },
  ];

  columnas: DatatableColumn<CitaResponse>[] = [
    {
      key: 'fechaHora',
      label: 'Fecha y hora',
      sortable: true,
      filterable: true,
      render: (row) => `<span class="font-medium text-gray-800">${formatearFecha(row.fechaHora)}</span>`,
      exportValue: (row) => formatearFecha(row.fechaHora)
    },
    {
      key: 'nombreNino',
      label: 'Paciente',
      sortable: true,
      filterable: true,
      render: (row) => `<span class="font-medium text-gray-800">${row.nombreNino}</span>`
    },
    {
      key: 'motivo',
      label: 'Motivo',
      filterable: true,
      render: (row) => row.motivo || '<span class="text-gray-400 text-xs">Sin motivo</span>'
    },
    {
      key: 'estado',
      label: 'Estado',
      sortable: true,
      filterable: true,
      render: (row) => this.badgeEstado(row.estado),
      exportValue: (row) => row.estado
    },
    {
      key: 'tienePrescripcion',
      label: 'Prescripción',
      render: (row) => row.tienePrescripcion
        ? `<span class="inline-flex items-center gap-1 text-xs font-semibold text-emerald-700 bg-emerald-50 px-2 py-0.5 rounded-full">Emitida</span>`
        : `<span class="inline-flex items-center gap-1 text-xs font-semibold text-gray-400 bg-gray-100 px-2 py-0.5 rounded-full">No</span>`,
      exportValue: (row) => row.tienePrescripcion ? 'Sí' : 'No'
    }
  ];

  acciones: DatatableAction<CitaResponse>[] = [
    {
      type: 'ver',
      label: 'Ver detalle',
      icon: 'heroEye',
      onClick: (row) => this.router.navigate(['/citas', row.id])
    }
  ];

  ngOnInit(): void {
    this.cargarHistorial();
  }

  cargarHistorial(): void {
    this.loadingBar.show();
    this.citasService.obtenerHistorial().subscribe({
      next: (res) => {
        if (res.success) this.citas.set(res.data);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  private badgeEstado(estado: string): string {
    const mapa: Record<string, string> = {
      [EstadoCita.Pendiente]: 'bg-amber-100 text-amber-700',
      [EstadoCita.EnCurso]: 'bg-blue-100 text-blue-700',
      [EstadoCita.Completada]: 'bg-emerald-100 text-emerald-700',
      [EstadoCita.Cancelada]: 'bg-red-100 text-red-600',
      [EstadoCita.NoAsistio]: 'bg-gray-100 text-gray-500',
    };
    const label: Record<string, string> = {
      [EstadoCita.EnCurso]: 'En curso',
      [EstadoCita.NoAsistio]: 'No asistió',
    };
    const clase = mapa[estado] ?? 'bg-gray-100 text-gray-500';
    const texto = label[estado] ?? estado;
    return `<span class="px-2 py-0.5 rounded-full text-xs font-bold ${clase}">${texto}</span>`;
  }
}