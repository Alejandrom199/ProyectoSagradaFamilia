import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroArrowLeft } from '@ng-icons/heroicons/outline';
import { PadreResponse } from '../../../../shared/interfaces/responses/padre.response';
import { Ninos } from '../../../../core/services/ninos';
import { Padres } from '../../../../core/services/padres';
import { Auth } from '../../../../core/services/auth';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";

@Component({
  selector: 'app-crear-paciente',
  imports: [RouterLink, FormsModule, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft })],
  templateUrl: './crear-paciente.html',
  styleUrl: './crear-paciente.css',
})
export class CrearPaciente implements OnInit {
  private ninosService = inject(Ninos);
  private padresService = inject(Padres);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  readonly auth = inject(Auth);

  padres = signal<PadreResponse[]>([]);
  guardando = signal(false);
  error = signal('');

  form = {
    representanteId: 0,
    nombre: '',
    apellido: '',
    fechaNacimiento: '',
    sexo: ''
  };

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Nuevo Paciente' },
  ];

  ngOnInit() {
    if (this.auth.esMedico()) {
      this.padresService.obtenerTodos().subscribe(r => {
        if (r.success) this.padres.set(r.data);
      });
    }
  }

  guardar() {
    if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    if (this.auth.esMedico() && !this.form.representanteId) {
      this.error.set('Debes seleccionar un padre representante.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    // Si es padre, el backend usará su claim para asignar el representanteId.
    // Si es médico, enviará el representanteId seleccionado.
    const request = {
      ...this.form,
      representanteId: this.auth.esMedico() ? this.form.representanteId : 0
    };

    this.ninosService.crear(request).subscribe({
      next: (r) => {
        if (r.success) {
          this.router.navigate(['/pacientes']);
        }
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al registrar el paciente.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
