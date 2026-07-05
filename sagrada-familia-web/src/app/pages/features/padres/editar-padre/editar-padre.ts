import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';

import { PadreDetailResponse, PadreUpdate } from '../../../../shared/interfaces/padre.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PadresService } from '../../../../core/services/padres';
import { MedicosService } from '../../../../core/services/medicos';
import { AuthService } from '../../../../core/services/auth';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';

@Component({
  selector: 'app-editar-padre',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, Breadcrumb, NgIcon],
  templateUrl: './editar-padre.html',
  styleUrl: './editar-padre.css',
})
export class EditarPadre implements OnInit {
  @Input() id!: string;

  private fb             = inject(FormBuilder);
  private padresService  = inject(PadresService);
  private medicosService = inject(MedicosService);
  readonly authService    = inject(AuthService);
  private router         = inject(Router);
  private loadingBar     = inject(LoadingBar);

  formPadre!: FormGroup;
  padre      = signal<PadreDetailResponse | null>(null);
  medicos    = signal<MedicoResponse[]>([]);
  guardando  = signal(false);
  error      = signal<string | null>(null);

  // ── Cambio de correo ────────────────────────────────────────────
  mostrarCambioEmail = signal(false);
  nuevoEmail         = signal('');
  guardandoEmail     = signal(false);
  errorEmail         = signal<string | null>(null);
  exitoEmail         = signal(false);

  // ── Reasignar médico ─────────────────────────────────────────────
  mostrarCambioMedico = signal(false);
  nuevoMedicoId        = signal<number | null>(null);
  guardandoMedico      = signal(false);
  errorMedico          = signal<string | null>(null);
  exitoMedico          = signal(false);

  migajas: BreadcrumbItem[] = [];

  ngOnInit(): void {
    this.migajas = this.authService.esAdmin()
      ? [{ label: 'Usuarios', ruta: '/usuarios' }, { label: 'Editar Perfil' }]
      : [{ label: 'Padres', ruta: '/padres' }, { label: 'Editar Perfil' }];

    this.formPadre = this.initForm();
    this.cargarDatosPadre();

    if (this.authService.esAdmin()) {
      this.medicosService.obtenerTodos().subscribe({
        next: (r) => { if (r.success) this.medicos.set(r.data); }
      });
    }
  }

  private initForm(): FormGroup {
    return this.fb.group({
      nombre:   ['', [Validators.required, Validators.minLength(3)]],
      apellido: ['', [Validators.required, Validators.minLength(3)]],
      telefono: ['', [Validators.pattern('^[0-9]{10}$')]]
    });
  }

  get f() { return this.formPadre.controls; }

  private cargarDatosPadre(): void {
    this.loadingBar.show();
    this.padresService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (res: ApiResponse<PadreDetailResponse>) => {
        if (res.success) {
          this.formPadre.patchValue({
            nombre:   res.data.nombre,
            apellido: res.data.apellido,
            telefono: res.data.telefono ?? ''
          });
          this.nuevoMedicoId.set(res.data.medicoId);
          this.padre.set(res.data);
        }
      },
      error: () => {
        this.error.set('No se pudo cargar la información del representante.');
        this.loadingBar.complete();
      },
      complete: () => this.loadingBar.complete()
    });
  }

  guardar(): void {
    if (this.formPadre.invalid) { this.formPadre.markAllAsTouched(); return; }

    this.guardando.set(true);
    this.loadingBar.show();

    const request: PadreUpdate = {
      nombre:   this.formPadre.value.nombre,
      apellido: this.formPadre.value.apellido,
      telefono: this.formPadre.value.telefono || ''
    };

    this.padresService.actualizar(parseInt(this.id), request).subscribe({
      next: (res: ApiResponse<PadreDetailResponse>) => {
        if (res.success) this.router.navigate(['/padres']);
        else this.error.set(res.message);
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al actualizar los datos.');
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      complete: () => { this.guardando.set(false); this.loadingBar.complete(); }
    });
  }

  cambiarEmail(): void {
    const email = this.nuevoEmail().trim();
    if (!email) return;

    this.guardandoEmail.set(true);
    this.errorEmail.set(null);

    this.padresService.cambiarEmail(parseInt(this.id), { email }).subscribe({
      next: (res) => {
        if (res.success) {
          const p = this.padre();
          if (p) this.padre.set({ ...p, email });
          this.mostrarCambioEmail.set(false);
          this.nuevoEmail.set('');
          this.exitoEmail.set(true);
          setTimeout(() => this.exitoEmail.set(false), 4000);
        } else {
          this.errorEmail.set(res.message);
        }
      },
      error: (err) => {
        this.errorEmail.set(err.error?.message ?? 'Error al cambiar el correo electrónico.');
      },
      complete: () => this.guardandoEmail.set(false)
    });
  }

  toggleCambioEmail(): void {
    this.mostrarCambioEmail.update(v => !v);
    this.errorEmail.set(null);
    this.nuevoEmail.set('');
  }

  cambiarMedico(): void {
    const medicoId = this.nuevoMedicoId();
    if (!medicoId) return;

    this.guardandoMedico.set(true);
    this.errorMedico.set(null);

    this.padresService.cambiarMedico(parseInt(this.id), { medicoId }).subscribe({
      next: (res) => {
        if (res.success) {
          const medico = this.medicos().find(m => m.id === medicoId);
          const p = this.padre();
          if (p && medico) this.padre.set({ ...p, medicoId, medicoNombreCompleto: `${medico.nombre} ${medico.apellido}` });
          this.mostrarCambioMedico.set(false);
          this.exitoMedico.set(true);
          setTimeout(() => this.exitoMedico.set(false), 4000);
        } else {
          this.errorMedico.set(res.message);
        }
      },
      error: (err) => {
        this.errorMedico.set(err.error?.message ?? 'Error al reasignar el médico.');
      },
      complete: () => this.guardandoMedico.set(false)
    });
  }

  toggleCambioMedico(): void {
    this.mostrarCambioMedico.update(v => !v);
    this.errorMedico.set(null);
    this.nuevoMedicoId.set(this.padre()?.medicoId ?? null);
  }
}
