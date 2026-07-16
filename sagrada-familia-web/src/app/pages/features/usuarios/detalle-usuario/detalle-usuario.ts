import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';
import { finalize } from 'rxjs';

import { formatearEdad, formatearFecha, renderFechaHora } from '../../../../shared/utils/date.utils';
import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { DesactivarCuentaModal } from '../../../../shared/components/desactivar-cuenta-modal/desactivar-cuenta-modal';
import { HistorialEstadoModal } from '../../../../shared/components/historial-estado-modal/historial-estado-modal';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { UsuariosService } from '../../../../core/services/usuarios';
import { MedicosService } from '../../../../core/services/medicos';
import { PadresService } from '../../../../core/services/padres';
import { NinosService } from '../../../../core/services/ninos';
import { AuthService } from '../../../../core/services/auth';
import { EstadoHistorialResponse, UsuarioDetailResponse } from '../../../../shared/interfaces/usuario.interface';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';
import { PadreDetailResponse } from '../../../../shared/interfaces/padre.interface';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';

@Component({
  selector: 'app-detalle-usuario',
  standalone: true,
  imports: [NgIcon, RouterLink, Breadcrumb, Datatable, ConfirmModal, DesactivarCuentaModal, HistorialEstadoModal],
  templateUrl: './detalle-usuario.html',
  styleUrl: './detalle-usuario.css',
})
export class DetalleUsuario implements OnInit {
  @Input() id!: string;

  private usuariosService = inject(UsuariosService);
  private medicosService  = inject(MedicosService);
  private padresService   = inject(PadresService);
  private ninosService    = inject(NinosService);
  private authService     = inject(AuthService);
  private router          = inject(Router);
  private loadingBar      = inject(LoadingBar);

  readonly esPropioUsuario = () => this.usuario()?.id === this.authService.currentUser()?.id;

  usuario   = signal<UsuarioDetailResponse | null>(null);
  medico    = signal<MedicoResponse | null>(null);
  padre     = signal<PadreDetailResponse | null>(null);
  pacientes = signal<NinoResponse[]>([]);
  hijos     = signal<NinoResponse[]>([]);

  procesando        = signal(false);
  mostrarModalReset = signal(false);
  resetExito        = signal(false);
  resetError        = signal('');

  mostrarModalDesactivar = signal(false);
  errorDesactivar         = signal<string | null>(null);

  mostrarModalHistorial = signal(false);
  cargandoHistorial     = signal(false);
  historialEstado       = signal<EstadoHistorialResponse[]>([]);

  formatearFecha = formatearFecha;

  get esMedico() { return !!this.usuario()?.medicoId; }
  get esPadre()  { return !!this.usuario()?.padreId;  }

  migajas: BreadcrumbItem[] = [
    { label: 'Usuarios', ruta: '/usuarios' },
    { label: 'Detalle' },
  ];

