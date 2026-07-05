import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';


import { MedicoDetailResponse, MedicoUpdate } from '../../../../shared/interfaces/medico.interface';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { MedicosService } from '../../../../core/services/medicos';

@Component({
  selector: 'app-editar-medico',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Breadcrumb, NgIcon],

  templateUrl: './editar-medico.html',
  styleUrl: './editar-medico.css',
})
export class EditarMedico implements OnInit {
  @Input() id!: string;

  private fb = inject(FormBuilder);
  private medicosService = inject(MedicosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formMedico!: FormGroup;
  medico = signal<MedicoDetailResponse | null>(null);
  guardando = signal<boolean>(false);
  error = signal<string | null>(null);

  migajas: BreadcrumbItem[] = [
    { label: 'Usuarios', ruta: '/usuarios' },
    { label: 'Editar Médico' },
  ];

  ngOnInit(): void {
    this.formMedico = this.initForm();
    this.cargarDatosMedico();
  }

  private initForm(): FormGroup {
    return this.fb.group({
      nombre: ['', [Validators.required, Validators.minLength(3)]],
      apellido: ['', [Validators.required, Validators.minLength(3)]],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      especialidad: [''],
      telefono: ['', [Validators.pattern('^[0-9]{10}$')]]
    });
  }

  get f() {
    return this.formMedico.controls;
  }

  private cargarDatosMedico(): void {
    this.loadingBar.show();
    const medicoId = parseInt(this.id);

    this.medicosService.obtenerPorId(medicoId).subscribe({
      next: (res) => {
        if (res.success) {
          this.medico.set(res.data);
          this.formMedico.patchValue({
            nombre: res.data.nombre,
            apellido: res.data.apellido,
            email: res.data.email,
            especialidad: res.data.especialidad ?? '',
            telefono: res.data.telefono ?? ''
          });
        }
      },
      error: () => {
        this.error.set('No se pudo cargar la información del médico.');
        this.loadingBar.complete();
      },
      complete: () => this.loadingBar.complete()
    });
  }

  guardar(): void {
    if (this.formMedico.invalid) {
      this.formMedico.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    const request: MedicoUpdate = {
      nombre: this.formMedico.value.nombre,
      apellido: this.formMedico.value.apellido,
      especialidad: this.formMedico.value.especialidad || undefined,
      telefono: this.formMedico.value.telefono || undefined
    };

    this.medicosService.actualizar(parseInt(this.id), request).subscribe({
      next: (res) => {
        if (res.success) this.router.navigate(['/medicos']);
        else this.error.set(res.message);
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al actualizar.');
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      complete: () => {
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
