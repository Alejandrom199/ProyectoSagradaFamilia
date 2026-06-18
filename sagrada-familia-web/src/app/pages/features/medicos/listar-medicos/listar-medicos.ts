import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { DatatableAction, DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ImportModal } from '../../../../shared/components/import-modal/import-modal';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { MedicosService } from '../../../../core/services/medicos';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';

@Component({
  selector: 'app-listar-medicos',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, ConfirmModal, ImportModal, Button, Breadcrumb],

  templateUrl: './listar-medicos.html',
  styleUrl: './listar-medicos.css',
})
export class ListarMedicos implements OnInit {
  private medicosService = inject(MedicosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  medicos         = signal<MedicoResponse[]>([]);
  totalMedicos    = signal(0);
  medicoAEliminar = signal<MedicoResponse | null>(null);
  mostrarModalImport = signal(false);

  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {} };

  readonly importarFn = (file: File) => this.medicosService.importar(file);

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
    const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
    this.loadingBar.show();
    this.medicosService.obtenerPaginado(page, pageSize, search, sortBy, sortDir === 'asc').subscribe({
      next: (r) => {
        if (r.success) {
          this.medicos.set(r.data);
          this.totalMedicos.set(r.totalItems);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarMedicos();
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

  descargarExcel(): void {
    this.loadingBar.show();
    this.medicosService.exportarExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `medicos-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  descargarPlantilla(): void {
    this.loadingBar.show();
    this.medicosService.descargarPlantilla().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `plantilla-medicos-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }
}