  columnasPacientes: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre', label: 'Paciente', sortable: true, filterable: true,
      render: (row) => `${row.nombre} ${row.apellido}`
    },
    {
      key: 'edadMeses', label: 'Edad', sortable: true,
      render: (row) => formatearEdad(row.edadMeses)
    },
    {
      key: 'nombrePadre', label: 'Representante', sortable: true, filterable: true
    },
    {
      key: 'fechaCreacion', label: 'Registro', sortable: true,
      render: (row) => renderFechaHora(row.fechaCreacion)
    }
  ];

  columnasHijos: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre', label: 'Hijo/a', sortable: true, filterable: true,
      render: (row) => `${row.nombre} ${row.apellido}`
    },
    {
      key: 'edadMeses', label: 'Edad', sortable: true,
      render: (row) => formatearEdad(row.edadMeses)
    },
    {
      key: 'fechaNacimiento', label: 'Nacimiento', sortable: true,
      render: (row) => formatearFecha(row.fechaNacimiento)
    },
    {
      key: 'fechaCreacion', label: 'Registro', sortable: true,
      render: (row) => renderFechaHora(row.fechaCreacion)
    }
  ];

  accionesPacientes: DatatableAction<NinoResponse>[] = [
    { type: 'ver', onClick: (row) => this.router.navigate(['/pacientes', row.id]) }
  ];

  accionesHijos: DatatableAction<NinoResponse>[] = [
    { type: 'ver', onClick: (row) => this.router.navigate(['/pacientes', row.id]) }
  ];

  ngOnInit(): void {
    this.cargarUsuario();
  }

  cargarUsuario(): void {
    this.loadingBar.show();
    this.usuariosService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.usuario.set(r.data);
          if (r.data.medicoId)  this.cargarDatosMedico(r.data.medicoId);
          else if (r.data.padreId) this.cargarDatosPadre(r.data.padreId);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  private cargarDatosMedico(medicoId: number): void {
    this.medicosService.obtenerTodos().subscribe(r => {
      if (r.success) {
        const m = r.data.find(x => x.id === medicoId) ?? null;
        this.medico.set(m);
        if (m) {
          this.migajas = [
            { label: 'Usuarios', ruta: '/usuarios' },
            { label: `${m.nombre} ${m.apellido}` }
          ];
        }
      }
    });

    this.ninosService.obtenerTodosAdmin().subscribe(r => {
      if (r.success) {
        this.pacientes.set(r.data.filter(n => n.medicoId === medicoId));
      }
    });
  }

  private cargarDatosPadre(padreId: number): void {
    this.padresService.obtenerPorId(padreId).subscribe(r => {
      if (r.success) {
        this.padre.set(r.data);
        this.migajas = [
          { label: 'Usuarios', ruta: '/usuarios' },
          { label: `${r.data.nombre} ${r.data.apellido}` }
        ];
      }
    });

    this.ninosService.obtenerPorPadre(padreId).subscribe(r => {
      if (r.success) this.hijos.set(r.data);
    });
  }

  confirmarRestablecerPassword(): void {
    const u = this.usuario();
    if (!u) return;

    this.loadingBar.show();
    const obs$ = u.medicoId
      ? this.medicosService.restablecerPassword(u.medicoId)
      : this.padresService.restablecerPassword(u.padreId!);

    obs$.pipe(finalize(() => this.loadingBar.complete())).subscribe({
      next: () => {
        this.mostrarModalReset.set(false);
        this.resetExito.set(true);
        this.resetError.set('');
      },
      error: (err) => {
        this.mostrarModalReset.set(false);
        this.resetError.set(err?.error?.message ?? 'No se pudo enviar el correo de restablecimiento.');
      }
    });
  }

  toggleEstado(): void {
    const u = this.usuario();
    if (!u) return;

    if (u.activo) {
      this.errorDesactivar.set(null);
      this.mostrarModalDesactivar.set(true);
      return;
    }

    this.procesando.set(true);
    this.loadingBar.show();
    this.usuariosService.actualizarEstado(u.id, true).subscribe({
      next: (r) => {
        if (r.success) this.usuario.set({ ...u, activo: true });
        this.procesando.set(false);
        this.loadingBar.complete();
      },
      error: () => {
        this.procesando.set(false);
        this.loadingBar.complete();
      }
    });
  }

  confirmarDesactivar(motivo: string): void {
    const u = this.usuario();
    if (!u) return;

    this.procesando.set(true);
    this.errorDesactivar.set(null);
    this.loadingBar.show();

    this.usuariosService.actualizarEstado(u.id, false, motivo).subscribe({
      next: (r) => {
        if (r.success) {
          this.usuario.set({ ...u, activo: false });
          this.mostrarModalDesactivar.set(false);
        } else {
          this.errorDesactivar.set(r.message);
        }
        this.procesando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.errorDesactivar.set(err.error?.message ?? 'No se pudo desactivar la cuenta.');
        this.procesando.set(false);
        this.loadingBar.complete();
      }
    });
  }

  abrirHistorial(): void {
    const u = this.usuario();
    if (!u) return;

    this.mostrarModalHistorial.set(true);
    this.cargandoHistorial.set(true);
    this.usuariosService.obtenerHistorialEstado(u.id).subscribe({
      next: (r) => {
        if (r.success) this.historialEstado.set(r.data);
        this.cargandoHistorial.set(false);
      },
      error: () => this.cargandoHistorial.set(false)
    });
  }

  nombreCompleto(): string {
    if (this.medico()) return `${this.medico()!.nombre} ${this.medico()!.apellido}`;
    if (this.padre())  return `${this.padre()!.nombre} ${this.padre()!.apellido}`;
    return this.usuario()?.email ?? '';
  }
}
