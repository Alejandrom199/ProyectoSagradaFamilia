import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";
import { AlimentosService } from '../../../../core/services/alimentos';
import { AlimentoUpdate, CategoriaResponse } from '../../../../shared/interfaces/alimento.interface';

@Component({
  selector: 'crear-alimento',
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon],

  templateUrl: './crear-alimento.html',
  styleUrl: './crear-alimento.css',
})
export class CrearAlimento implements OnInit {
  private alimentosService = inject(AlimentosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  categorias = signal<CategoriaResponse[]>([]);
  guardando = signal(false);
  error = signal('');

  form = {
    categoriaId: 0,
    nombre: '',
    descripcion: '',
    edadMinimaIntro: 0,
    edadMaxima: null as number | null,
    recomendacion: ''
  };

  migajas: BreadcrumbItem[] = [
    { label: 'Alimentos', ruta: '/alimentos' },
    { label: 'Nuevo alimento' }
  ];

  ngOnInit() {
    this.alimentosService.obtenerCategorias().subscribe(r => {
      if (r.success) this.categorias.set(r.data);
    });
  }

  guardar() {
    //   if (!this.form.nombre || !this.form.categoriaId || !this.form.edadMinimaIntro) {
    //     this.error.set('Completá nombre, categoría y edad mínima.');
    //     return;
    //   }

    //   this.guardando.set(true);
    //   this.loadingBar.show();

    const request: AlimentoUpdate = {
      categoriaId: this.form.categoriaId,
      nombre: this.form.nombre,
      descripcion: this.form.descripcion || undefined,
      edadMinimaMeses: this.form.edadMinimaIntro,
      recomendacion: this.form.recomendacion || undefined,
      activo: true
    };

    //   this.alimentosService.crear(request).subscribe({
    //     next: (r) => {
    //       if (r.success) this.router.navigate(['/alimentos']);
    //       this.guardando.set(false);
    //       this.loadingBar.complete();
    //     },
    //     error: (err) => {
    //       this.error.set(err.error?.message ?? 'Error al actualizar.');
    //       this.guardando.set(false);
    //       this.loadingBar.complete();
    //     }
    //   });
    // }
  }
}
