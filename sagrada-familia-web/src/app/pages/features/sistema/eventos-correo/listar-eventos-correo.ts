import { Component, OnInit, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { CommonModule } from '@angular/common';

import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
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
  imports: [CommonModule, FormsModule, NgIcon, Breadcrumb],
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

  guardarAsignacion(estado: EstadoEvento): void {
    estado.guardando = true;
    estado.exito     = false;
    estado.error     = '';

    this.eventosService.asignarPlantilla(estado.evento.id, { plantillaCorreoId: estado.plantillaSeleccionada }).subscribe({
      next: (r) => {
        estado.guardando = false;
        if (r.success) {
          estado.evento = r.data;
          estado.exito  = true;
          setTimeout(() => { estado.exito = false; }, 3000);
        } else {
          estado.error = r.message;
        }
      },
      error: (err) => {
        estado.guardando = false;
        estado.error     = err?.error?.message ?? 'No se pudo guardar la asignación.';
      }
    });
  }

  // Fuerza que Angular detecte cambios en objetos mutados directamente
  trackById(_: number, e: EstadoEvento): number {
    return e.evento.id;
  }
}
