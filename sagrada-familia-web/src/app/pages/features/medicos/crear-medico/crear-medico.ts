import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { MedicosService } from '../../../../core/services/medicos';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { MedicoCreate } from '../../../../shared/interfaces/medico.interface';
import { passwordValidator } from '../../../../shared/validators/password.validator';

@Component({
  selector: 'app-crear-medico',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule],

  templateUrl: './crear-medico.html',
  styleUrl: './crear-medico.css',
})
export class CrearMedico implements OnInit {
  private fb = inject(FormBuilder);
  private medicosService = inject(MedicosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formMedico!: FormGroup;

  guardando = signal(false);
  error = signal<string | null>(null);
  verPassword = signal(false);

  migajas: BreadcrumbItem[] = [
    { label: 'Médicos', ruta: '/medicos' },
    { label: 'Crear Médico' },
  ];

  ngOnInit(): void {
    this.formMedico = this.initForm();
  }

  private initForm(): FormGroup {
    return this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(3)]],
      apellido: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, passwordValidator]],
      especialidad: ['', [Validators.minLength(3)]],
      telefono: ['', [Validators.pattern('^[0-9]{10}$')]]
    });
  }

  get f() {
    return this.formMedico.controls;
  }

  guardar(): void {
    if (this.formMedico.invalid) {
      this.formMedico.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const data = this.formMedico.getRawValue() as MedicoCreate;

    this.medicosService.crear(data)
      .pipe(
        finalize(() => {
          this.guardando.set(false);
          this.loadingBar.complete();
        })
      )
      .subscribe({
        next: (res) => {
          if (res.success) this.router.navigate(['/medicos']);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error crítico en el servidor. Intente más tarde.')
      });
  }
}
