import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { NinoResponse } from '../../../../shared/interfaces/responses/nino.response';
import { Ninos } from '../../../../core/services/ninos';
import { Auth } from '../../../../core/services/auth';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';

@Component({
  selector: 'app-editar-paciente',
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './editar-paciente.html',
  styleUrl: './editar-paciente.css',
})
export class EditarPaciente implements OnInit {
  @Input() id!: string;

  private ninosService = inject(Ninos);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  readonly auth = inject(Auth);

  nino = signal<NinoResponse | null>(null);
  guardando = signal(false);
  error = signal('');

  form = {
    nombre: '',
    apellido: '',
    fechaNacimiento: '',
    sexo: ''
  };

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Editar Paciente' },
  ];

  ngOnInit() {
    this.loadingBar.show();

    this.ninosService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.nino.set(r.data);
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

    this.ninosService.actualizar(parseInt(this.id), this.form).subscribe({
      next: (r) => {
        if (r.success) {
          this.router.navigate(['/pacientes']);
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
