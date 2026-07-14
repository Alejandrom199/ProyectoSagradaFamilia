import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { formatearFecha, renderFechaHora } from '../../../../shared/utils/date.utils';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { PrescripcionesService } from '../../../../core/services/prescripciones';
import { AuthService } from '../../../../core/services/auth';
import { Reportes } from '../../../../core/services/reportes';
import { PrescripcionResponse } from '../../../../shared/interfaces/prescripcion.interface';

@Component({
  selector: 'app-listar-prescripciones',
  standalone: true,
  imports: [Datatable, Breadcrumb],

  templateUrl: './historial-prescripciones.html',
})
export class HistorialPrescripciones implements OnInit {
  private prescripcionesService = inject(PrescripcionesService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  private authService = inject(AuthService);
  private reportesService = inject(Reportes);

  prescripciones = signal<PrescripcionResponse[]>([]);

  migajas: BreadcrumbItem[] = [
    { label: 'Prescripciones' },
  ];

  columnas: DatatableColumn<PrescripcionResponse>[] = [
    {
      key: 'fechaCreacion',
      label: 'Fecha',
      sortable: true,
      filterable: true,
      render: (row) => renderFechaHora(row.fechaCreacion),
      exportValue: (row) => formatearFecha(row.fechaCreacion)
    },
    {
      key: 'nombreNino',
      label: 'Paciente',
      sortable: true,
      filterable: true,
      render: (row) => `<span class="font-medium text-[var(--color-text-primary)]">${row.nombreNino}</span>`
    },
    {
      key: 'medicamentos',
      label: 'Medicamentos',
      filterable: true,
      render: (row) => {
        const nombres = row.medicamentos.map(m => m.nombre).join(', ');
        return `<span class="text-sm text-[var(--color-text-secondary)] line-clamp-1" title="${nombres}">${row.medicamentos.length} medicamento(s): ${nombres}</span>`;
      }
    },
    {
      key: 'indicaciones',
      label: 'Indicaciones',
      render: (row) => row.indicaciones || '<span class="text-muted text-xs">Sin indicaciones</span>'
    }
  ];

  acciones: DatatableAction<PrescripcionResponse>[] = [
    {
      type: 'ver',
      label: 'Ver cita',
      icon: 'matVisibilityOutline',
      onClick: (row) => this.router.navigate(['/citas', row.citaId])
    }
  ];

  ngOnInit(): void {
    this.cargarPrescripciones();
  }

  cargarPrescripciones(): void {
    this.loadingBar.show();
    this.prescripcionesService.obtenerMisPrescripciones().subscribe({
      next: (res) => {
        if (res.success) this.prescripciones.set(res.data);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  exportarPdf(): void {
    this.loadingBar.show();
    const u = this.authService.currentUser();
    const params = { titulo: 'Mis Prescripciones', usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/prescripciones-medico-pdf', params).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `mis-prescripciones-${new Date().toISOString().split('T')[0]}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel(): void {
    this.prescripcionesService.exportarExcelMisPrescripciones().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `mis-prescripciones-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {}
    });
  }
}