import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  heroCalendarDays, heroUser, heroBeaker, heroCheckCircle,
  heroXCircle, heroClock, heroExclamationCircle,
  heroClipboardDocumentList, heroChartBar
} from '@ng-icons/heroicons/outline';

import { CitasService } from '../../../../core/services/citas';
import { CitaResponse, EstadoCita } from '../../../../shared/interfaces/cita.interface';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { formatearFecha } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'detalle-cita',
  standalone: true,
  imports: [RouterLink, NgIcon, Breadcrumb],
  viewProviders: [provideIcons({
    heroCalendarDays, heroUser, heroBeaker, heroCheckCircle,
    heroXCircle, heroClock, heroExclamationCircle,
    heroClipboardDocumentList, heroChartBar
  })],
  templateUrl: './detalle-cita.html',
})
export class DetalleCita implements OnInit {
  @Input() citaId!: string;

  private citasService = inject(CitasService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  cita = signal<CitaResponse | null>(null);
  cambiandoEstado = signal(false);
  formatearFecha = formatearFecha;
  EstadoCita = EstadoCita;

  migajas: BreadcrumbItem[] = [
    { label: 'Citas', ruta: '/citas' },
    { label: 'Detalle de consulta' },
  ];

  ngOnInit(): void {
    this.cargarCita();
  }

  cargarCita(): void {
    this.loadingBar.show();
    this.citasService.obtenerPorId(parseInt(this.citaId)).subscribe({
      next: (r) => {
        if (r.success) this.cita.set(r.data);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  cambiarEstado(nuevoEstado: EstadoCita): void {
    const cita = this.cita();
    if (!cita) return;

    this.cambiandoEstado.set(true);
    this.loadingBar.show();

    this.citasService.cambiarEstado(cita.id, nuevoEstado).subscribe({
      next: (r) => {
        if (r.success) this.cargarCita();
        this.cambiandoEstado.set(false);
        this.loadingBar.complete();
      },
      error: () => {
        this.cambiandoEstado.set(false);
        this.loadingBar.complete();
      }
    });
  }

  emitirPrescripcion(): void {
    this.router.navigate(['/prescripciones/cita', this.citaId, 'crear']);
  }

  badge(estado: string): { clase: string; label: string } {
    const mapa: Record<string, { clase: string; label: string }> = {
      'Pendiente': { clase: 'bg-yellow-100 text-yellow-700 border-yellow-200', label: 'Pendiente' },
      'En Curso': { clase: 'bg-blue-100 text-blue-700 border-blue-200', label: 'En curso' },
      'Completada': { clase: 'bg-green-100 text-green-700 border-green-200', label: 'Completada' },
      'NoAsistio': { clase: 'bg-gray-100 text-gray-600 border-gray-200', label: 'No asistió' },
      'Cancelada': { clase: 'bg-red-100 text-red-600 border-red-200', label: 'Cancelada' },
    };
    return mapa[estado] ?? { clase: 'bg-gray-100 text-gray-600 border-gray-200', label: estado };
  }
}