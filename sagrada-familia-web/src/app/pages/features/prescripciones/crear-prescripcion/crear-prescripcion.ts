import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { provideIcons, NgIcon } from '@ng-icons/core';
import { heroBeaker, heroClipboardDocumentList, heroUser, heroCalendarDays } from '@ng-icons/heroicons/outline';
import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { CitasService } from '../../../../core/services/citas';
import { PrescripcionesService } from '../../../../core/services/prescripciones';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PrescripcionCreate } from '../../../../shared/interfaces/prescripcion.interface';
import { CitaResponse } from '../../../../shared/interfaces/cita.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'app-crear-prescripcion',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule],
  viewProviders: [provideIcons({ heroBeaker, heroClipboardDocumentList, heroUser, heroCalendarDays })],
  templateUrl: './crear-prescripcion.html',
})
export class CrearPrescripcion implements OnInit {
  @Input() citaId!: string;

  private fb = inject(FormBuilder);
  private prescripcionesService = inject(PrescripcionesService);
  private citasService = inject(CitasService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formPrescripcion!: FormGroup;
  cita = signal<CitaResponse | null>(null);
  guardando = signal(false);
  error = signal<string | null>(null);
  formatearFecha = formatearFecha;

  migajas: BreadcrumbItem[] = [
    { label: 'Citas', ruta: '/citas' },
    { label: 'Nueva receta' },
  ];

  ngOnInit(): void {
    this.formPrescripcion = this.fb.group({
      detalleMedicamentos: ['', [Validators.required, Validators.minLength(5)]],
      diagnostico: [''],
      indicaciones: ['']
    });
    this.cargarCita();
  }

  get f() { return this.formPrescripcion.controls; }

  private cargarCita(): void {
    this.loadingBar.show();
    this.citasService.obtenerPorId(parseInt(this.citaId)).subscribe({
      next: (r) => {
        if (r.success) {
          this.cita.set(r.data);
          this.migajas = [
            { label: 'Citas', ruta: '/citas' },
            { label: 'Consulta', ruta: `/citas/${this.citaId}` },
            { label: 'Nueva receta' },
          ];
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  guardar(): void {
    if (this.formPrescripcion.invalid) {
      this.formPrescripcion.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const data: PrescripcionCreate = {
      citaId: parseInt(this.citaId),
      detalleMedicamentos: this.formPrescripcion.value.detalleMedicamentos,
      diagnostico: this.formPrescripcion.value.diagnostico || undefined,
      indicaciones: this.formPrescripcion.value.indicaciones || undefined,
    };

    this.prescripcionesService.crear(data)
      .pipe(finalize(() => {
        this.guardando.set(false);
        this.loadingBar.complete();
      }))
      .subscribe({
        next: (res) => {
          if (res.success) this.router.navigate(['/citas', this.citaId]);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error al registrar la receta.')
      });
  }
}