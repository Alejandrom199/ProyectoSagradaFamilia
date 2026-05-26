import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { formatearEdad, formatearFecha } from '../../../../shared/utils/date.utils';
import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PadresService } from '../../../../core/services/padres';
import { NinosService } from '../../../../core/services/ninos';
import { AuthService } from '../../../../core/services/auth';
import { PadreDetailResponse } from '../../../../shared/interfaces/padre.interface';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';
import { ConfirmModal } from "../../../../shared/components/confirm-modal/confirm-modal";


@Component({
  selector: 'detalle-padre',
  imports: [NgIcon, Datatable, Breadcrumb, ConfirmModal],

  templateUrl: './detalle-padre.html',
  styleUrl: './detalle-padre.css',
})
export class DetallePadre implements OnInit {
  @Input() id!: string;

  private padresService = inject(PadresService);
  private ninosService = inject(NinosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  auth = inject(AuthService);

  padre = signal<PadreDetailResponse | null>(null);
  hijos = signal<NinoResponse[]>([]);
  hijoAEliminar = signal<NinoResponse | null>(null);
  mostrarModalReset = signal(false);
  resetExito = signal(false);
  resetError = signal('');

  formatearFecha = formatearFecha;

  columnas: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre', label: 'Hijo/a', sortable: true, filterable: true,
      render: (row) => generarAvatarHtml(row.nombre, row.apellido, row.sexo),
      exportValue: (row) => `${row.nombre} ${row.apellido}`
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
      render: (row) => formatearFecha(row.fechaCreacion)
    }
  ];

  acciones: DatatableAction<NinoResponse>[] = [
    {
      type: 'ver',
      label: 'Ver ficha',
      onClick: (row) => this.router.navigate(['/pacientes', row.id])
    }
  ];
  // acciones: DatatableAction<NinoResponse>[] = [
  //   {
  //     type: 'editar',
  //     onClick: (row) => this.router.navigate(
  //       ['/padres', this.id, 'hijos', row.id, 'editar'],
  //       { queryParams: { origen: 'padres' } }
  //     )
  //   },
  //   {
  //     type: 'eliminar',
  //     onClick: (row) => this.hijoAEliminar.set(row)
  //   }
  // ];

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Detalle del Padre' },
  ];

  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.loadingBar.show();

    this.padresService.obtenerPorId(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) {
          this.padre.set(r.data);
          this.migajas = [
            { label: 'Padres', ruta: '/padres' },
            { label: `${r.data.nombre} ${r.data.apellido}` },
          ];
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });

    this.cargarHijos();
  }

  cargarHijos() {
    this.ninosService.obtenerPorPadre(parseInt(this.id)).subscribe({
      next: (r) => {
        if (r.success) this.hijos.set(r.data);
      }
    });
  }

  confirmarEliminar() {
    const hijo = this.hijoAEliminar();
    if (!hijo) return;

    this.loadingBar.show();
    this.ninosService.eliminar(hijo.id).subscribe({
      next: (r) => {
        if (r.success) {
          this.cargarHijos();
          this.cargarDatos();
        }
        this.hijoAEliminar.set(null);
        this.loadingBar.complete();
      },
      error: () => {
        this.hijoAEliminar.set(null);
        this.loadingBar.complete();
      }
    });
  }

  confirmarRestablecerPassword() {
    this.padresService.restablecerPassword(parseInt(this.id)).subscribe({
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
}
