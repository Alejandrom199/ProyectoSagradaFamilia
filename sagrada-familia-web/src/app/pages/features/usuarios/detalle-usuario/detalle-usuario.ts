import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { UsuariosService } from '../../../../core/services/usuarios';
import { UsuarioDetailResponse } from '../../../../shared/interfaces/usuario.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'app-detalle-usuario',
  standalone: true,
  imports: [NgIcon, RouterLink, Breadcrumb],

  templateUrl: './detalle-usuario.html',
  styleUrl: './detalle-usuario.css',
})
export class DetalleUsuario implements OnInit {
  @Input() id!: string;

  private usuariosService = inject(UsuariosService);
  private loadingBar = inject(LoadingBar);

  usuario = signal<UsuarioDetailResponse | null>(null);
  procesando = signal(false);

  formatearFecha = formatearFecha;

  migajas: BreadcrumbItem[] = [
    { label: 'Usuarios', ruta: '/usuarios' },
    { label: 'Detalle' },
  ];

  ngOnInit(): void {
    this.cargarUsuario();
  }

  cargarUsuario(): void {
    this.loadingBar.show();
    this.usuariosService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) this.usuario.set(r.data);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  toggleEstado(): void {
    const u = this.usuario();
    if (!u) return;

    this.procesando.set(true);
    this.loadingBar.show();
    this.usuariosService.actualizarEstado(u.id, !u.activo).subscribe({
      next: (r) => {
        if (r.success) {
          this.usuario.set({ ...u, activo: !u.activo });
        }
        this.procesando.set(false);
        this.loadingBar.complete();
      },
      error: () => {
        this.procesando.set(false);
        this.loadingBar.complete();
      }
    });
  }
}
