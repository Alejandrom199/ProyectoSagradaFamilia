import { Component, OnInit, Input, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroPlus, heroCheckCircle, heroXCircle, heroCalendarDays } from '@ng-icons/heroicons/outline';

import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { CitasService } from '../../../../core/services/citas';
import { NinosService } from '../../../../core/services/ninos';
import { CitaResponse, EstadoCita } from '../../../../shared/interfaces/cita.interface';
// CORRECCIÓN: Importamos la interfaz de detalle correspondiente
import { NinoDetailResponse, NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'listar-citas',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, Button, Breadcrumb],
  viewProviders: [provideIcons({ heroPlus, heroCheckCircle, heroXCircle, heroCalendarDays })],
  templateUrl: './listar-citas.html',
  styleUrl: './listar-citas.css',
})
export class ListarCitas implements OnInit {
  @Input() ninoId!: string;

  private citasService = inject(CitasService);
  private ninosService = inject(NinosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  // SOLUCIÓN: Cambiado a NinoDetailResponse para que acepte los datos del GET por ID sin romper tipado
  nino = signal<NinoDetailResponse | null>(null);
  citas = signal<CitaResponse[]>([]);

  formatearFecha = formatearFecha;

  columnas: DatatableColumn<CitaResponse>[] = [
    {
      key: 'fechaHora', label: 'Fecha y hora', sortable: true,
      render: (row) => {
        const fecha = new Date(row.fechaHora);
        return `
          <div>
            <p class="font-medium text-gray-800">${fecha.toLocaleDateString('es-EC')}</p>
            <p class="text-xs text-gray-500">${fecha.toLocaleTimeString('es-EC', { hour: '2-digit', minute: '2-digit' })}</p>
          </div>`;
      },
      exportValue: (row) => new Date(row.fechaHora).toLocaleString('es-EC')
    },
    {
      key: 'nombreMedico', label: 'Médico', sortable: true, filterable: true,
      render: (row) => `<span class="text-gray-700">Dr(a). ${row.nombreMedico}</span>`
    },
    {
      key: 'motivo', label: 'Motivo', filterable: true,
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
      label: 'Completar',
      icon: 'heroCheckCircle',
      class: 'text-green-600 hover:bg-green-50',
      visible: (row) => row.estado === 'Pendiente',
      onClick: (row) => this.cambiarEstado(row.id, EstadoCita.Completada)
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
    this.citasService.obtenerPorNino(id).subscribe({
      next: (r) => {
        if (r.success) this.citas.set(r.data);
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
}