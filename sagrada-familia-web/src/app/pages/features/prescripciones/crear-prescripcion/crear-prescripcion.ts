import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormArray, FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

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
  imports: [FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule],

  templateUrl: './crear-prescripcion.html',
})
export class CrearPrescripcion implements OnInit {
  @Input() consultaId!: string;

  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private prescripcionesService = inject(PrescripcionesService);
  private citasService = inject(CitasService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formPrescripcion!: FormGroup;
  cita = signal<CitaResponse | null>(null);
  guardando = signal(false);
  error = signal<string | null>(null);
  formatearFecha = formatearFecha;

  private citaId: string | null = null;

  migajas: BreadcrumbItem[] = [
    { label: 'Citas', ruta: '/citas' },
    { label: 'Nueva receta' },
  ];

  ngOnInit(): void {
    this.formPrescripcion = this.fb.group({
      medicamentos: this.fb.array([this.crearFilaMedicamento()]),
      indicaciones: ['']
    });

    this.citaId = this.route.snapshot.queryParamMap.get('citaId');
    if (this.citaId) this.cargarCita();
  }

  get medicamentos(): FormArray {
    return this.formPrescripcion.get('medicamentos') as FormArray;
  }

  crearFilaMedicamento(): FormGroup {
    return this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(200)]],
      presentacion: [''],
      dosis: ['', Validators.required],
      frecuencia: ['', Validators.required],
      viaAdministracion: ['', Validators.required],
      duracion: [''],
      cantidad: [''],
      observaciones: [''],
    });
  }

  agregarMedicamento(): void {
    this.medicamentos.push(this.crearFilaMedicamento());
  }

  quitarMedicamento(i: number): void {
    if (this.medicamentos.length > 1) this.medicamentos.removeAt(i);
  }

  cancelar(): void {
    this.router.navigate(this.citaId ? ['/citas', this.citaId] : ['/prescripciones']);
  }

  private cargarCita(): void {
    if (!this.citaId) return;
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
      consultaId: parseInt(this.consultaId),
      medicamentos: this.formPrescripcion.value.medicamentos,
      indicaciones: this.formPrescripcion.value.indicaciones || undefined,
    };

    this.prescripcionesService.crear(data)
      .pipe(finalize(() => {
        this.guardando.set(false);
        this.loadingBar.complete();
      }))
      .subscribe({
        next: (res) => {
          if (res.success) this.router.navigate(this.citaId ? ['/citas', this.citaId] : ['/prescripciones']);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error al registrar la receta.')
      });
  }
}
