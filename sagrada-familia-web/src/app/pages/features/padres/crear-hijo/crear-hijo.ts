import { Component, Input, inject, signal, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { provideIcons, NgIcon } from '@ng-icons/core';
import { heroArrowLeft, heroUserPlus } from '@ng-icons/heroicons/outline';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PadreDetailResponse, PadreResponse } from '../../../../shared/interfaces/padre.interface';
import { NinoCreate, NinoDetailResponse, NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';
import { NinosService } from '../../../../core/services/ninos';
import { PadresService } from '../../../../core/services/padres';

@Component({
  selector: 'app-crear-hijo',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon],
  viewProviders: [provideIcons({ heroArrowLeft, heroUserPlus })],
  templateUrl: './crear-hijo.html',
  styleUrl: './crear-hijo.css',
})
export class CrearHijo implements OnInit {
  @Input() id!: string;

  private ninosService = inject(NinosService);
  private padresService = inject(PadresService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  padre = signal<PadreDetailResponse | null>(null);
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

  migajas: BreadcrumbItem[] = [];

  ngOnInit(): void {
    this.padresService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.padre.set(r.data);
          this.migajas = [
            { label: 'Padres', ruta: '/padres' },
            { label: `${r.data.nombre} ${r.data.apellido}`, ruta: `/padres/${this.id}/hijos` },
            { label: 'Agregar hijo' },
          ];
        }
      }
    });
  }

  guardar(): void {
    if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    const request: NinoCreate = {
      padreId: parseInt(this.id),
      nombre: this.form.nombre,
      apellido: this.form.apellido,
      fechaNacimiento: this.form.fechaNacimiento,
      sexo: this.form.sexo as 'M' | 'F',
      medicoId: this.padre()?.medicoId ?? 0
    };

    this.ninosService.crear(request).subscribe({
      next: (r: ApiResponse<NinoDetailResponse>) => {
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