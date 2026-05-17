import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  heroPencilSquare, heroEnvelope, heroPhone, heroChevronLeft, heroUser
} from '@ng-icons/heroicons/outline';

import { PadreDetailResponse, PadreResponse, PadreUpdate } from '../../../../shared/interfaces/padre.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PadresService } from '../../../../core/services/padres';

@Component({
  selector: 'app-editar-padre',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Breadcrumb, NgIcon],
  viewProviders: [provideIcons({ heroPencilSquare, heroEnvelope, heroPhone, heroChevronLeft, heroUser })],
  templateUrl: './editar-padre.html',
  styleUrl: './editar-padre.css',
})
export class EditarPadre implements OnInit {
  @Input() id!: string;

  private fb = inject(FormBuilder);
  private padresService = inject(PadresService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formPadre!: FormGroup;
  // SOLUCIÓN: Cambiado a PadreDetailResponse para que coincida con el valor del GET por ID
  padre = signal<PadreDetailResponse | null>(null);
  guardando = signal<boolean>(false);
  error = signal<string | null>(null);

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
      nombre: ['', [Validators.required, Validators.minLength(3)]],
      apellido: ['', [Validators.required, Validators.minLength(3)]],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      telefono: ['', [Validators.pattern('^[0-9]{10}$')]]
    });
  }

  get f() {
    return this.formPadre.controls;
  }

  private cargarDatosPadre(): void {
    this.loadingBar.show();
    const padreId = parseInt(this.id);

    this.padresService.obtenerPorId(padreId).subscribe({
      next: (res: ApiResponse<PadreDetailResponse>) => {
        if (res.success) {
          this.formPadre.patchValue({
            nombre: res.data.nombre,
            apellido: res.data.apellido,
            email: res.data.email,
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
    if (this.formPadre.invalid) {
      this.formPadre.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    const request: PadreUpdate = {
      nombre: this.formPadre.value.nombre,
      apellido: this.formPadre.value.apellido,
      telefono: this.formPadre.value.telefono || "",
      medicoId: this.padre()?.medicoId ?? 0
    };

    this.padresService.actualizar(parseInt(this.id), request).subscribe({
      next: (res: ApiResponse<PadreDetailResponse>) => {
        if (res.success) {
          this.router.navigate(['/padres']);
        } else {
          this.error.set(res.message);
        }
      },
      error: (err) => {
        const msg = err.error?.message ?? 'Error al actualizar los datos.';
        this.error.set(msg);
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