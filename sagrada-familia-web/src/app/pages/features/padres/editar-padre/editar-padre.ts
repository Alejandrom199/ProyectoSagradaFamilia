import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';

import { PadreDetailResponse, PadreUpdate } from '../../../../shared/interfaces/padre.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PadresService } from '../../../../core/services/padres';

@Component({
  selector: 'app-editar-padre',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Breadcrumb, NgIcon],
  templateUrl: './editar-padre.html',
  styleUrl: './editar-padre.css',
})
export class EditarPadre implements OnInit {
  @Input() id!: string;

  private fb            = inject(FormBuilder);
  private padresService = inject(PadresService);
  private router        = inject(Router);
  private loadingBar    = inject(LoadingBar);

  formPadre!: FormGroup;
  padre      = signal<PadreDetailResponse | null>(null);
  guardando  = signal(false);
  error      = signal<string | null>(null);

  // ── Cambio de correo ────────────────────────────────────────────
  mostrarCambioEmail = signal(false);
  nuevoEmail         = signal('');
  guardandoEmail     = signal(false);
  errorEmail         = signal<string | null>(null);
  exitoEmail         = signal(false);

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Editar Perfil' },
  ];

  ngOnInit(): void {
    this.formPadre = this.initForm();
    this.cargarDatosPadre();
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
      telefono: this.formPadre.value.telefono || '',
      medicoId: this.padre()?.medicoId ?? 0
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
}
