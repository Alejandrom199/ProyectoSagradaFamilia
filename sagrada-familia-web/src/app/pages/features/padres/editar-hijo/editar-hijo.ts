import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PadreResponse } from '../../../../shared/interfaces/padre.interface';
import { NinoResponse, NinoUpdate } from '../../../../shared/interfaces/nino.interface'; // 💡 Importamos el Update
import { ApiResponse } from '../../../../shared/interfaces/api.interface';
import { NinosService } from '../../../../core/services/ninos';
import { PadresService } from '../../../../core/services/padres';

@Component({
  selector: 'app-editar-hijo',
  standalone: true, // 💡 Añadido standalone para consistencia
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './editar-hijo.html',
  styleUrl: './editar-hijo.css',
})
export class EditarHijo implements OnInit {
  @Input() id!: string;
  @Input() hijoId!: string;

  private ninosService = inject(NinosService);
  private padresService = inject(PadresService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  padre = signal<PadreResponse | null>(null);
  hijo = signal<NinoResponse | null>(null);
  guardando = signal(false);
  error = signal('');

  // 💡 TIPADO EXPLÍCITO: Aquí es donde matamos el error
  form: {
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: 'M' | 'F' | ''; // Permitimos el vacío inicial pero restringimos el resto
  } = {
      nombre: '',
      apellido: '',
      fechaNacimiento: '',
      sexo: ''
    };

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Detalle del Padre', ruta: `/padres/${this.id}` }, // 💡 Template string para la ruta
    { label: 'Editar Hijo' },
  ];

  ngOnInit(): void {
    this.loadingBar.show();

    // Cargar datos del padre para el contexto
    this.padresService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r: ApiResponse<PadreResponse>) => {
        if (r.success) this.padre.set(r.data);
      }
    });

    // Cargar datos del hijo para el formulario
    this.ninosService.obtenerPorId(parseInt(this.hijoId)).subscribe({
      next: (r: ApiResponse<NinoResponse>) => {
        if (r.success) {
          this.hijo.set(r.data);
          this.form = {
            nombre: r.data.nombre,
            apellido: r.data.apellido,
            fechaNacimiento: r.data.fechaNacimiento,
            sexo: r.data.sexo as 'M' | 'F' // Cast seguro desde la respuesta
          };
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  guardar(): void {
    // Validación básica
    if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    const request: NinoUpdate = {
      nombre: this.form.nombre,
      apellido: this.form.apellido,
      fechaNacimiento: this.form.fechaNacimiento,
      sexo: this.form.sexo as 'M' | 'F' // TypeScript ahora está tranquilo porque ya validamos que no sea ''
    };

    this.ninosService.actualizar(parseInt(this.hijoId), request).subscribe({
      next: (r: ApiResponse<NinoResponse>) => {
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