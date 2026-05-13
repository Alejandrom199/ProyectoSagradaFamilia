import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { finalize } from 'rxjs';

import { heroArrowLeft, heroVariable, heroFaceSmile, heroCake, heroGlobeAlt, heroBeaker, heroSparkles } from '@ng-icons/heroicons/outline';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { CategoriaFiltro, ExploradorVisual, ItemExplorador } from '../../../../shared/components/explorador-visual/explorador-visual';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { AlimentosService } from '../../../../core/services/alimentos';
import { NinosService } from '../../../../core/services/ninos';
import { AlimentoResponse, CategoriaResponse } from '../../../../shared/interfaces/alimento.interface';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';

@Component({
  selector: 'orientacion-padres',
  standalone: true,
  imports: [CommonModule, ExploradorVisual, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft, heroVariable, heroFaceSmile, heroCake, heroGlobeAlt, heroBeaker, heroSparkles })],
  templateUrl: './orientacion-padres.html'
})
export class OrientacionPadres implements OnInit {
  private readonly alimentosService = inject(AlimentosService);
  private readonly ninosService = inject(NinosService);
  private readonly route = inject(ActivatedRoute);
  private readonly loadingBar = inject(LoadingBar);

  alimentos = signal<AlimentoResponse[]>([]);
  categorias = signal<CategoriaResponse[]>([]);
  ninoSeleccionado = signal<NinoResponse | null>(null);

  migajas: BreadcrumbItem[] = [
    { label: 'Orientación Alimentaria' }
  ];

  obtenerEstiloCategoria(nombre: string) {
    const mapa: Record<string, { icono: string, color: string }> = {
      'Frutas': { icono: 'heroFaceSmile', color: 'bg-orange-100 text-orange-600' },
      'Verduras': { icono: 'heroVariable', color: 'bg-green-100 text-green-600' },
      'Proteínas': { icono: 'heroCake', color: 'bg-red-100 text-red-600' },
      'Cereales y tubérculos': { icono: 'heroGlobeAlt', color: 'bg-amber-100 text-amber-700' },
      'Lácteos': { icono: 'heroBeaker', color: 'bg-blue-100 text-blue-600' }
    };
    return mapa[nombre] || { icono: 'heroSparkles', color: 'bg-gray-100 text-gray-600' };
  }

  itemsParaElNino = computed<ItemExplorador[]>(() => {
    const nino = this.ninoSeleccionado();
    if (!nino) return [];
    const edadActual = nino.edadMeses;

    return this.alimentos()
      .filter(a => a.edadMinimaIntro <= edadActual && (!a.edadMaxima || a.edadMaxima >= edadActual))
      .map(a => ({
        id: a.id,
        nombre: a.nombre,
        descripcion: a.recomendacion || 'Sin recomendaciones específicas.',
        categoriaId: a.categoriaId,
        categoriaNombre: a.categoriaNombre,
        badge: `A partir de los ${a.edadMinimaIntro} meses`,
        edadMaxima: a.edadMaxima || undefined
      }));
  });

  categoriasMapeadas = computed<CategoriaFiltro[]>(() => {
    return this.categorias().map(c => {
      const estilo = this.obtenerEstiloCategoria(c.nombre);
      return {
        id: c.id,
        nombre: c.nombre,
        icono: estilo.icono,
        color: estilo.color
      };
    });
  });

  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.loadingBar.show();

    this.alimentosService.obtenerTodos().subscribe({
      next: (r) => { if (r.success) this.alimentos.set(r.data); }
    });

    this.alimentosService.obtenerCategorias().subscribe({
      next: (r) => { if (r.success) this.categorias.set(r.data); }
    });

    const ninoIdParam = this.route.snapshot.queryParamMap.get('ninoId');
    if (ninoIdParam) {
      const idBuscar = parseInt(ninoIdParam);
      this.ninosService.obtenerPorId(idBuscar)
        .pipe(finalize(() => this.loadingBar.complete()))
        .subscribe({
          next: (r) => {
            if (r.success && r.data) {
              this.ninoSeleccionado.set(r.data);
            }
          }
        });
    } else {
      this.loadingBar.complete();
    }
  }
}