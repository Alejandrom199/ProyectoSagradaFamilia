import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { DatatableAction, DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { ImportModal } from '../../../../shared/components/import-modal/import-modal';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { DesactivarCuentaModal } from '../../../../shared/components/desactivar-cuenta-modal/desactivar-cuenta-modal';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { UsuariosService } from '../../../../core/services/usuarios';
import { MedicosService } from '../../../../core/services/medicos';
import { PadresService } from '../../../../core/services/padres';
import { AuthService } from '../../../../core/services/auth';
import { Reportes } from '../../../../core/services/reportes';
import { UsuarioResponse } from '../../../../shared/interfaces/usuario.interface';
import { formatearFecha, renderFechaHora } from '../../../../shared/utils/date.utils';
import { MenuService } from '../../../../core/services/menu';
import { Accion } from '../../../../shared/enums/accion.enum';
import { ROL_BADGE_COLOR } from '../../../../shared/constants/rol-badge.constants';
import { badgeHtml } from '../../../../shared/utils/badge.util';
import { RutaApp } from '../../../../shared/enums/ruta-app.enum';

@Component({
  selector: 'app-listar-usuarios',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, ImportModal, ConfirmModal, DesactivarCuentaModal, Button, Breadcrumb],
  templateUrl: './listar-usuarios.html',
  styleUrl: './listar-usuarios.css',
})
export class ListarUsuarios implements OnInit {
  private readonly usuariosService = inject(UsuariosService);
  private readonly medicosService = inject(MedicosService);
  private readonly padresService = inject(PadresService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly loadingBar = inject(LoadingBar);
  private readonly reportesService = inject(Reportes);

  readonly menu = inject(MenuService);
  protected readonly Accion = Accion;
  protected readonly RutaApp = RutaApp;

  usuarios = signal<UsuarioResponse[]>([]);
  totalUsuarios = signal(0);
  mostrarModalImport = signal(false);
  usuarioAEliminar = signal<UsuarioResponse | null>(null);
  errorEliminar = signal<string | null>(null);
  eliminando = signal(false);

  usuarioADesactivar = signal<UsuarioResponse | null>(null);
  errorDesactivar = signal<string | null>(null);
  desactivando = signal(false);

  readonly filterOptions: Record<string, string[]> = {
    rolNombre: ['Administrador', 'Medico', 'Padre'],
  };

  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {} };

  readonly importarFn = (file: File) => this.usuariosService.importar(file);

  columnas: DatatableColumn<UsuarioResponse>[] = [
    {
      key: 'email', label: 'Email', sortable: true, filterable: true,
      render: (row) => `
        <div class="flex items-center gap-3">
          <div class="w-10 h-10 rounded-lg bg-blue-100 dark:bg-blue-500/15 text-blue-600 dark:text-blue-400 flex items-center justify-center text-sm font-bold">
            ${row.email.charAt(0).toUpperCase()}
          </div>
          <p class="font-medium text-[var(--color-text-primary)]">${row.email}</p>
        </div>`
    },
    {
      key: 'rolNombre', label: 'Rol', sortable: true, filterable: true,
      render: (row) => badgeHtml(row.rolNombre, ROL_BADGE_COLOR[row.rolNombre] ?? 'muted')
    },
    {
      key: 'esMedico', label: 'Perfil',
      render: (row) => {
        if (row.esMedico) return '<span class="text-xs text-blue-700 dark:text-blue-400 font-semibold">Médico vinculado</span>';
        if (row.esPadre) return '<span class="text-xs text-pink-700 dark:text-pink-400 font-semibold">Padre vinculado</span>';
        return '<span class="text-xs text-muted">Administrativo</span>';
      }
    },
    {
      key: 'activo', label: 'Estado', sortable: true,
      render: (row) => row.activo
        ? `<span class="px-2 py-1 rounded-full text-xs font-bold bg-success-soft text-success">Activo</span>`
        : `<span class="px-2 py-1 rounded-full text-xs font-bold bg-danger-soft text-danger">Inactivo</span>`
    },
    {
      key: 'fechaCreacion', label: 'Registro', sortable: true,
      render: (row) => renderFechaHora(row.fechaCreacion)
    }
  ];

