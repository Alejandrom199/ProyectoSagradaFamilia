import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';
import { finalize } from 'rxjs';
import { PadresService } from '../../../../core/services/padres';
import { AuthService } from '../../../../core/services/auth';
import { MedicosService } from '../../../../core/services/medicos';
import { PadreCreate } from '../../../../shared/interfaces/padre.interface';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';

@Component({
  selector: 'crear-padre',
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule, SearchableSelect],
  templateUrl: './crear-padre.html',
  styleUrl: './crear-padre.css',
})
export class CrearPadre implements OnInit {
  private fb = inject(FormBuilder);
  private padresService = inject(PadresService);
  private medicosService = inject(MedicosService);
  readonly authService = inject(AuthService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  medicos = signal<MedicoResponse[]>([]);

  formPadre!: FormGroup;

  guardando = signal(false);
  error = signal<string | null>(null);

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Crear Padre' },
  ];

  readonly medicoLabelFn = (m: MedicoResponse) => `Dr(a). ${m.nombre} ${m.apellido}`;

  ngOnInit(): void {
    this.formPadre = this.initForm();
    const user = this.authService.currentUser();

    if (this.authService.esMedico() && user?.medicoId) {
      this.f['medicoId'].patchValue(user.medicoId);
      this.f['medicoId'].disable();
    } else if (this.authService.esAdmin()) {
      this.medicosService.obtenerTodos().subscribe({
        next: (r) => { if (r.success) this.medicos.set(r.data); }
      });
    }
  }

  private initForm(): FormGroup {
    return this.fb.group({
      medicoId: [this.authService.currentUser()?.medicoId ?? null, Validators.required],
      nombre: ['', [Validators.required, Validators.minLength(3)]],
      apellido: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      telefono: ['', [Validators.required, Validators.pattern('^[0-9]{10}$')]]
    });
  }

  get f() {
    return this.formPadre.controls;
  }

  guardar(): void {
    if (this.formPadre.invalid) {
      this.formPadre.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const data = this.formPadre.getRawValue() as PadreCreate;

    this.padresService.crear(data)
      .pipe(
        finalize(() => {
          this.guardando.set(false);
          this.loadingBar.complete();
        })
      )
      .subscribe({
        next: (res) => {
          if (res.success) this.router.navigate(['/padres']);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error crítico en el servidor. Intente más tarde.')
      });
  }
}
