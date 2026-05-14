import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { AlimentosService } from '../../../../core/services/alimentos';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";
import { AlimentoResponse, AlimentoUpdate, CategoriaResponse } from '../../../../shared/interfaces/alimento.interface';

@Component({
  selector: 'app-editar-alimento',
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
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
    { label: 'Editar alimento' }
  ];

  ngOnInit() {
    this.loadingBar.show();

    // // Cargar categorías
    // this.alimentosService.obtenerCategorias().subscribe(r => {
    //   if (r.success) this.categorias.set(r.data);
    // });

    // // Cargar datos del alimento
    // this.alimentosService.obtenerTodos().subscribe({
    //   next: (r) => {
    //     if (r.success) {
    //       const encontrado = r.data.find(a => a.id === parseInt(this.id));
    //       if (encontrado) {
    //         this.alimento.set(encontrado);
    //         this.form = {
    //           categoriaId: encontrado.categoriaId,
    //           nombre: encontrado.nombre,
    //           descripcion: encontrado.descripcion ?? '',
    //           edadMinimaIntro: encontrado.edadMinimaIntro,
    //           edadMaxima: encontrado.edadMaxima,
    //           recomendacion: encontrado.recomendacion ?? ''
    //         };
    //       }
    //     }
    //     this.loadingBar.complete();
    //   },
    //   error: () => this.loadingBar.complete()
    // });
  }

  // guardar() {
  //   if (!this.form.nombre || !this.form.categoriaId || !this.form.edadMinimaIntro) {
  //     this.error.set('Completá nombre, categoría y edad mínima.');
  //     return;
  //   }

  //   this.guardando.set(true);
  //   this.loadingBar.show();

  //   const request: AlimentoUpdate = {
  //     categoriaId: this.form.categoriaId,
  //     nombre: this.form.nombre,
  //     descripcion: this.form.descripcion || undefined,
  //     edadMinimaIntro: this.form.edadMinimaIntro,
  //     edadMaxima: this.form.edadMaxima ?? undefined,
  //     recomendacion: this.form.recomendacion || undefined
  //   };

  //   this.alimentosService.actualizar(parseInt(this.id), request).subscribe({
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
