import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { finalize } from 'rxjs';

import { CitasService } from '../../../../core/services/citas';
import { ParametrosService } from '../../../../core/services/parametros';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { CitaResponse, CitaUpdate } from '../../../../shared/interfaces/cita.interface';
import { formatearFecha, formatearHora } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'editar-cita',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, Breadcrumb, NgIcon],
  templateUrl: './editar-cita.html',
})
export class EditarCita implements OnInit {
  @Input() citaId!: string;

  private fb               = inject(FormBuilder);
  private citasService     = inject(CitasService);
  private parametrosService = inject(ParametrosService);
  private router           = inject(Router);
  private loadingBar       = inject(LoadingBar);

  cita             = signal<CitaResponse | null>(null);
  formCita!:       FormGroup;
  guardando        = signal(false);
  error            = signal<string | null>(null);
  horaInicioAtencion = signal<string | null>(null);
  horaFinAtencion    = signal<string | null>(null);

  formatearFecha = formatearFecha;
  formatearHora  = formatearHora;

  readonly fechaMinima = (() => {
    const hoy = new Date();
    return `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}-${String(hoy.getDate()).padStart(2, '0')}`;
  })();

  migajas: BreadcrumbItem[] = [
    { label: 'Citas', ruta: '/citas' },
    { label: 'Reagendar cita' },
  ];

  ngOnInit(): void {
    this.formCita = this.initForm();
    this.cargarCita();
    this.cargarHorarioAtencion();
  }

  private initForm(): FormGroup {
    return this.fb.group({
      fecha:   ['', Validators.required],
      hora:    ['', Validators.required],
      horaFin: ['', Validators.required],
      motivo:  ['']
    }, { validators: this.fechaHoraFuturaValidator });
  }

  get f() { return this.formCita.controls; }

  private cargarCita(): void {
    this.loadingBar.show();
    this.citasService.obtenerPorId(parseInt(this.citaId)).subscribe({
      next: (r) => {
        if (r.success) {
          this.cita.set(r.data);

          const estadosPermitidos = ['Pendiente', 'Cancelada', 'NoAsistio'];
          if (!estadosPermitidos.includes(r.data.estado)) {
            this.router.navigate(['/citas', this.citaId]);
            return;
          }

          this.migajas = [
            { label: 'Citas', ruta: '/citas' },
            { label: r.data.nombreNino, ruta: `/citas/${this.citaId}` },
            { label: 'Reagendar cita' },
          ];

          const f   = new Date(r.data.fechaHora);
          const fin = r.data.fechaHoraFin ? new Date(r.data.fechaHoraFin) : null;

          this.formCita.patchValue({
            fecha:   `${f.getFullYear()}-${String(f.getMonth() + 1).padStart(2, '0')}-${String(f.getDate()).padStart(2, '0')}`,
            hora:    `${String(f.getHours()).padStart(2, '0')}:${String(f.getMinutes()).padStart(2, '0')}`,
            horaFin: fin ? `${String(fin.getHours()).padStart(2, '0')}:${String(fin.getMinutes()).padStart(2, '0')}` : '',
            motivo:  r.data.motivo ?? ''
          });
        }
        this.loadingBar.complete();
      },
      error: () => {
        this.loadingBar.complete();
        this.router.navigate(['/citas']);
      }
    });
  }

  private cargarHorarioAtencion(): void {
    this.parametrosService.obtenerPorGrupo('HORARIO_ATENCION').subscribe({
      next: (r) => {
        if (r.success) {
          const inicio = r.data.find(p => p.codigo === 'HORA_INICIO' && p.activo);
          const fin    = r.data.find(p => p.codigo === 'HORA_FIN'    && p.activo);
          if (inicio) this.horaInicioAtencion.set(inicio.valor);
          if (fin)    this.horaFinAtencion.set(fin.valor);
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
    const inicio = this.horaInicioAtencion();
    const fin    = this.horaFinAtencion();

    if (v.horaFin <= v.hora) {
      this.error.set('La hora de terminación debe ser posterior a la hora de inicio.');
      return;
    }
    if (inicio && v.hora < inicio) {
      this.error.set(`Las citas solo pueden agendarse a partir de las ${inicio} horas.`);
      return;
    }
    if (fin && v.horaFin > fin) {
      this.error.set(`Las citas deben terminar antes de las ${fin} horas.`);
      return;
    }

    const cita = this.cita();
    if (!cita) return;

    this.guardando.set(true);
    this.error.set(null);
    this.loadingBar.show();

    const request: CitaUpdate = {
      medicoId:     cita.medicoId,
      fechaHora:    `${v.fecha}T${v.hora}:00`,
      fechaHoraFin: `${v.fecha}T${v.horaFin}:00`,
      motivo:       v.motivo || undefined
    };

    this.citasService.actualizar(parseInt(this.citaId), request)
      .pipe(finalize(() => {
        this.guardando.set(false);
        this.loadingBar.complete();
      }))
      .subscribe({
        next: (res) => {
          if (res.success) this.router.navigate(['/citas', res.data.id]);
          else this.error.set(res.message);
        },
        error: (err) => this.error.set(err?.error?.message ?? 'Error al reagendar la cita. Intente más tarde.')
      });
  }

  private fechaHoraFuturaValidator(group: AbstractControl): ValidationErrors | null {
    const fecha = group.get('fecha')?.value;
    const hora  = group.get('hora')?.value;
    if (!fecha || !hora) return null;

    const hoy    = new Date();
    const hoyStr = `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}-${String(hoy.getDate()).padStart(2, '0')}`;

    if (fecha > hoyStr) return null;
    if (fecha < hoyStr) return { fechaPasada: true };

    const seleccionada = new Date(`${fecha}T${hora}:00`);
    return seleccionada <= hoy ? { fechaPasada: true } : null;
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