  // Las acciones de activar/desactivar son operacionales (basadas en estado de fila), no CRUD.
  // El 'ver' siempre está disponible para quien tiene acceso a /usuarios.
  // Editar/Eliminar solo aplican a usuarios vinculados a un perfil de Médico o Padre
  // (las cuentas administrativas puras no tienen una pantalla de edición propia).
  readonly acciones = computed<DatatableAction<UsuarioResponse>[]>(() => [
    { type: 'ver', onClick: (row) => this.router.navigate(['/usuarios', row.id]) },
    {
      type: 'editar',
      visible: (row) => row.esMedico || row.esPadre,
      onClick: (row) => {
        if (row.esMedico) this.router.navigate(['/medicos', row.medicoId, 'editar']);
        else if (row.esPadre) this.router.navigate(['/padres', row.padreId, 'editar']);
      }
    },
    {
      type: 'eliminar',
      visible: (row) => row.esMedico || row.esPadre,
      onClick: (row) => { this.errorEliminar.set(null); this.usuarioAEliminar.set(row); }
    },
    {
      label: 'Desactivar',
      icon: 'matLockOutline',
      class: 'text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-500/10',
      visible: (row) => row.activo && row.id !== this.authService.currentUser()?.id,
      onClick: (row) => { this.errorDesactivar.set(null); this.usuarioADesactivar.set(row); }
    },
    {
      label: 'Activar',
      icon: 'matLockOpenOutline',
      class: 'text-green-600 dark:text-green-400 hover:bg-green-50 dark:hover:bg-green-500/10',
      visible: (row) => !row.activo,
      onClick: (row) => this.toggleEstado(row, true)
    }
  ]);

  migajas: BreadcrumbItem[] = [{ label: 'Usuarios' }];

  ngOnInit(): void {
    this.cargarUsuarios();
  }

  cargarUsuarios(): void {
    const q = this.queryActual;
    this.loadingBar.show();
    this.usuariosService.obtenerPaginado(q.page, q.pageSize, q.search, q.sortBy, q.sortDir === 'asc').subscribe({
      next: (res) => {
        this.usuarios.set(res.data);
        this.totalUsuarios.set(res.totalItems);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarUsuarios();
  }

  confirmarEliminar(): void {
    const usuario = this.usuarioAEliminar();
    if (!usuario) return;

    const eliminar$ = usuario.esMedico
      ? this.medicosService.eliminar(usuario.medicoId!)
      : this.padresService.eliminar(usuario.padreId!);

    this.eliminando.set(true);
    this.errorEliminar.set(null);

    eliminar$.subscribe({
      next: (r) => {
        if (r.success) {
          this.usuarios.update(lista => lista.filter(u => u.id !== usuario.id));
          this.usuarioAEliminar.set(null);
        }
        this.eliminando.set(false);
      },
      error: (err) => {
        this.errorEliminar.set(err.error?.message ?? 'No se pudo eliminar el registro.');
        this.eliminando.set(false);
      }
    });
  }

  toggleEstado(usuario: UsuarioResponse, activo: boolean): void {
    this.loadingBar.show();
    this.usuariosService.actualizarEstado(usuario.id, activo).subscribe({
      next: (r) => {
        if (r.success) this.usuarios.update(lista => lista.map(u => u.id === usuario.id ? { ...u, activo } : u));
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  confirmarDesactivar(motivo: string): void {
    const usuario = this.usuarioADesactivar();
    if (!usuario) return;

    this.desactivando.set(true);
    this.errorDesactivar.set(null);
    this.loadingBar.show();

    this.usuariosService.actualizarEstado(usuario.id, false, motivo).subscribe({
      next: (r) => {
        if (r.success) {
          this.usuarios.update(lista => lista.map(u => u.id === usuario.id ? { ...u, activo: false } : u));
          this.usuarioADesactivar.set(null);
        } else {
          this.errorDesactivar.set(r.message);
        }
        this.desactivando.set(false);
        this.loadingBar.complete();
      },
      error: (err) => {
        this.errorDesactivar.set(err.error?.message ?? 'No se pudo desactivar la cuenta.');
        this.desactivando.set(false);
        this.loadingBar.complete();
      }
    });
  }

  exportarPdf(): void {
    this.loadingBar.show();
    const u = this.authService.currentUser();
    const params = { titulo: 'Listado de Usuarios', usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/usuarios-pdf', params).subscribe({
      next: (blob) => { this.descargarBlob(blob, `usuarios-${hoy()}.pdf`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel(): void {
    this.loadingBar.show();
    this.usuariosService.exportarExcel().subscribe({
      next: (blob) => { this.descargarBlob(blob, `usuarios-${hoy()}.xlsx`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarPlantilla(): void {
    this.loadingBar.show();
    this.usuariosService.descargarPlantilla().subscribe({
      next: (blob) => { this.descargarBlob(blob, `plantilla-usuarios-${hoy()}.xlsx`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  private descargarBlob(blob: Blob, nombre: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = nombre;
    link.click();
    window.URL.revokeObjectURL(url);
  }

}

function hoy(): string { return new Date().toISOString().split('T')[0]; }
