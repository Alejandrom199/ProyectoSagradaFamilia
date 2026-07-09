import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { formatearFecha, formatearHora } from '../../../../shared/utils/date.utils';
import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { NinosService } from '../../../../core/services/ninos';
import { CitasService } from '../../../../core/services/citas';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';
import { CitaResponse, EstadoCita } from '../../../../shared/interfaces/cita.interface';

interface EstadoInfo {
  label:  string;
  fondo:  string;
  texto:  string;
  icono:  string;
}

const ESTADOS: Record<EstadoCita, EstadoInfo> = {
  [EstadoCita.Pendiente]: { label: 'Próxima',    fondo: 'bg-blue-50',   texto: 'text-blue-700',   icono: 'matEventNoteOutline' },
  [EstadoCita.EnCurso]:   { label: 'En curso',   fondo: 'bg-amber-50',  texto: 'text-amber-700',  icono: 'matScheduleOutline' },
  [EstadoCita.Completada]:{ label: 'Atendida',   fondo: 'bg-green-50',  texto: 'text-green-700',  icono: 'matCheckCircleOutline' },
  [EstadoCita.Cancelada]: { label: 'Cancelada',  fondo: 'bg-red-50',    texto: 'text-red-700',    icono: 'matCancelOutline' },
  [EstadoCita.NoAsistio]: { label: 'No asistió', fondo: 'bg-slate-100', texto: 'text-slate-600',  icono: 'matWarningAmberOutline' },
  [EstadoCita.Reagendada]:{ label: 'Reagendada', fondo: 'bg-violet-50', texto: 'text-violet-700',  icono: 'matEditCalendarOutline' },
};

@Component({
  selector: 'citas-hijo',
  standalone: true,
  imports: [CommonModule, NgIcon, Breadcrumb],
  templateUrl: './citas-hijo.html',
})
export class CitasHijo implements OnInit {
  private ninosService = inject(NinosService);
  private citasService = inject(CitasService);
  private loadingBar   = inject(LoadingBar);
  private route        = inject(ActivatedRoute);

  nino     = signal<NinoDetailResponse | null>(null);
  citas    = signal<CitaResponse[]>([]);
  cargando = signal(false);

  readonly formatearFecha = formatearFecha;
  readonly formatearHora  = formatearHora;

  migajas: BreadcrumbItem[] = [{ label: 'Citas' }];

  proximaCita = computed(() => {
    const ahora = Date.now();
    return this.citas()
      .filter(c => c.estado === EstadoCita.Pendiente && new Date(c.fechaHora).getTime() >= ahora)
      .sort((a, b) => new Date(a.fechaHora).getTime() - new Date(b.fechaHora).getTime())[0] ?? null;
  });

  ngOnInit() {
    const ninoIdParam = this.route.snapshot.queryParamMap.get('ninoId');
    if (!ninoIdParam) return;
    const ninoId = parseInt(ninoIdParam, 10);

    this.cargando.set(true);
    this.loadingBar.show();

    this.ninosService.obtenerPorId(ninoId).subscribe({
      next: (r) => {
        if (r.success) {
          this.nino.set(r.data);
          this.migajas = [{ label: `Citas de ${r.data.nombre}` }];
        }
      }
    });

    this.citasService.obtenerPorNino(ninoId)
      .pipe(finalize(() => { this.cargando.set(false); this.loadingBar.complete(); }))
      .subscribe({
        next: (r) => {
          if (r.success) {
            const ordenadas = [...r.data].sort((a, b) =>
              new Date(b.fechaHora).getTime() - new Date(a.fechaHora).getTime()
            );
            this.citas.set(ordenadas);
          }
        }
      });
  }

  estadoInfo(estado: EstadoCita): EstadoInfo {
    return ESTADOS[estado];
  }
}
