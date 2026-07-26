import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

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
import { ParametrosService } from '../../../../core/services/parametros';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';

@Component({
  selector: 'app-crear-cita',
  standalone: true,
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, ReactiveFormsModule, SearchableSelect],

  templateUrl: './crear-cita.html',
  styleUrl: './crear-cita.css',
})
export class CrearCita implements OnInit {
  private fb = inject(FormBuilder);
  private citasService = inject(CitasService);
  private ninosService = inject(NinosService);
  private medicosService = inject(MedicosService);
  private authService = inject(AuthService);
  private parametrosService = inject(ParametrosService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  formCita!: FormGroup;
  pacientes = signal<NinoResponse[]>([]);
  medicos   = signal<MedicoResponse[]>([]);

  readonly pacienteLabelFn = (n: NinoResponse)  => `${n.nombre} ${n.apellido}`;
  readonly medicoLabelFn   = (m: MedicoResponse) => `Dr(a). ${m.nombre} ${m.apellido}`;

  guardando = signal(false);
  error = signal<string | null>(null);

  horaInicioAtencion = signal<string | null>(null);
  horaFinAtencion = signal<string | null>(null);

  readonly fechaMinima = (() => {
    const hoy = new Date();
    return `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}-${String(hoy.getDate()).padStart(2, '0')}`;
  })();

  esMedico = signal(false);

  migajas: BreadcrumbItem[] = [
    { label: 'Citas', ruta: '/citas' },
    { label: 'Programar cita' },
  ];

  ngOnInit(): void {
    this.esMedico.set(this.authService.esMedico());
    this.formCita = this.initForm();

    const ninoIdParam = this.route.snapshot.queryParamMap.get('ninoId');
    if (ninoIdParam) {
      this.formCita.patchValue({ ninoId: parseInt(ninoIdParam) });
    }

    this.cargarPacientes();
    this.cargarMedicos();
    this.cargarHorarioAtencion();
  }

  private initForm(): FormGroup {
    return this.fb.group({
      ninoId:   [null, Validators.required],
      medicoId: [null, Validators.required],
      fecha:    ['', Validators.required],
      hora:     ['', Validators.required],
      horaFin:  ['', Validators.required],
      motivo:   ['']
    }, { validators: this.fechaHoraFuturaValidator });
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
            const medicoId = this.authService.currentUser()?.medicoId;
            if (medicoId) this.formCita.patchValue({ medicoId });
          }
        }
      }
    });
  }

  private cargarHorarioAtencion(): void {
    this.parametrosService.obtenerPorGrupo('HORARIO_ATENCION').subscribe({
        next: (r) => {
            if (r.success) {
                const inicio = r.data.find(p => p.codigo === 'HORA_INICIO' && p.activo);
                const fin = r.data.find(p => p.codigo === 'HORA_FIN' && p.activo);
                if (inicio) this.horaInicioAtencion.set(inicio.valor);
                if (fin) this.horaFinAtencion.set(fin.valor);
            }
        }
    });
  }

  guardar(): void {
    if (this.formCita.invalid) {
      this.formCita.markAllAsTouched();
      return;
    }

    const v = this.formCita.value;
    const horaSeleccionada = v.hora as string;

    const inicio = this.horaInicioAtencion();
    const fin = this.horaFinAtencion();

    if (inicio && horaSeleccionada < inicio) {
        this.error.set(`Las citas solo pueden agendarse a partir de las ${inicio} horas.`);
        return;
    }
    if (fin && horaSeleccionada >= fin) {
        this.error.set(`Las citas no pueden agendarse después de las ${fin} horas.`);
        return;
    }

    // Validaciones frontend ANTES de activar el spinner
    if (v.horaFin <= v.hora) {
      this.error.set('La hora de terminación debe ser posterior a la hora de inicio.');
      return;
    }

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    // Hora local sin conversión UTC: el sistema opera en una sola zona (Ecuador)
    const fechaHora    = `${v.fecha}T${v.hora}:00`;
    const fechaHoraFin = `${v.fecha}T${v.horaFin}:00`;

    const data: CitaCreate = {
      ninoId:       v.ninoId,
      medicoId:     v.medicoId,
      fechaHora,
      fechaHoraFin,
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
        error: (err) => this.error.set(err?.error?.message ?? 'Ocurrió un error al programar la cita. Intente más tarde.')
      });
  }

  private fechaHoraFuturaValidator(group: AbstractControl): ValidationErrors | null {
    const fecha = group.get('fecha')?.value;
    const hora = group.get('hora')?.value;
    if (!fecha || !hora) return null;

    const seleccionada = new Date(`${fecha}T${hora}:00`);
    const ahora = new Date();

    const hoy = new Date();
    const hoyStr = `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}-${String(hoy.getDate()).padStart(2, '0')}`;

    if (fecha > hoyStr) return null;
    if (fecha < hoyStr) return { fechaPasada: true };

    return seleccionada <= ahora ? { fechaPasada: true } : null;
  }

  get horaMinima(): string {
    const fechaSel = this.f['fecha'].value;
    if (fechaSel === this.fechaMinima) {
      const now = new Date();
      return `${String(now.getHours()).padStart(2, '0')}:${String(now.getMinutes()).padStart(2, '0')}`;
    }
    return '';
  }

  // Un minuto después de 'hora inicio': evita que el picker nativo permita
  // una hora de terminación igual o anterior a la de inicio.
  get horaFinMinima(): string {
    const horaInicio = this.f['hora'].value as string;
    if (!horaInicio) return '';

    const [h, m] = horaInicio.split(':').map(Number);
    const totalMinutos = (h * 60 + m + 1) % (24 * 60);
    const hh = Math.floor(totalMinutos / 60);
    const mm = totalMinutos % 60;
    return `${String(hh).padStart(2, '0')}:${String(mm).padStart(2, '0')}`;
  }
}
