import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { DatatableAction, DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ImportModal } from '../../../../shared/components/import-modal/import-modal';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { MedicosService } from '../../../../core/services/medicos';
import { AuthService } from '../../../../core/services/auth';
import { Reportes } from '../../../../core/services/reportes';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';
import { generarAvatarHtml } from '../../../../shared/utils/avatar.util';
import { MenuService } from '../../../../core/services/menu';
import { Accion } from '../../../../shared/enums/accion.enum';
import { RutaApp } from '../../../../shared/enums/ruta-app.enum';

@Component({
  selector: 'app-listar-medicos',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, ConfirmModal, ImportModal, Button, Breadcrumb],
  templateUrl: './listar-medicos.html',
  styleUrl: './listar-medicos.css',
})
export class ListarMedicos implements OnInit {
  private readonly medicosService  = inject(MedicosService);
  private readonly router          = inject(Router);
  private readonly loadingBar      = inject(LoadingBar);
  private readonly authService     = inject(AuthService);
  private readonly reportesService = inject(Reportes);

  readonly menu   = inject(MenuService);
  protected readonly Accion  = Accion;
  protected readonly RutaApp = RutaApp;

  medicos            = signal<MedicoResponse[]>([]);
  totalMedicos       = signal(0);
  medicoAEliminar    = signal<MedicoResponse | null>(null);
  errorEliminar      = signal<string | null>(null);
  mostrarModalImport = signal(false);

  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'asc', columnFilters: {} };

  readonly importarFn = (file: File) => this.medicosService.importar(file);

  columnas: DatatableColumn<MedicoResponse>[] = [
    {
      key: 'nombre', label: 'Médico', sortable: true, filterable: true,
      render: (row) => generarAvatarHtml(row.nombre, row.apellido),
      exportValue: (row) => `${row.nombre} ${row.apellido}`
    },
    {
      key: 'email', label: 'Email', sortable: true, filterable: true,
      render: (row) => `<span class="text-[var(--color-text-secondary)]">${row.email}</span>`
    },
    {
      key: 'especialidad', label: 'Especialidad', sortable: true, filterable: true,
      render: (row) => row.especialidad
        ? `<span class="badge badge-primary">${row.especialidad}</span>`
        : '<span class="text-muted">Sin especialidad</span>'
    },
    {
      key: 'telefono', label: 'Teléfono', sortable: true,
      render: (row) => row.telefono || '<span class="text-muted">Sin registrar</span>'
    }
  ];

  readonly acciones = computed<DatatableAction<MedicoResponse>[]>(() => {
    const puede = (a: Accion) => this.menu.puedeHacer(RutaApp.Medicos, a);
    const lista: DatatableAction<MedicoResponse>[] = [
      { type: 'ver', label: 'Ver detalle', onClick: (row) => this.router.navigate(['/medicos', row.id]) },
    ];
    if (puede(Accion.Editar))   lista.push({ type: 'editar',   onClick: (row) => this.router.navigate(['/medicos', row.id, 'editar']) });
    if (puede(Accion.Eliminar)) lista.push({ type: 'eliminar', onClick: (row) => { this.errorEliminar.set(null); this.medicoAEliminar.set(row); } });
    return lista;
  });

  migajas: BreadcrumbItem[] = [{ label: 'Médicos' }];

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
    this.errorEliminar.set(null);
    this.medicosService.eliminar(medico.id).subscribe({
      next: (r) => {
        if (r.success) {
          this.medicos.update(lista => lista.filter(m => m.id !== medico.id));
          this.medicoAEliminar.set(null);
        }
        this.loadingBar.complete();
      },
      error: (err) => {
        this.errorEliminar.set(err.error?.message ?? 'No se pudo eliminar el médico.');
        this.loadingBar.complete();
      }
    });
  }

  exportarPdf(): void {
    this.loadingBar.show();
    const u = this.authService.currentUser();
    const params = { titulo: 'Listado de Médicos', usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/medicos-pdf', params).subscribe({
      next: (blob) => { this.descargarBlob(blob, `medicos-${hoy()}.pdf`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel(): void {
    this.loadingBar.show();
    this.medicosService.exportarExcel().subscribe({
      next: (blob) => { this.descargarBlob(blob, `medicos-${hoy()}.xlsx`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  descargarPlantilla(): void {
    this.loadingBar.show();
    this.medicosService.descargarPlantilla().subscribe({
      next: (blob) => { this.descargarBlob(blob, `plantilla-medicos-${hoy()}.xlsx`); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  private descargarBlob(blob: Blob, nombre: string): void {
    const url  = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = nombre;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}

function hoy(): string {
  return new Date().toISOString().split('T')[0];
}
