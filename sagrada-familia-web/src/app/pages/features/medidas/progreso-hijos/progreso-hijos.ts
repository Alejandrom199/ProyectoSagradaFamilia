import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { NinosService } from '../../../../core/services/ninos';
import { MedidasService } from '../../../../core/services/medidas';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';
import { MedidaResponse } from '../../../../shared/interfaces/medida.interface';
import { ESTADO_NUTRICIONAL_FONDO, ESTADO_NUTRICIONAL_TEXTO, ESTADO_NUTRICIONAL_BORDE, EstadoNutricional } from '../../../../shared/constants/estado-nutricional.constants';

interface EstadoInfo {
  titulo:      string;
  descripcion: string;
  icono:       string;
  fondo:       string;
  texto:       string;
  borde:       string;
}

@Component({
  selector: 'progreso-hijos',
  standalone: true,
  imports: [CommonModule, NgIcon, Breadcrumb],
  templateUrl: './progreso-hijos.html',
  styleUrl: './progreso-hijos.css'
})
export class ProgresoHijos implements OnInit {
  private ninosService  = inject(NinosService);
  private medidasService = inject(MedidasService);
  private loadingBar    = inject(LoadingBar);
  private route         = inject(ActivatedRoute);

  ninoSeleccionado = signal<NinoDetailResponse | null>(null);
  ultimaMedida     = signal<MedidaResponse | null>(null);
  todasMedidas     = signal<MedidaResponse[]>([]);
  cargando         = signal(false);
  hayDatos         = signal(false);

  readonly formatearFecha = formatearFecha;

  estadoBanner = computed<EstadoInfo | null>(() => {
    const m = this.ultimaMedida();
    return m ? this.estadoInfo(m.estadoNutricional) : null;
  });

  migajas: BreadcrumbItem[] = [{ label: 'Progreso del niño' }];

  ngOnInit() {
    const ninoIdParam = this.route.snapshot.queryParamMap.get('ninoId');
    if (ninoIdParam) {
      this.loadingBar.show();
      this.ninosService.obtenerPorId(parseInt(ninoIdParam, 10))
        .pipe(finalize(() => this.loadingBar.complete()))
        .subscribe({
          next: (r) => { if (r.success && r.data) this.seleccionarNino(r.data); },
          error: () => this.loadingBar.complete()
        });
    }
  }

  seleccionarNino(nino: NinoDetailResponse) {
    this.ninoSeleccionado.set(nino);
    this.cargando.set(true);
    this.loadingBar.show();

    this.medidasService.obtenerPorNino(nino.id)
      .pipe(finalize(() => { this.cargando.set(false); this.loadingBar.complete(); }))
      .subscribe({
        next: (r) => {
          if (r.success && r.data.length > 0) {
            this.hayDatos.set(true);
            const ordenadas = [...r.data].sort((a, b) =>
              new Date(b.fechaMedicion).getTime() - new Date(a.fechaMedicion).getTime()
            );
            this.ultimaMedida.set(ordenadas[0]);
            this.todasMedidas.set(ordenadas);
          } else {
            this.hayDatos.set(false);
            this.ultimaMedida.set(null);
            this.todasMedidas.set([]);
          }
        },
        error: () => {
          this.hayDatos.set(false);
          this.ultimaMedida.set(null);
          this.todasMedidas.set([]);
        }
      });
  }

  formatearEdad(meses: number): string {
    if (meses < 24) return `${meses} meses`;
    const anios = Math.floor(meses / 12);
    const rest  = meses % 12;
    if (rest === 0) return `${anios} año${anios > 1 ? 's' : ''}`;
    return `${anios} año${anios > 1 ? 's' : ''} y ${rest} mes${rest > 1 ? 'es' : ''}`;
  }

  private estadoInfo(estado: string): EstadoInfo {
    const textos: Record<EstadoNutricional, { titulo: string; descripcion: string; icono: string }> = {
      Normal:         { titulo: '¡Todo va bien!', descripcion: 'Tu hijo tiene un peso saludable para su edad y estatura.',               icono: 'matFavoriteBorderOutline' },
      BajoPeso:       { titulo: 'Peso bajo',      descripcion: 'Te recomendamos comentarlo con el pediatra en la próxima visita.',       icono: 'matWarningAmberOutline' },
      BajoPesoSevero: { titulo: 'Peso muy bajo',  descripcion: 'Por favor consulta al médico lo antes posible.',                         icono: 'matErrorOutline' },
      Sobrepeso:      { titulo: 'Peso elevado',   descripcion: 'El pediatra puede orientarte con recomendaciones en la próxima visita.', icono: 'matWarningAmberOutline' },
      Obesidad:       { titulo: 'Obesidad',       descripcion: 'Por favor consulta al médico lo antes posible.',                         icono: 'matErrorOutline' },
    };
    const clave = estado as EstadoNutricional;
    const texto = textos[clave] ?? { titulo: estado, descripcion: '', icono: 'matInfoOutline' };
    return {
      ...texto,
      fondo: ESTADO_NUTRICIONAL_FONDO[clave] ?? 'bg-[var(--color-surface-alt)]',
      texto: ESTADO_NUTRICIONAL_TEXTO[clave] ?? 'text-muted',
      borde: ESTADO_NUTRICIONAL_BORDE[clave] ?? 'border-[var(--color-surface-border)]',
    };
  }
}
