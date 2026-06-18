import { Component, OnInit, Input, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { NinosService } from '../../../../core/services/ninos';
import { PrescripcionResponse } from '../../../../shared/interfaces/prescripcion.interface';
import { NinoDetailResponse, NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { PrescripcionesService } from '../../../../core/services/prescripciones';

@Component({
  selector: 'app-listar-prescripciones',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, Button, Breadcrumb],

  templateUrl: './listar-prescripciones.html',
  styleUrl: './listar-prescripciones.css',
})
export class ListarPrescripciones implements OnInit {
  @Input() ninoId!: string;

  private prescripcionesService = inject(PrescripcionesService);
  private ninosService = inject(NinosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  // SOLUCIÓN: Cambiado a NinoDetailResponse para alinearse con el servicio de consulta por ID
  nino = signal<NinoDetailResponse | null>(null);
  prescripciones = signal<PrescripcionResponse[]>([]);
  totalPrescripciones = signal(0);
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'desc', columnFilters: {} };

  columnas: DatatableColumn<PrescripcionResponse>[] = [
    {
      key: 'fechaCreacion', label: 'Fecha', sortable: true,
      render: (row) => `<span class="font-medium text-gray-700">${formatearFecha(row.fechaCreacion)}</span>`,
      exportValue: (row) => formatearFecha(row.fechaCreacion)
    },
    {
      key: 'nombreMedico', label: 'Médico', sortable: true, filterable: true,
      render: (row) => `
        <div>
          <p class="font-medium text-gray-800">Dr(a). ${row.nombreMedico}</p>
          ${row.especialidadMedico ? `<p class="text-xs text-gray-500">${row.especialidadMedico}</p>` : ''}
        </div>`
    },
    {
      key: 'detalleMedicamentos', label: 'Medicamentos', filterable: true,
      render: (row) => `<p class="text-sm text-gray-800 max-w-md">${row.detalleMedicamentos}</p>`
    },
    {
      key: 'indicaciones', label: 'Indicaciones',
      render: (row) => row.indicaciones
        ? `<p class="text-xs text-gray-600 max-w-xs truncate" title="${row.indicaciones}">${row.indicaciones}</p>`
        : '<span class="text-gray-400">—</span>'
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Prescripciones' },
  ];

  ngOnInit(): void {
    const id = parseInt(this.ninoId);
    this.loadingBar.show();

    this.ninosService.obtenerPorId(id).subscribe({
      next: (r) => {
        if (r.success) {
          this.nino.set(r.data);
          this.migajas = [
            { label: 'Pacientes', ruta: '/pacientes' },
            { label: `${r.data.nombre} ${r.data.apellido}`, ruta: `/pacientes/${this.ninoId}` },
            { label: 'Prescripciones' },
          ];
        }
      }
    });

    this.cargarPrescripciones();
  }

  cargarDatos(): void {
    const id = parseInt(this.ninoId);
    this.loadingBar.show();

    this.ninosService.obtenerPorId(id).subscribe({
      next: (r) => { if (r.success) this.nino.set(r.data); }
    });

    this.cargarPrescripciones();
  }

  cargarPrescripciones(): void {
    const id = parseInt(this.ninoId);
    const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
    this.loadingBar.show();
    this.prescripcionesService.obtenerPaginadoPorNino(id, page, pageSize, search, sortBy, sortDir === 'asc').subscribe({
      next: (r) => {
        if (r.success) {
          this.prescripciones.set(r.data);
          this.totalPrescripciones.set(r.totalItems);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarPrescripciones();
  }
}