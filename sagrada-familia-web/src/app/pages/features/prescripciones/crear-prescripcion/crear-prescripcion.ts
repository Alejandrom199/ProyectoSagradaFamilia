import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { provideIcons, NgIcon } from '@ng-icons/core';
import { heroBeaker, heroClipboardDocumentList, heroPlus, heroUser } from '@ng-icons/heroicons/outline';
import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { PrescripcionesService } from '../../../../core/services/prescripciones';
import { NinosService } from '../../../../core/services/ninos';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PrescripcionCreate } from '../../../../shared/interfaces/prescripcion.interface';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';

@Component({
  selector: 'app-crear-prescripcion',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule],
  viewProviders: [provideIcons({ heroBeaker, heroClipboardDocumentList, heroPlus, heroUser })],
  templateUrl: './crear-prescripcion.html',
  styleUrl: './crear-prescripcion.css',
})
export class CrearPrescripcion implements OnInit {
  private fb = inject(FormBuilder);
  private prescripcionesService = inject(PrescripcionesService);
  private ninosService = inject(NinosService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formPrescripcion!: FormGroup;
  pacientes = signal<NinoResponse[]>([]);

  guardando = signal(false);
  error = signal<string | null>(null);

  migajas: BreadcrumbItem[] = [
    { label: 'Prescripciones' },
    { label: 'Nueva receta' },
  ];

  ngOnInit(): void {
    this.formPrescripcion = this.initForm();

    const ninoIdParam = this.route.snapshot.queryParamMap.get('ninoId');
    if (ninoIdParam) {
      this.formPrescripcion.patchValue({ ninoId: parseInt(ninoIdParam) });
    }

    this.cargarPacientes();
  }

  private initForm(): FormGroup {
    return this.fb.group({
      ninoId: [null, Validators.required],
      detalleMedicamentos: ['', [Validators.required, Validators.minLength(5)]],
      indicaciones: ['']
    });
  }

  get f() {
    return this.formPrescripcion.controls;
  }

  private cargarPacientes(): void {
    this.ninosService.obtenerTodos().subscribe({
      next: (r) => { if (r.success) this.pacientes.set(r.data); }
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
      ninoId: this.formPrescripcion.value.ninoId,
      detalleMedicamentos: this.formPrescripcion.value.detalleMedicamentos,
      indicaciones: this.formPrescripcion.value.indicaciones || undefined
    };

    this.prescripcionesService.crear(data)
      .pipe(
        finalize(() => {
          this.guardando.set(false);
          this.loadingBar.complete();
        })
      )
      .subscribe({
        next: (res) => {
          if (res.success) {
            this.router.navigate(['/prescripciones/nino', data.ninoId]);
          } else {
            this.error.set(res.message);
          }
        },
        error: () => this.error.set('Ocurrió un error al registrar la receta. Intente más tarde.')
      });
  }
}
