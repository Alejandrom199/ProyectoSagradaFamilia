import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroPlus, heroCheckCircle, heroXCircle, heroCalendarDays, heroClock } from '@ng-icons/heroicons/outline';

import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { CitasService } from '../../../../core/services/citas';
import { CitaResponse, EstadoCita } from '../../../../shared/interfaces/cita.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'app-mis-citas-hoy',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, Button, Breadcrumb],
  viewProviders: [provideIcons({ heroCheckCircle, heroXCircle, heroCalendarDays, heroClock, heroPlus })],
  templateUrl: './mis-citas-hoy.html',
  styleUrl: './mis-citas-hoy.css',
})
export class MisCitasHoy implements OnInit {
  private citasService = inject(CitasService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  citas = signal<CitaResponse[]>([]);
  today = new Date().toISOString();

  contarPorEstado(estado: string): number {
    return this.citas().filter(c => c.estado === estado).length;
  }

  columnas: DatatableColumn<CitaResponse>[] = [
    {
      key: 'fechaHora', label: 'Hora', sortable: true,
      render: (row) => {
        const hora = new Date(row.fechaHora).toLocaleTimeString('es-EC', { hour: '2-digit', minute: '2-digit' });
        return `<span class="font-bold text-blue-700">${hora}</span>`;
      },
      exportValue: (row) => new Date(row.fechaHora).toLocaleTimeString('es-EC', { hour: '2-digit', minute: '2-digit' })
    },
    {
      key: 'nombreNino', label: 'Paciente', sortable: true, filterable: true,
      render: (row) => `<span class="font-medium text-gray-800">${row.nombreNino}</span>`
    },
    {
      key: 'motivo', label: 'Motivo', filterable: true,
      render: (row) => row.motivo || '<span class="text-gray-400">Sin motivo registrado</span>'
    },
    {
      key: 'estado', label: 'Estado', sortable: true,
      render: (row) => this.badgeEstado(row.estado),
      exportValue: (row) => row.estado
    }
  ];

  acciones: DatatableAction<CitaResponse>[] = [
    {
      type: 'ver',
      label: 'Ver consulta',
      icon: 'heroEye',
      onClick: (row) => this.router.navigate(['/citas', row.id])
    },
    {
      type: 'ver',
      label: 'Iniciar / Completar',
      icon: 'heroCheckCircle',
      class: 'text-green-600 hover:bg-green-50',
      visible: (row) => row.estado === 'Pendiente' || row.estado === 'EnCurso',
      onClick: (row) => {
        const siguiente = row.estado === 'Pendiente' ? EstadoCita.EnCurso : EstadoCita.Completada;
        this.cambiarEstado(row.id, siguiente);
      }
    },
    {
      type: 'eliminar',
      label: 'Cancelar',
      icon: 'heroXCircle',
      class: 'text-red-600 hover:bg-red-50',
      visible: (row) => row.estado === 'Pendiente',
      onClick: (row) => this.cambiarEstado(row.id, EstadoCita.Cancelada)
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Citas' },
  ];

  ngOnInit(): void {
    this.cargarCitas();
  }

  cargarCitas(): void {
    this.loadingBar.show();
    this.citasService.obtenerMisCitasHoy().subscribe({
      next: (response) => {
        if (response.success) {
          this.citas.set(response.data);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
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

  protected readonly formatearFecha = formatearFecha;
}
