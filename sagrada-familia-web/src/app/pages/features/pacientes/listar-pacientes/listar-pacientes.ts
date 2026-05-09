import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroPlus, heroPencil, heroTrash, heroEye, heroChartBar, heroUserCircle } from '@ng-icons/heroicons/outline';
import { NinoResponse } from '../../../../shared/interfaces/responses/nino.response';
import { formatearEdad, formatearFecha } from '../../../../shared/utils/date.utils';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Ninos } from '../../../../core/services/ninos';
import { Datatable, DatatableAction, DatatableColumn } from '../../../../shared/components/datatable/datatable';

import { Button } from '../../../../shared/components/button/button';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Breadcrumb, BreadcrumbItem } from "../../../../shared/components/breadcrumb/breadcrumb";
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';

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
  viewProviders: [provideIcons({ heroPlus, heroPencil, heroTrash, heroEye, heroChartBar, heroUserCircle })],
  templateUrl: './listar-pacientes.html',
  styleUrl: './listar-pacientes.css',
})
export class ListarPacientes implements OnInit {
  private ninosService = inject(Ninos);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  ninos = signal<NinoResponse[]>([]);
  ninoAEliminar = signal<NinoResponse | null>(null);

  columnas: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre', label: 'Paciente', sortable: true, filterable: true,
      render: (row) => generarAvatarHtml(row.nombre, row.apellido, row.sexo)
    },
    {
      key: 'edadMeses', label: 'Edad Actual', sortable: true,
      render: (row) => `<span class="font-medium text-gray-700">${formatearEdad(row.edadMeses)}</span>`
    },
    {
      key: 'nombrePadre', label: 'Representante', sortable: true, filterable: true
    },
    {
      key: 'fechaNacimiento', label: 'Nacimiento', sortable: true,
      render: (row) => formatearFecha(row.fechaNacimiento)
    }
  ];

  acciones: DatatableAction<NinoResponse>[] = [
    { type: 'ver', onClick: (row) => this.router.navigate(['/pacientes', row.id]) },
    { type: 'medidas', onClick: (row) => this.router.navigate(['/medidas', row.id]) },
    { type: 'editar', onClick: (row) => this.router.navigate(['/pacientes', row.id, 'editar']) },
    { type: 'eliminar', onClick: (row) => this.ninoAEliminar.set(row) }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
  ];

  ngOnInit() {
    this.cargarNinos();
  }

  cargarNinos() {
    this.loadingBar.show();
    this.ninosService.obtenerTodos().subscribe(r => {
      if (r.success) this.ninos.set(r.data);
      this.loadingBar.complete();
    });
  }

  confirmarEliminar() {
    const nino = this.ninoAEliminar();
    if (!nino) return;

    this.loadingBar.show();
    this.ninosService.eliminar(nino.id).subscribe(r => {
      if (r.success) this.cargarNinos();
      this.ninoAEliminar.set(null);
      this.loadingBar.complete();
    });
  }
}