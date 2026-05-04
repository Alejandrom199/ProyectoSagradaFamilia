import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { NinoResponse } from '../../../../shared/interfaces/responses/nino.response';
import { PadreResponse } from '../../../../shared/interfaces/responses/padre.response';
import { Ninos } from '../../../../core/services/ninos';
import { Padres } from '../../../../core/services/padres';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';

@Component({
  selector: 'app-editar-hijo',
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './editar-hijo.html',
  styleUrl: './editar-hijo.css',
})
export class EditarHijo implements OnInit {
  @Input() id!: string;
  @Input() hijoId!: string;

  private ninosService = inject(Ninos);
  private padresService = inject(Padres);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  padre = signal<PadreResponse | null>(null);
  hijo = signal<NinoResponse | null>(null);
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
    { label: 'Editar Hijo' },
  ];

  ngOnInit() {
    this.loadingBar.show();

    this.padresService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          const p = r.data.find(x => x.id === parseInt(this.id));
          if (p) this.padre.set(p);
        }
      }
    });

    this.ninosService.obtenerPorId(parseInt(this.hijoId)).subscribe({
      next: (r) => {
        if (r.success) {
          this.hijo.set(r.data);
          this.form = {
            nombre: r.data.nombre,
            apellido: r.data.apellido,
            fechaNacimiento: r.data.fechaNacimiento,
            sexo: r.data.sexo
          };
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  guardar() {
    if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    this.ninosService.actualizar(parseInt(this.hijoId), this.form).subscribe({
      next: (r) => {
        if (r.success) {
          this.router.navigate(['/padres', this.id, 'hijos']);
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
