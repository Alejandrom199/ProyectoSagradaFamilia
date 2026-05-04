import { Component, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { PadreResponse } from '../../../../shared/interfaces/responses/padre.response';
import { Ninos } from '../../../../core/services/ninos';
import { Padres } from '../../../../core/services/padres';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';


@Component({
  selector: 'app-crear-hijo',
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './crear-hijo.html',
  styleUrl: './crear-hijo.css',
})
export class CrearHijo {
  @Input() id!: string;

  private ninosService = inject(Ninos);
  private padresService = inject(Padres);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  padre = signal<PadreResponse | null>(null);
  guardando = signal(false);
  error = signal('');

  form = {
    nombre: '',
    apellido: '',
    fechaNacimiento: '',
    sexo: ''
  };

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Detalle del Padre', ruta: '/padres/:id' },
    { label: 'Crear Hijo' },
  ];

  ngOnInit() {
    this.padresService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          const p = r.data.find(x => x.id === parseInt(this.id));
          if (p) this.padre.set(p);
        }
      }
    });
  }

  guardar() {
    if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    const request = {
      representanteId: parseInt(this.id),
      ...this.form
    };

    this.ninosService.crear(request).subscribe({
      next: (r) => {
        if (r.success) {
          this.router.navigate(['/padres', this.id, 'hijos']);
        }
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al registrar el hijo.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
