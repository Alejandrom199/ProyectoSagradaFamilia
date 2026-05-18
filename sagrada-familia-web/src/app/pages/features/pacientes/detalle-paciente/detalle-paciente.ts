import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroUser, heroCalendarDays, heroBeaker, heroChartBar, heroPencil } from '@ng-icons/heroicons/outline';

import { NinosService } from '../../../../core/services/ninos';
import { CitasService } from '../../../../core/services/citas';
import { PrescripcionesService } from '../../../../core/services/prescripciones';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';
import { CitaResponse } from '../../../../shared/interfaces/cita.interface';
import { PrescripcionResponse } from '../../../../shared/interfaces/prescripcion.interface';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { formatearFecha, formatearEdad } from '../../../../shared/utils/date.utils';
import { DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';

@Component({
  selector: 'detalle-paciente',
  standalone: true,
  imports: [RouterLink, NgIcon, Breadcrumb, Datatable],
  viewProviders: [provideIcons({ heroUser, heroCalendarDays, heroBeaker, heroChartBar, heroPencil })],
  templateUrl: './detalle-paciente.html',
})
export class DetallePaciente implements OnInit {
  @Input() id!: string;

  private ninosService = inject(NinosService);
  private citasService = inject(CitasService);
  private prescripcionesService = inject(PrescripcionesService);
  private loadingBar = inject(LoadingBar);

  nino = signal<NinoDetailResponse | null>(null);
  citas = signal<CitaResponse[]>([]);
  prescripciones = signal<PrescripcionResponse[]>([]);

  formatearFecha = formatearFecha;
  formatearEdad = formatearEdad;

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Ficha del paciente' },
  ];

  columnasCitas: DatatableColumn<CitaResponse>[] = [
    {
      key: 'fechaHora', label: 'Fecha', sortable: true,
      render: (row) => `<span class="text-sm text-gray-700">${formatearFecha(row.fechaHora)}</span>`
    },
    {
      key: 'motivo', label: 'Motivo',
      render: (row) => row.motivo || '<span class="text-gray-400">—</span>'
    },
    {
      key: 'estado', label: 'Estado',
      render: (row) => {
        const mapa: Record<string, string> = {
          'Pendiente': 'bg-yellow-100 text-yellow-700',
          'EnCurso': 'bg-blue-100 text-blue-700',
          'Completada': 'bg-green-100 text-green-700',
          'Cancelada': 'bg-red-100 text-red-600',
          'NoAsistio': 'bg-gray-100 text-gray-600',
        };
        const clase = mapa[row.estado] || 'bg-gray-100 text-gray-600';
        const label = row.estado === 'NoAsistio' ? 'No asistió' :
          row.estado === 'EnCurso' ? 'En curso' : row.estado;
        return `<span class="px-2 py-1 rounded-full text-xs font-bold ${clase}">${label}</span>`;
      }
    }
  ];

  columnasPrescripciones: DatatableColumn<PrescripcionResponse>[] = [
    {
      key: 'fechaCreacion', label: 'Fecha', sortable: true,
      render: (row) => `<span class="text-sm text-gray-700">${formatearFecha(row.fechaCreacion)}</span>`
    },
    {
      key: 'detalleMedicamentos', label: 'Medicamentos',
      render: (row) => `<p class="text-sm text-gray-800 max-w-xs truncate" title="${row.detalleMedicamentos}">${row.detalleMedicamentos}</p>`
    },
    {
      key: 'nombreMedico', label: 'Médico',
      render: (row) => `<span class="text-sm text-gray-600">Dr(a). ${row.nombreMedico}</span>`
    }
  ];

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos(): void {
    const id = parseInt(this.id);
    this.loadingBar.show();

    this.ninosService.obtenerPorId(id).subscribe({
      next: (r) => {
        if (r.success) {
          this.nino.set(r.data);
          // ✅ migas dinámicas
          this.migajas = [
            { label: 'Pacientes', ruta: '/pacientes' },
            { label: `${r.data.nombre} ${r.data.apellido}` },
          ];
        }
      }
    });

    this.citasService.obtenerPorNino(id).subscribe({
      next: (r) => { if (r.success) this.citas.set(r.data.slice(0, 5)); }
    });

    this.prescripcionesService.obtenerHistorialPorNino(id).subscribe({
      next: (r) => {
        if (r.success) this.prescripciones.set(r.data.slice(0, 5));
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }
}