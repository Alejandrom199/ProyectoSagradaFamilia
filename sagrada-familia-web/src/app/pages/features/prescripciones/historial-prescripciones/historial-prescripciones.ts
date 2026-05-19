import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroEye, heroBeaker } from '@ng-icons/heroicons/outline';

import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { PrescripcionesService } from '../../../../core/services/prescripciones';
import { PrescripcionResponse } from '../../../../shared/interfaces/prescripcion.interface';

@Component({
  selector: 'app-listar-prescripciones',
  standalone: true,
  imports: [Datatable, Breadcrumb],
  viewProviders: [provideIcons({ heroEye, heroBeaker })],
  templateUrl: './historial-prescripciones.html',
})
export class HistorialPrescripciones implements OnInit {
  private prescripcionesService = inject(PrescripcionesService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

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
      render: (row) => `<span class="font-medium text-gray-800">${formatearFecha(row.fechaCreacion)}</span>`,
      exportValue: (row) => formatearFecha(row.fechaCreacion)
    },
    {
      key: 'nombreNino',
      label: 'Paciente',
      sortable: true,
      filterable: true,
      render: (row) => `<span class="font-medium text-gray-800">${row.nombreNino}</span>`
    },
    {
      key: 'diagnostico',
      label: 'Diagnóstico',
      filterable: true,
      render: (row) => row.diagnostico || '<span class="text-gray-400 text-xs">Sin diagnóstico</span>'
    },
    {
      key: 'detalleMedicamentos',
      label: 'Medicamentos',
      filterable: true,
      render: (row) => `<span class="text-sm text-gray-700 line-clamp-1">${row.detalleMedicamentos}</span>`
    },
    {
      key: 'indicaciones',
      label: 'Indicaciones',
      render: (row) => row.indicaciones || '<span class="text-gray-400 text-xs">Sin indicaciones</span>'
    }
  ];

  acciones: DatatableAction<PrescripcionResponse>[] = [
    {
      type: 'ver',
      label: 'Ver cita',
      icon: 'heroEye',
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
}