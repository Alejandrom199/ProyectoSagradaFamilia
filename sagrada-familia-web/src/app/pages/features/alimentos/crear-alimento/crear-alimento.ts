import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";
import { AlimentosService } from '../../../../core/services/alimentos';
import { AlimentoCreate, CategoriaResponse } from '../../../../shared/interfaces/alimento.interface';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';

@Component({
  selector: 'crear-alimento',
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, SearchableSelect],

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
  submitted = signal(false);

  form = {
    categoriaId: null as number | null,
    nombre: '',
    descripcion: '',
    edadMinimaMeses: null as number | null,
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
    this.submitted.set(true);
    this.error.set('');

    if (!this.form.nombre.trim() || !this.form.categoriaId || this.form.edadMinimaMeses === null || this.form.edadMinimaMeses < 0) {
      this.error.set('Completá los campos obligatorios marcados con *.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    const request: AlimentoCreate = {
      categoriaId: this.form.categoriaId,
      nombre: this.form.nombre.trim(),
      descripcion: this.form.descripcion || undefined,
      edadMinimaMeses: this.form.edadMinimaMeses,
      recomendacion: this.form.recomendacion || undefined,
    };

    this.alimentosService.crear(request).subscribe({
      next: (r) => {
        if (r.success) this.router.navigate(['/alimentos']);
        else this.error.set(r.message);
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al crear el alimento.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
