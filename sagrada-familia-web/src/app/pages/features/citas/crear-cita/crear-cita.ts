import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { provideIcons, NgIcon } from '@ng-icons/core';
import { heroCalendarDays, heroClock, heroDocumentText, heroPlus, heroUser } from '@ng-icons/heroicons/outline';
import { finalize } from 'rxjs';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { CitasService } from '../../../../core/services/citas';
import { NinosService } from '../../../../core/services/ninos';
import { AuthService } from '../../../../core/services/auth';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';
import { MedicosService } from '../../../../core/services/medicos';
import { CitaCreate } from '../../../../shared/interfaces/cita.interface';

@Component({
  selector: 'app-crear-cita',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule],
  viewProviders: [provideIcons({ heroCalendarDays, heroClock, heroDocumentText, heroPlus, heroUser })],
  templateUrl: './crear-cita.html',
  styleUrl: './crear-cita.css',
})
export class CrearCita implements OnInit {
  private fb = inject(FormBuilder);
  private citasService = inject(CitasService);
  private ninosService = inject(NinosService);
  private medicosService = inject(MedicosService);
  private authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formCita!: FormGroup;
  pacientes = signal<NinoResponse[]>([]);
  medicos = signal<MedicoResponse[]>([]);

  guardando = signal(false);
  error = signal<string | null>(null);

  migajas: BreadcrumbItem[] = [
    { label: 'Citas', ruta: '/citas' },
    { label: 'Programar cita' },
  ];

  ngOnInit(): void {
    this.formCita = this.initForm();

    const ninoIdParam = this.route.snapshot.queryParamMap.get('ninoId');
    if (ninoIdParam) {
      this.formCita.patchValue({ ninoId: parseInt(ninoIdParam) });
    }

    this.cargarPacientes();
    this.cargarMedicos();
  }

  private initForm(): FormGroup {
    return this.fb.group({
      ninoId: [null, Validators.required],
      medicoId: [null, Validators.required],
      fecha: ['', Validators.required],
      hora: ['', Validators.required],
      motivo: ['']
    });
  }

  get f() {
    return this.formCita.controls;
  }

  private cargarPacientes(): void {
    this.ninosService.obtenerTodos().subscribe({
      next: (r) => { if (r.success) this.pacientes.set(r.data); }
    });
  }

  private cargarMedicos(): void {
    this.medicosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          this.medicos.set(r.data);

          if (this.authService.esMedico()) {
            const user = this.authService.currentUser();
            const propio = r.data.find(m => m.email === user?.nombre);
            if (propio) this.formCita.patchValue({ medicoId: propio.id });
          }
        }
      }
    });
  }

  guardar(): void {
    if (this.formCita.invalid) {
      this.formCita.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const v = this.formCita.value;
    const fechaHora = new Date(`${v.fecha}T${v.hora}:00`).toISOString();

    const data: CitaCreate = {
      ninoId: v.ninoId,
      medicoId: v.medicoId,
      fechaHora,
      motivo: v.motivo || undefined
    };

    this.citasService.crear(data)
      .pipe(
        finalize(() => {
          this.guardando.set(false);
          this.loadingBar.complete();
        })
      )
      .subscribe({
        next: (res) => {
          if (res.success) this.router.navigate(['/citas']);
          else this.error.set(res.message);
        },
        error: () => this.error.set('Ocurrió un error al programar la cita. Intente más tarde.')
      });
  }
}
