import { Component, Input, inject, signal, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PadreResponse } from '../../../../shared/interfaces/padre.interface';
import { NinoCreate, NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';
import { NinosService } from '../../../../core/services/ninos';
import { PadresService } from '../../../../core/services/padres';

@Component({
  selector: 'app-crear-hijo',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './crear-hijo.html',
  styleUrl: './crear-hijo.css',
})
export class CrearHijo implements OnInit {
  @Input() id!: string;

  private ninosService = inject(NinosService);
  private padresService = inject(PadresService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  padre = signal<PadreResponse | null>(null);
  guardando = signal(false);
  error = signal('');

  form: {
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: 'M' | 'F' | '';
  } = {
      nombre: '',
      apellido: '',
      fechaNacimiento: '',
      sexo: ''
    };

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Detalle del Padre', ruta: `/padres/${this.id}` },
    { label: 'Crear Hijo' },
  ];

  ngOnInit(): void {
    this.padresService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r: ApiResponse<PadreResponse>) => {
        if (r.success) this.padre.set(r.data);
      }
    });
  }

  // guardar(): void {
  //   if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
  //     this.error.set('Completá todos los campos obligatorios.');
  //     return;
  //   }

  //   this.guardando.set(true);
  //   this.loadingBar.show();

  // Ajustado al nuevo DTO 'NinoCreate' que exige medicoId
  const request: NinoCreate = {
    padreId: parseInt(this.id),
    nombre: this.form.nombre,
    apellido: this.form.apellido,
    fechaNacimiento: this.form.fechaNacimiento,
    sexo: this.form.sexo as 'M' | 'F',
    medicoId: this.padre()?.medicoId ?? 0
  };

  //   this.ninosService.crear(request).subscribe({
  //     next: (r: ApiResponse<NinoResponse>) => {
  //       if (r.success) {
  //         this.router.navigate(['/padres', this.id, 'hijos']);
  //       }
  //       this.guardando.set(false);
  //       this.loadingBar.complete();
  //     },
  //     error: (err) => {
  //       this.error.set(err.error?.message ?? 'Error al registrar el hijo.');
  //       this.guardando.set(false);
  //       this.loadingBar.complete();
  //     }
  //   });
  // }
}