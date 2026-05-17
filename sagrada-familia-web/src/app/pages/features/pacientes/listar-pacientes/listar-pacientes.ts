import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import {
  heroPlus, heroPencil, heroTrash, heroEye, heroChartBar, heroUserCircle
} from '@ng-icons/heroicons/outline';

// 💡 Interfaces estrictamente tipadas
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { ApiResponse } from '../../../../shared/interfaces/api.interface';

import { formatearEdad, formatearFecha } from '../../../../shared/utils/date.utils';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';


import { Datatable, DatatableAction, DatatableColumn } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from "../../../../shared/components/breadcrumb/breadcrumb";
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';
import { NinosService } from '../../../../core/services/ninos';

@Component({
  selector: 'listar-pacientes',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NgIcon,
    RouterLink,
    Datatable,
    ConfirmModal,
    Button,
    Breadcrumb
  ],
  viewProviders: [provideIcons({
    heroPlus, heroPencil, heroTrash, heroEye, heroChartBar, heroUserCircle
  })],
  templateUrl: './listar-pacientes.html',
  styleUrl: './listar-pacientes.css',
})
export class ListarPacientes implements OnInit {
  private ninosService = inject(NinosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  ninos = signal<NinoResponse[]>([]);
  ninoAEliminar = signal<NinoResponse | null>(null);

  columnas: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre',
      label: 'Paciente',
      sortable: true,
      filterable: true,
      render: (row: NinoResponse) => generarAvatarHtml(row.nombreCompleto, row.sexo),
      exportValue: (row: NinoResponse) => `${row.nombreCompleto}`
    },
    {
      key: 'edadMeses',
      label: 'Edad Actual',
      sortable: true,
      render: (row: NinoResponse) => `<span class="font-medium text-gray-700">${formatearEdad(row.edadMeses)}</span>`,
      exportValue: (row: NinoResponse) => formatearEdad(row.edadMeses)
    },
    {
      key: 'nombrePadre',
      label: 'Representante',
      sortable: true,
      filterable: true,
      exportValue: (row: NinoResponse) => row.nombrePadre
    },
    {
      key: 'fechaNacimiento',
      label: 'Nacimiento',
      sortable: true,
      render: (row: NinoResponse) => formatearFecha(row.fechaNacimiento),
      exportValue: (row: NinoResponse) => formatearFecha(row.fechaNacimiento)
    }
  ];

  acciones: DatatableAction<NinoResponse>[] = [
    { type: 'ver', onClick: (row: NinoResponse) => this.router.navigate(['/pacientes', row.id]) },
    { type: 'medidas', onClick: (row: NinoResponse) => this.router.navigate(['/medidas', row.id]) },
    { type: 'editar', onClick: (row: NinoResponse) => this.router.navigate(['/pacientes', row.id, 'editar']) },
    { type: 'eliminar', onClick: (row: NinoResponse) => this.ninoAEliminar.set(row) }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
  ];

  ngOnInit(): void {
    this.cargarNinos();
  }

  cargarNinos(): void {
    this.loadingBar.show();
    this.ninosService.obtenerTodos().subscribe({
      next: (res: ApiResponse<NinoResponse[]>) => {
        if (res.success) {
          this.ninos.set(res.data);
        }
      },
      complete: () => this.loadingBar.complete(),
      error: () => this.loadingBar.complete()
    });
  }

  confirmarEliminar(): void {
    const nino = this.ninoAEliminar();
    if (!nino) return;

    this.loadingBar.show();
    this.ninosService.eliminar(nino.id).subscribe({
      next: (res: ApiResponse<null>) => {
        if (res.success) {
          this.cargarNinos();
        }
        this.ninoAEliminar.set(null);
      },
      complete: () => this.loadingBar.complete(),
      error: () => {
        this.ninoAEliminar.set(null);
        this.loadingBar.complete();
      }
    });
  }
}