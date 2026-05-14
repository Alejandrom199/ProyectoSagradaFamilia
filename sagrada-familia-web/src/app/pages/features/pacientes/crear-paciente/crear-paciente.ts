import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";
import { NinosService } from '../../../../core/services/ninos';
import { PadresService } from '../../../../core/services/padres';
import { AuthService } from '../../../../core/services/auth';
import { PadreResponse } from '../../../../shared/interfaces/padre.interface';
import { NinoCreate, NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';

@Component({
  selector: 'app-crear-paciente',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './crear-paciente.html',
  styleUrl: './crear-paciente.css',
})
export class CrearPaciente implements OnInit {
  private ninosService = inject(NinosService);
  private padresService = inject(PadresService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  readonly auth = inject(AuthService);

  padres = signal<PadreResponse[]>([]);
  guardando = signal(false);
  error = signal('');

  // 💡 Tipado corregido para coincidir con NinoCreate
  form: {
    padreId: number;
    nombre: string;
    apellido: string;
    fechaNacimiento: string;
    sexo: 'M' | 'F' | '';
  } = {
      padreId: 0,
      nombre: '',
      apellido: '',
      fechaNacimiento: '',
      sexo: ''
    };

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Nuevo Paciente' },
  ];

  ngOnInit(): void {
    if (this.auth.esMedico()) {
      this.padresService.obtenerTodos().subscribe({
        next: (r: ApiResponse<PadreResponse[]>) => {
          if (r.success) this.padres.set(r.data);
        }
      });
    }
  }

  //   guardar(): void {
  //     if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
  //       this.error.set('Completá todos los campos obligatorios.');
  //       return;
  //     }

  //     if (this.auth.esMedico() && !this.form.padreId) {
  //       this.error.set('Debes seleccionar un padre representante.');
  //       return;
  //     }

  //     this.guardando.set(true);
  //     this.loadingBar.show();

  //     const request: NinoCreate = {
  //       nombre: this.form.nombre,
  //       apellido: this.form.apellido,
  //       fechaNacimiento: this.form.fechaNacimiento,
  //       sexo: this.form.sexo as 'M' | 'F',
  //       padreId: this.auth.esMedico() ? this.form.padreId : 0
  //     };

  //     this.ninosService.crear(request).subscribe({
  //       next: (r: ApiResponse<NinoResponse>) => {
  //         if (r.success) {
  //           this.router.navigate(['/pacientes']);
  //         }
  //         this.guardando.set(false);
  //         this.loadingBar.complete();
  //       },
  //       error: (err) => {
  //         this.error.set(err.error?.message ?? 'Error al registrar el paciente.');
  //         this.guardando.set(false);
  //         this.loadingBar.complete();
  //       }
  //     });
  //   }
}