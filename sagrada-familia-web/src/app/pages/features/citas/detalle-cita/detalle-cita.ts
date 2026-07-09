import { Component, Input, OnInit, inject, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { CitasService } from '../../../../core/services/citas';
import { ConsultasService } from '../../../../core/services/consultas';
import { CitaResponse, EstadoCita } from '../../../../shared/interfaces/cita.interface';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { formatearFecha, formatearHora } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'detalle-cita',
  standalone: true,
  imports: [RouterLink, NgIcon, Breadcrumb, FormsModule],
  templateUrl: './detalle-cita.html',
})
export class DetalleCita implements OnInit {
  @Input() citaId!: string;

  private citasService     = inject(CitasService);
  private consultasService = inject(ConsultasService);
  private router       = inject(Router);
  private loadingBar   = inject(LoadingBar);

  cita            = signal<CitaResponse | null>(null);
  cambiandoEstado = signal(false);
  formatearFecha  = formatearFecha;
  formatearHora   = formatearHora;
  EstadoCita      = EstadoCita;

  // Estado de modales
  mostrarModalEstado    = signal(false);
  estadoPendiente       = signal<EstadoCita | null>(null);
  motivoCancelacion     = signal('');
  mostrarModalReagendar = signal(false);

  // Datos clínicos de la consulta (editables mientras está EnCurso)
  consultaMotivo        = signal('');
  consultaDiagnostico   = signal('');
  consultaIndicaciones  = signal('');
  consultaEvolucion     = signal('');
  guardandoConsulta     = signal(false);

  puedeAccionar = computed(() => {
    const c = this.cita();
    if (!c) return false;
    return new Date() >= new Date(c.fechaHora);
  });

  esEstadoFinal = computed(() => {
    const estado = this.cita()?.estado;
    return estado === 'Completada' || estado === 'NoAsistio' || estado === 'Cancelada';
  });

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
        if (r.success) {
          this.cita.set(r.data);
          const consulta = r.data.consulta;
          this.consultaMotivo.set(consulta?.motivo ?? '');
          this.consultaDiagnostico.set(consulta?.diagnostico ?? '');
          this.consultaIndicaciones.set(consulta?.indicaciones ?? '');
          this.consultaEvolucion.set(consulta?.evolucion ?? '');
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  iniciarConsulta(): void {
    const cita = this.cita();
    if (!cita) return;

    this.cambiandoEstado.set(true);
    this.loadingBar.show();

    this.citasService.iniciarConsulta(cita.id).subscribe({
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

  // Guarda los datos clínicos y completa la consulta en un solo paso.
  // El motivo no se re-edita acá: se reenvía tal cual llegó (copiado de la cita al iniciar
  // la consulta) para no perderlo, ya que el backend reemplaza el campo completo al actualizar.
  completarConsulta(): void {
    const consultaId = this.cita()?.consulta?.id;
    if (!consultaId) return;

    this.guardandoConsulta.set(true);
    this.loadingBar.show();

    this.consultasService.actualizar(consultaId, {
      motivo: this.consultaMotivo() || undefined,
      diagnostico: this.consultaDiagnostico() || undefined,
      indicaciones: this.consultaIndicaciones() || undefined,
      evolucion: this.consultaEvolucion() || undefined,
    }).subscribe({
      next: (r) => {
        if (!r.success) {
          this.guardandoConsulta.set(false);
          this.loadingBar.complete();
          return;
        }
        this.consultasService.completar(consultaId).subscribe({
          next: () => {
            this.cargarCita();
            this.guardandoConsulta.set(false);
            this.loadingBar.complete();
          },
          error: () => {
            this.guardandoConsulta.set(false);
            this.loadingBar.complete();
          }
        });
      },
      error: () => {
        this.guardandoConsulta.set(false);
        this.loadingBar.complete();
      }
    });
  }

  iniciarCambioEstado(nuevoEstado: EstadoCita): void {
    if (nuevoEstado === EstadoCita.Cancelada || nuevoEstado === EstadoCita.NoAsistio) {
      this.estadoPendiente.set(nuevoEstado);
      this.motivoCancelacion.set('');
      this.mostrarModalEstado.set(true);
    } else {
      this.ejecutarCambioEstado(nuevoEstado);
    }
  }

  confirmarCambioEstado(): void {
    const estado = this.estadoPendiente();
    if (!estado) return;
    this.mostrarModalEstado.set(false);
    this.ejecutarCambioEstado(estado, this.motivoCancelacion() || undefined);
  }

  private ejecutarCambioEstado(nuevoEstado: EstadoCita, motivoCancelacion?: string): void {
    const cita = this.cita();
    if (!cita) return;

    this.cambiandoEstado.set(true);
    this.loadingBar.show();

    this.citasService.cambiarEstado(cita.id, nuevoEstado, motivoCancelacion).subscribe({
      next: (r) => {
        if (r.success) {
          this.cargarCita();
          if (nuevoEstado === EstadoCita.Cancelada || nuevoEstado === EstadoCita.NoAsistio) {
            this.mostrarModalReagendar.set(true);
          }
        }
        this.cambiandoEstado.set(false);
        this.loadingBar.complete();
      },
      error: () => {
        this.cambiandoEstado.set(false);
        this.loadingBar.complete();
      }
    });
  }

  irAReagendar(): void {
    this.mostrarModalReagendar.set(false);
    this.router.navigate(['/citas', this.citaId, 'editar']);
  }

  emitirPrescripcion(): void {
    const consultaId = this.cita()?.consulta?.id;
    if (!consultaId) return;
    this.router.navigate(['/prescripciones/consulta', consultaId, 'crear'], {
      queryParams: { citaId: this.citaId }
    });
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