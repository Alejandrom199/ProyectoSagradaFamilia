import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { MedicosService } from '../../../../core/services/medicos';
import { CatalogoValoresService } from '../../../../core/services/catalogo-valores';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { MedicoCreate } from '../../../../shared/interfaces/medico.interface';
import { CatalogoValorResponse } from '../../../../shared/interfaces/catalogo-valor.interface';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';

const TIPO_ESPECIALIDAD_MEDICA = 'ESPECIALIDAD_MEDICA';

@Component({
  selector: 'app-crear-medico',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule, SearchableSelect],
  templateUrl: './crear-medico.html',
  styleUrl: './crear-medico.css',
})
export class CrearMedico implements OnInit {
  private fb             = inject(FormBuilder);
  private medicosService = inject(MedicosService);
  private catalogoValoresService = inject(CatalogoValoresService);
  private router         = inject(Router);
  private loadingBar     = inject(LoadingBar);

  formMedico!: FormGroup;
  especialidades = signal<CatalogoValorResponse[]>([]);

  guardando = signal(false);
  error     = signal<string | null>(null);

  migajas: BreadcrumbItem[] = [
    { label: 'Médicos', ruta: '/medicos' },
    { label: 'Crear Médico' },
  ];

  ngOnInit(): void {
    this.formMedico = this.initForm();
    this.catalogoValoresService.obtenerPorTipo(TIPO_ESPECIALIDAD_MEDICA).subscribe(res => {
      if (res.success) this.especialidades.set(res.data);
    });
  }

  private initForm(): FormGroup {
    return this.fb.group({
      nombre:       ['', [Validators.required, Validators.minLength(2)]],
      apellido:     ['', [Validators.required, Validators.minLength(2)]],
      email:        ['', [Validators.required, Validators.email]],
      especialidad: [''],
      telefono:     ['', [Validators.pattern('^[0-9]{10}$')]],
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

    const v = this.formMedico.value;
    const data: MedicoCreate = {
      email:        v.email,
      nombre:       v.nombre,
      apellido:     v.apellido,
      especialidad: v.especialidad || undefined,
      telefono:     v.telefono     || undefined,
    };

    this.medicosService.crear(data)
      .pipe(finalize(() => { this.guardando.set(false); this.loadingBar.complete(); }))
      .subscribe({
        next:  (res) => {
          if (res.success) this.router.navigate(['/medicos']);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error crítico en el servidor. Intente más tarde.'),
      });
  }
}
