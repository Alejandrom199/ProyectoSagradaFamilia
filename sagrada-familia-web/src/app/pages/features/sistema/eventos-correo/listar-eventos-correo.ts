import { Component, OnInit, signal, computed, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { CommonModule } from '@angular/common';

import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';
import { Button } from '../../../../shared/components/button/button';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { EventosCorreoService } from '../../../../core/services/eventos-correo';
import { PlantillasService } from '../../../../core/services/plantillas';
import { EventoCorreoResponse } from '../../../../shared/interfaces/evento-correo.interface';
import { PlantillaResponse } from '../../../../shared/interfaces/plantilla.interface';

interface EstadoEvento {
  evento:            EventoCorreoResponse;
  plantillaSeleccionada: number | null;
  guardando:         boolean;
  exito:             boolean;
  error:             string;
}

@Component({
  selector: 'app-listar-eventos-correo',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIcon, Breadcrumb, SearchableSelect, Button],
  templateUrl: './listar-eventos-correo.html',
})
export class ListarEventosCorreo implements OnInit {
  private readonly eventosService    = inject(EventosCorreoService);
  private readonly plantillasService = inject(PlantillasService);
  private readonly loadingBar        = inject(LoadingBar);

  estados      = signal<EstadoEvento[]>([]);
  plantillas   = signal<PlantillaResponse[]>([]);
  cargando     = signal(true);

  migajas: BreadcrumbItem[] = [
    { label: 'Eventos de correo' },
  ];

  readonly totalSinPlantilla = computed(() =>
    this.estados().filter(e => !e.evento.plantillaCorreoId).length
  );

  ngOnInit(): void {
    this.cargarDatos();
  }

  private cargarDatos(): void {
    this.loadingBar.show();
    this.cargando.set(true);

    // Cargamos eventos y plantillas activas en paralelo
    let eventosListos = false;
    let plantillasListas = false;

    const checkListo = () => {
      if (eventosListos && plantillasListas) {
        this.cargando.set(false);
        this.loadingBar.complete();
      }
    };

    this.eventosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          this.estados.set(r.data.map(e => ({
            evento:               e,
            plantillaSeleccionada: e.plantillaCorreoId,
            guardando:            false,
            exito:                false,
            error:                '',
          })));
        }
        eventosListos = true;
        checkListo();
      },
      error: () => { eventosListos = true; checkListo(); }
    });

    this.plantillasService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) this.plantillas.set(r.data.filter(p => p.activo));
        plantillasListas = true;
        checkListo();
      },
      error: () => { plantillasListas = true; checkListo(); }
    });
  }

  cambioRealizado(estado: EstadoEvento): boolean {
    return estado.plantillaSeleccionada !== estado.evento.plantillaCorreoId;
  }

  seleccionarPlantilla(eventoId: number, plantillaId: number | null): void {
    this.mutarEstado(eventoId, e => ({ ...e, plantillaSeleccionada: plantillaId }));
  }

  guardarAsignacion(estado: EstadoEvento): void {
    const eventoId = estado.evento.id;
    this.mutarEstado(eventoId, e => ({ ...e, guardando: true, exito: false, error: '' }));

    this.eventosService.asignarPlantilla(eventoId, { plantillaCorreoId: estado.plantillaSeleccionada }).subscribe({
      next: (r) => {
        if (r.success) {
          this.mutarEstado(eventoId, e => ({ ...e, guardando: false, evento: r.data, exito: true }));
          setTimeout(() => this.mutarEstado(eventoId, e => ({ ...e, exito: false })), 3000);
        } else {
          this.mutarEstado(eventoId, e => ({ ...e, guardando: false, error: r.message }));
        }
      },
      error: (err) => {
        this.mutarEstado(eventoId, e => ({
          ...e, guardando: false, error: err?.error?.message ?? 'No se pudo guardar la asignación.'
        }));
      }
    });
  }

  // Reemplaza el estado del evento dentro de la señal para que los computed() (ej. totalSinPlantilla)
  // detecten el cambio — mutar el objeto in-place no dispara la reactividad de signals.
  private mutarEstado(eventoId: number, updater: (e: EstadoEvento) => EstadoEvento): void {
    this.estados.update(lista => lista.map(e => e.evento.id === eventoId ? updater(e) : e));
  }

  trackById(_: number, e: EstadoEvento): number {
    return e.evento.id;
  }

  staggerClass(i: number): string {
    return 'stagger-' + Math.min(i + 1, 6);
  }
}
