import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { SearchableSelect } from '../../../../shared/components/searchable-select/searchable-select';
import { SEXO_OPTIONS } from '../../../../shared/constants/sexo.constants';
import { NinosService } from '../../../../core/services/ninos';
import { MedicosService } from '../../../../core/services/medicos';
import { PadresService } from '../../../../core/services/padres';
import { AuthService } from '../../../../core/services/auth';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';
import { PadreResponse } from '../../../../shared/interfaces/padre.interface';

@Component({
  selector: 'app-editar-paciente',
  imports: [RouterLink, FormsModule, Breadcrumb, NgIcon, SearchableSelect],

  templateUrl: './editar-paciente.html',
})
export class EditarPaciente implements OnInit {
  @Input() id!: string;

  private ninosService = inject(NinosService);
  private medicosService = inject(MedicosService);
  private padresService = inject(PadresService);
  readonly authService = inject(AuthService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  nino = signal<NinoDetailResponse | null>(null);
  medicos = signal<MedicoResponse[]>([]);
  padres = signal<PadreResponse[]>([]);
  guardando = signal(false);
  error = signal('');

  form = {
    nombre: '',
    apellido: '',
    fechaNacimiento: '',
    sexo: '' as 'M' | 'F' | ''
  };

  // ── Reasignar médico (Admin) ────────────────────────────────────
  mostrarCambioMedico = signal(false);
  nuevoMedicoId = signal<number | null>(null);
  guardandoMedico = signal(false);
  errorMedico = signal<string | null>(null);
  exitoMedico = signal(false);

  // ── Reasignar representante (Médico) ─────────────────────────────
  mostrarCambioPadre = signal(false);
  nuevoPadreId = signal<number | null>(null);
  guardandoPadre = signal(false);
  errorPadre = signal<string | null>(null);
  exitoPadre = signal(false);

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Editar Paciente' },
  ];

  readonly medicoLabelFn = (m: MedicoResponse) => `Dr(a). ${m.nombre} ${m.apellido}`;
  readonly padreLabelFn = (p: PadreResponse) => `${p.nombre} ${p.apellido} — ${p.email}`;
  readonly sexoOptions = SEXO_OPTIONS;

  ngOnInit(): void {
    if (this.authService.esAdmin()) {
      this.medicosService.obtenerTodos().subscribe({
        next: (r) => { if (r.success) this.medicos.set(r.data); }
      });
    }

    if (this.authService.esMedico()) {
      this.padresService.obtenerTodos().subscribe({
        next: (r) => { if (r.success) this.padres.set(r.data); }
      });
    }

    this.loadingBar.show();
    this.ninosService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.nino.set(r.data);
          this.migajas = [
            { label: 'Pacientes', ruta: '/pacientes' },
            { label: `${r.data.nombre} ${r.data.apellido}`, ruta: `/pacientes/${this.id}` },
            { label: 'Editar' },
          ];
          this.form = {
            nombre: r.data.nombre,
            apellido: r.data.apellido,
            fechaNacimiento: r.data.fechaNacimiento,
            sexo: r.data.sexo
          };
          this.nuevoMedicoId.set(r.data.medicoId);
          this.nuevoPadreId.set(r.data.padreId);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  guardar(): void {
    if (!this.form.nombre || !this.form.apellido || !this.form.fechaNacimiento || !this.form.sexo) {
      this.error.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.loadingBar.show();

    this.ninosService.actualizar(parseInt(this.id), {
      nombre: this.form.nombre,
      apellido: this.form.apellido,
      fechaNacimiento: this.form.fechaNacimiento,
      sexo: this.form.sexo as 'M' | 'F'
    }).subscribe({
      next: (r) => {
        if (r.success) this.router.navigate(['/pacientes', this.id]);
        this.guardando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Error al actualizar.');
        this.guardando.set(false);
        this.loadingBar.complete();
      }
    });
  }

  cambiarMedico(): void {
    const medicoId = this.nuevoMedicoId();
    if (!medicoId) return;

    this.guardandoMedico.set(true);
    this.errorMedico.set(null);

    this.ninosService.cambiarMedico(parseInt(this.id), { medicoId }).subscribe({
      next: (res) => {
        if (res.success) {
          const medico = this.medicos().find(m => m.id === medicoId);
          const n = this.nino();
          if (n && medico) this.nino.set({ ...n, medicoId, medicoNombreCompleto: `${medico.nombre} ${medico.apellido}` });
          this.mostrarCambioMedico.set(false);
          this.exitoMedico.set(true);
          setTimeout(() => this.exitoMedico.set(false), 4000);
        } else {
          this.errorMedico.set(res.message);
        }
      },
      error: (err) => {
        this.errorMedico.set(err.error?.message ?? 'Error al reasignar el médico.');
      },
      complete: () => this.guardandoMedico.set(false)
    });
  }

  toggleCambioMedico(): void {
    this.mostrarCambioMedico.update(v => !v);
    this.errorMedico.set(null);
    this.nuevoMedicoId.set(this.nino()?.medicoId ?? null);
  }

  cambiarPadre(): void {
    const padreId = this.nuevoPadreId();
    if (!padreId) return;

    this.guardandoPadre.set(true);
    this.errorPadre.set(null);

    this.ninosService.cambiarPadre(parseInt(this.id), { padreId }).subscribe({
      next: (res) => {
        if (res.success) {
          const padre = this.padres().find(p => p.id === padreId);
          const n = this.nino();
          if (n && padre) this.nino.set({ ...n, padreId, padreNombreCompleto: `${padre.nombre} ${padre.apellido}`, padreEmail: padre.email });
          this.mostrarCambioPadre.set(false);
          this.exitoPadre.set(true);
          setTimeout(() => this.exitoPadre.set(false), 4000);
        } else {
          this.errorPadre.set(res.message);
        }
      },
      error: (err) => {
        this.errorPadre.set(err.error?.message ?? 'Error al reasignar el representante.');
      },
      complete: () => this.guardandoPadre.set(false)
    });
  }

  toggleCambioPadre(): void {
    this.mostrarCambioPadre.update(v => !v);
    this.errorPadre.set(null);
    this.nuevoPadreId.set(this.nino()?.padreId ?? null);
  }
}
