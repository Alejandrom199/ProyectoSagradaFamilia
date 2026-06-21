import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { AlimentosService } from '../../../../core/services/alimentos';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";
import { AlimentoResponse, AlimentoUpdate, CategoriaResponse } from '../../../../shared/interfaces/alimento.interface';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';

@Component({
  selector: 'editar-alimento',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, SearchableSelect],

  templateUrl: './editar-alimento.html',
  styleUrl: './editar-alimento.css',
})
export class EditarAlimento implements OnInit {
  @Input() id!: string;

  private alimentosService = inject(AlimentosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  alimento = signal<AlimentoResponse | null>(null);
  categorias = signal<CategoriaResponse[]>([]);
  guardando = signal(false);
  error = signal('');
  submitted = signal(false);

  form = {
    categoriaId: null as number | null,
    nombre: '',
    descripcion: '',
    edadMinimaMeses: null as number | null,
    recomendacion: ''
  };

  migajas: BreadcrumbItem[] = [
    { label: 'Alimentos', ruta: '/alimentos' },
    { label: 'Editar alimento' }
  ];

  ngOnInit() {
    this.loadingBar.show();

    this.alimentosService.obtenerCategorias().subscribe(r => {
      if (r.success) this.categorias.set(r.data);
    });

    this.alimentosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          const encontrado = r.data.find(a => a.id === parseInt(this.id));
          if (encontrado) {
            this.alimento.set(encontrado);
            this.form = {
              categoriaId: encontrado.categoriaId,
              nombre: encontrado.nombre,
              descripcion: encontrado.descripcion ?? '',
              edadMinimaMeses: encontrado.edadMinimaMeses,
              recomendacion: encontrado.recomendacion ?? ''
            };
          }
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
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

    const request: AlimentoUpdate = {
      categoriaId: this.form.categoriaId,
      nombre: this.form.nombre.trim(),
      descripcion: this.form.descripcion || undefined,
      edadMinimaMeses: this.form.edadMinimaMeses,
      recomendacion: this.form.recomendacion || undefined,
      activo: this.alimento()?.activo ?? true
    };

    this.alimentosService.actualizar(parseInt(this.id), request).subscribe({
      next: (r) => {
        if (r.success) this.router.navigate(['/alimentos']);
        else this.error.set(r.message);
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al actualizar.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
