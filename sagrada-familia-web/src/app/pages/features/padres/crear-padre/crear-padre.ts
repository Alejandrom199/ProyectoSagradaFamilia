import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { Padres } from '../../../../core/services/padres';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';

@Component({
  selector: 'app-crear-padre',
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './crear-padre.html',
  styleUrl: './crear-padre.css',
})
export class CrearPadre {
  private padresService = inject(Padres);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  guardando = signal(false);
  error = signal('');

  form = {
    nombre: '',
    apellido: '',
    email: '',
    password: '',
    telefono: ''
  };

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Crear Padre' },
  ];

  guardar() {
    if (!this.form.nombre || !this.form.apellido || !this.form.email || !this.form.password) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    this.padresService.crear(this.form).subscribe({
      next: (r) => {
        if (r.success) {
          this.router.navigate(['/padres']);
        }
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al crear el padre.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
