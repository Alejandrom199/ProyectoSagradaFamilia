import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { NinosService } from '../../../../core/services/ninos';
import { PrescripcionesService } from '../../../../core/services/prescripciones';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';
import { PrescripcionResponse } from '../../../../shared/interfaces/prescripcion.interface';

@Component({
  selector: 'prescripciones-hijo',
  standalone: true,
  imports: [CommonModule, NgIcon, Breadcrumb],
  templateUrl: './prescripciones-hijo.html',
})
export class PrescripcionesHijo implements OnInit {
  private ninosService          = inject(NinosService);
  private prescripcionesService = inject(PrescripcionesService);
  private loadingBar            = inject(LoadingBar);
  private route                 = inject(ActivatedRoute);

  nino          = signal<NinoDetailResponse | null>(null);
  prescripciones = signal<PrescripcionResponse[]>([]);
  cargando      = signal(false);
  expandidaId   = signal<number | null>(null);

  readonly formatearFecha = formatearFecha;

  migajas: BreadcrumbItem[] = [{ label: 'Recetas' }];

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
          this.migajas = [{ label: `Recetas de ${r.data.nombre}` }];
        }
      }
    });

    this.prescripcionesService.obtenerHistorialPorNino(ninoId)
      .pipe(finalize(() => { this.cargando.set(false); this.loadingBar.complete(); }))
      .subscribe({
        next: (r) => {
          if (r.success) {
            const ordenadas = [...r.data].sort((a, b) =>
              new Date(b.fechaCreacion).getTime() - new Date(a.fechaCreacion).getTime()
            );
            this.prescripciones.set(ordenadas);
          }
        }
      });
  }

  alternar(id: number) {
    this.expandidaId.set(this.expandidaId() === id ? null : id);
  }
}
