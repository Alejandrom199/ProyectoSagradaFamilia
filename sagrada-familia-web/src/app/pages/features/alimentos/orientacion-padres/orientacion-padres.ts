import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { forkJoin, of, switchMap, finalize } from 'rxjs';

import {
  heroVariable, heroFaceSmile, heroCake, heroGlobeAlt,
  heroBeaker, heroSparkles, heroExclamationCircle
} from '@ng-icons/heroicons/outline';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { CategoriaFiltro, ExploradorVisual, ItemExplorador } from '../../../../shared/components/explorador-visual/explorador-visual';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { AlimentosService } from '../../../../core/services/alimentos';
import { NinosService } from '../../../../core/services/ninos';
import { AlimentoResponse, CategoriaResponse } from '../../../../shared/interfaces/alimento.interface';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';

@Component({
  selector: 'orientacion-padres',
  standalone: true,
  imports: [CommonModule, NgIcon, ExploradorVisual, Breadcrumb],
  viewProviders: [provideIcons({
    heroVariable, heroFaceSmile, heroCake, heroGlobeAlt,
    heroBeaker, heroSparkles, heroExclamationCircle
  })],
  templateUrl: './orientacion-padres.html'
})
export class OrientacionPadres implements OnInit {
  private readonly alimentosService = inject(AlimentosService);
  private readonly ninosService     = inject(NinosService);
  private readonly route            = inject(ActivatedRoute);
  private readonly loadingBar       = inject(LoadingBar);

  alimentos         = signal<AlimentoResponse[]>([]);
  categorias        = signal<CategoriaResponse[]>([]);
  ninoSeleccionado  = signal<NinoDetailResponse | null>(null);
  cargando          = signal(true);
  error             = signal('');

  migajas: BreadcrumbItem[] = [{ label: 'Orientación Alimentaria' }];

  itemsParaElNino = computed<ItemExplorador[]>(() => {
    const nino = this.ninoSeleccionado();
    if (!nino) return [];

    return this.alimentos()
      .filter(a => a.edadMinimaMeses <= nino.edadMeses)
      .map(a => ({
        id:             a.id,
        nombre:         a.nombre,
        descripcion:    a.recomendacion || 'Sin recomendaciones específicas.',
        categoriaId:    a.categoriaId,
        categoriaNombre: a.categoriaNombre,
        badge:          `A partir de los ${a.edadMinimaMeses} meses`,
      }));
  });

  categoriasMapeadas = computed<CategoriaFiltro[]>(() =>
    this.categorias().map(c => ({
      id:     c.id,
      nombre: c.nombre,
      icono:  this.estiloCategoria(c.nombre).icono,
      color:  this.estiloCategoria(c.nombre).color,
    }))
  );

  ngOnInit() {
    this.cargarDatos();
  }

  private cargarDatos() {
    this.loadingBar.show();
    this.cargando.set(true);
    this.error.set('');

    // Cargar alimentos y categorías en paralelo
    forkJoin({
      alimentos:  this.alimentosService.obtenerTodos(),
      categorias: this.alimentosService.obtenerCategorias(),
    }).subscribe({
      next: ({ alimentos, categorias }) => {
        if (alimentos.success)  this.alimentos.set(alimentos.data);
        if (categorias.success) this.categorias.set(categorias.data);
      },
      error: () => this.error.set('No se pudieron cargar los alimentos.')
    });

    const ninoIdParam = this.route.snapshot.queryParamMap.get('ninoId');

    if (ninoIdParam) {
      // Hay ninoId en la URL → cargar ese niño directamente
      this.ninosService.obtenerPorId(parseInt(ninoIdParam, 10))
        .pipe(finalize(() => { this.cargando.set(false); this.loadingBar.complete(); }))
        .subscribe({
          next: (r) => { if (r.success && r.data) this.ninoSeleccionado.set(r.data); },
          error: () => this.error.set('No se pudo cargar la información del niño.')
        });
    } else {
      // Sin ninoId → auto-seleccionar el primer hijo del padre
      this.ninosService.obtenerMisNinos()
        .pipe(
          switchMap(r => {
            if (r.success && r.data?.length > 0) {
              return this.ninosService.obtenerPorId(r.data[0].id);
            }
            return of(null);
          }),
          finalize(() => { this.cargando.set(false); this.loadingBar.complete(); })
        )
        .subscribe({
          next: (res) => {
            if (res?.success && res.data) this.ninoSeleccionado.set(res.data);
          },
          error: () => this.error.set('No se pudo cargar la información del niño.')
        });
    }
  }

  private estiloCategoria(nombre: string): { icono: string; color: string } {
    const mapa: Record<string, { icono: string; color: string }> = {
      'Frutas':               { icono: 'heroFaceSmile', color: 'bg-orange-100 text-orange-600' },
      'Verduras':             { icono: 'heroVariable',  color: 'bg-green-100 text-green-600' },
      'Proteínas':            { icono: 'heroCake',      color: 'bg-red-100 text-red-600' },
      'Cereales y tubérculos':{ icono: 'heroGlobeAlt',  color: 'bg-amber-100 text-amber-700' },
      'Lácteos':              { icono: 'heroBeaker',    color: 'bg-blue-100 text-blue-600' },
    };
    return mapa[nombre] ?? { icono: 'heroSparkles', color: 'bg-slate-100 text-slate-600' };
  }
}
