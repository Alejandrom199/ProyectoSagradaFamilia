import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroPlus, heroPencil, heroTrash, heroAcademicCap } from '@ng-icons/heroicons/outline';

import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { MedicosService } from '../../../../core/services/medicos';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';

@Component({
  selector: 'app-listar-medicos',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, ConfirmModal, Button, Breadcrumb],
  viewProviders: [provideIcons({ heroPlus, heroPencil, heroTrash, heroAcademicCap })],
  templateUrl: './listar-medicos.html',
  styleUrl: './listar-medicos.css',
})
export class ListarMedicos implements OnInit {
  private medicosService = inject(MedicosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  medicos = signal<MedicoResponse[]>([]);
  medicoAEliminar = signal<MedicoResponse | null>(null);

  columnas: DatatableColumn<MedicoResponse>[] = [
    {
      key: 'nombre',
      label: 'Médico',
      sortable: true,
      filterable: true,
      render: (row) => generarAvatarHtml(row.nombre, row.apellido),
      exportValue: (row) => `${row.nombre} ${row.apellido}`
    },
    {
      key: 'email',
      label: 'Email',
      sortable: true,
      filterable: true,
      render: (row) => `<span class="text-gray-700">${row.email}</span>`
    },
    {
      key: 'especialidad',
      label: 'Especialidad',
      sortable: true,
      filterable: true,
      render: (row) => row.especialidad
        ? `<span class="badge badge-primary">${row.especialidad}</span>`
        : '<span class="text-gray-400">Sin especialidad</span>'
    },
    {
      key: 'telefono',
      label: 'Teléfono',
      sortable: true,
      render: (row) => row.telefono || '<span class="text-gray-400">Sin registrar</span>'
    }
  ];

  acciones: DatatableAction<MedicoResponse>[] = [
    {
      type: 'ver',
      label: 'Ver detalle',
      onClick: (row) => this.router.navigate(['/medicos', row.id])
    },
    {
      type: 'editar',
      onClick: (row) => this.router.navigate(['/medicos', row.id, 'editar'])
    },
    {
      type: 'eliminar',
      onClick: (row) => this.medicoAEliminar.set(row)
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Médicos' },
  ];

  ngOnInit(): void {
    this.cargarMedicos();
  }

  cargarMedicos(): void {
    this.loadingBar.show();
    this.medicosService.obtenerTodos().subscribe({
      next: (response) => {
        if (response.success) {
          this.medicos.set(response.data);
        } else {
          console.error(response.message, response.errors);
        }
        this.loadingBar.complete();
      },
      error: (err) => {
        console.error('Error al cargar médicos', err);
        this.loadingBar.complete();
      }
    });
  }

  confirmarEliminar(): void {
    const medico = this.medicoAEliminar();
    if (!medico) return;

    this.loadingBar.show();
    this.medicosService.eliminar(medico.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.medicos.update(actuales => actuales.filter(m => m.id !== medico.id));
        } else {
          console.error(response.message);
        }
        this.medicoAEliminar.set(null);
        this.loadingBar.complete();
      },
      error: (err) => {
        console.error('Error al eliminar médico', err);
        this.medicoAEliminar.set(null);
        this.loadingBar.complete();
      }
    });
  }
}
