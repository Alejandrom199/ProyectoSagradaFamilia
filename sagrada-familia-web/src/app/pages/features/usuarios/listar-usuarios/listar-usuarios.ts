import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { DatatableAction, DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { UsuariosService } from '../../../../core/services/usuarios';
import { AuthService } from '../../../../core/services/auth';
import { UsuarioResponse } from '../../../../shared/interfaces/usuario.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'app-listar-usuarios',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, Button, Breadcrumb],

  templateUrl: './listar-usuarios.html',
  styleUrl: './listar-usuarios.css',
})
export class ListarUsuarios implements OnInit {
  private usuariosService = inject(UsuariosService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  usuarios = signal<UsuarioResponse[]>([]);
  totalUsuarios = signal(0);
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {} };

  columnas: DatatableColumn<UsuarioResponse>[] = [
    {
      key: 'email', label: 'Email', sortable: true, filterable: true,
      render: (row) => `
        <div class="flex items-center gap-3">
          <div class="w-9 h-9 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center text-sm font-bold">
            ${row.email.charAt(0).toUpperCase()}
          </div>
          <p class="font-medium text-gray-800">${row.email}</p>
        </div>`
    },
    {
      key: 'rolNombre', label: 'Rol', sortable: true, filterable: true,
      render: (row) => `<span class="px-2 py-1 rounded-full text-xs font-bold ${this.colorRol(row.rolNombre)}">${row.rolNombre}</span>`
    },
    {
      key: 'esMedico', label: 'Perfil',
      render: (row) => {
        if (row.esMedico) return '<span class="text-xs text-blue-700 font-semibold">Médico vinculado</span>';
        if (row.esPadre) return '<span class="text-xs text-pink-700 font-semibold">Padre vinculado</span>';
        return '<span class="text-xs text-gray-400">Administrativo</span>';
      }
    },
    {
      key: 'activo', label: 'Estado', sortable: true,
      render: (row) => row.activo
        ? `<span class="px-2 py-1 rounded-full text-xs font-bold bg-green-100 text-green-700">Activo</span>`
        : `<span class="px-2 py-1 rounded-full text-xs font-bold bg-red-100 text-red-700">Inactivo</span>`
    },
    {
      key: 'fechaCreacion', label: 'Alta', sortable: true,
      render: (row) => formatearFecha(row.fechaCreacion)
    }
  ];

  acciones: DatatableAction<UsuarioResponse>[] = [
    {
      type: 'ver',
      onClick: (row) => this.router.navigate(['/usuarios', row.id])
    },
    {
      label: 'Desactivar',
      icon: 'matLockOutline',
      class: 'text-red-600 hover:bg-red-50',
      visible: (row) => row.activo && row.id !== this.authService.currentUser()?.id,
      onClick: (row) => this.toggleEstado(row, false)
    },
    {
      label: 'Activar',
      icon: 'matLockOpenOutline',
      class: 'text-green-600 hover:bg-green-50',
      visible: (row) => !row.activo,
      onClick: (row) => this.toggleEstado(row, true)
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Usuarios' },
  ];

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

  toggleEstado(usuario: UsuarioResponse, activo: boolean): void {
    this.loadingBar.show();
    this.usuariosService.actualizarEstado(usuario.id, activo).subscribe({
      next: (response) => {
        if (response.success) {
          this.usuarios.update(actuales =>
            actuales.map(u => u.id === usuario.id ? { ...u, activo } : u)
          );
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  private colorRol(rol: string): string {
    const mapa: Record<string, string> = {
      'Administrador': 'bg-purple-100 text-purple-700',
      'Medico': 'bg-blue-100 text-blue-700',
      'Padre': 'bg-pink-100 text-pink-700'
    };
    return mapa[rol] || 'bg-gray-100 text-gray-700';
  }
}
