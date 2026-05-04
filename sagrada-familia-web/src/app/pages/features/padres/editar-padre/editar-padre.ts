import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { PadreResponse } from '../../../../shared/interfaces/responses/padre.response';
import { Padres } from '../../../../core/services/padres';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';

@Component({
  selector: 'app-editar-padre',
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './editar-padre.html',
  styleUrl: './editar-padre.css',
})
export class EditarPadre implements OnInit {
  @Input() id!: string;

  private padresService = inject(Padres);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  padre = signal<PadreResponse | null>(null);
  guardando = signal(false);
  error = signal('');

  form = {
    nombre: '',
    apellido: '',
    email: '',
    telefono: ''
  };

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Editar Padre' },
  ];

  ngOnInit() {
    this.loadingBar.show();
    this.padresService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.padre.set(r.data);
          this.form = {
            nombre: r.data.nombre,
            apellido: r.data.apellido,
            email: r.data.email,
            telefono: r.data.telefono ?? ''
          };
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  guardar() {
    if (!this.form.nombre || !this.form.apellido || !this.form.email) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    this.padresService.actualizar(parseInt(this.id), this.form).subscribe({
      next: (r) => {
        if (r.success) {
          this.router.navigate(['/padres']);
        }
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
