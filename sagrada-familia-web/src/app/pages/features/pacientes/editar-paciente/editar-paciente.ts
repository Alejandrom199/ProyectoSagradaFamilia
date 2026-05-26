import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { NinosService } from '../../../../core/services/ninos';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';

@Component({
  selector: 'app-editar-paciente',
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon],

  templateUrl: './editar-paciente.html',
})
export class EditarPaciente implements OnInit {
  @Input() id!: string;

  private ninosService = inject(NinosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  nino = signal<NinoDetailResponse | null>(null);
  guardando = signal(false);
  error = signal('');

  form = {
    nombre: '',
    apellido: '',
    fechaNacimiento: '',
    sexo: '' as 'M' | 'F' | '',
    medicoId: undefined as number | undefined
  };

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Editar Paciente' },
  ];

  ngOnInit(): void {
    this.loadingBar.show();
    this.ninosService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.nino.set(r.data);
          this.migajas = [
            { label: 'Pacientes', ruta: '/pacientes' },
            { label: `${r.data.nombre} ${r.data.apellido}`, ruta: `/pacientes/${this.id}` },
            { label: 'Editar' },
          ];
          this.form = {
            nombre: r.data.nombre,
            apellido: r.data.apellido,
            fechaNacimiento: r.data.fechaNacimiento,
            sexo: r.data.sexo,
            medicoId: r.data.medicoId
          };
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  guardar(): void {
    if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    this.ninosService.actualizar(parseInt(this.id), {
      nombre: this.form.nombre,
      apellido: this.form.apellido,
      fechaNacimiento: this.form.fechaNacimiento,
      sexo: this.form.sexo as 'M' | 'F',
      medicoId: this.form.medicoId
    }).subscribe({
      next: (r) => {
        if (r.success) this.router.navigate(['/pacientes', this.id]);
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