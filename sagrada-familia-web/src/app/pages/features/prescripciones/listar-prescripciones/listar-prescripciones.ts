import { Component, OnInit, Input, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { NinosService } from '../../../../core/services/ninos';
import { MedicosService } from '../../../../core/services/medicos';
import { AuthService } from '../../../../core/services/auth';
import { Reportes } from '../../../../core/services/reportes';
import { PrescripcionResponse } from '../../../../shared/interfaces/prescripcion.interface';
import { NinoDetailResponse, NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { formatearFecha, renderFechaHora } from '../../../../shared/utils/date.utils';
import { PrescripcionesService } from '../../../../core/services/prescripciones';

@Component({
  selector: 'app-listar-prescripciones',
  standalone: true,
  imports: [Datatable, Breadcrumb],

  templateUrl: './listar-prescripciones.html',
  styleUrl: './listar-prescripciones.css',
})
export class ListarPrescripciones implements OnInit {
  @Input() ninoId!: string;

  private prescripcionesService = inject(PrescripcionesService);
  private ninosService = inject(NinosService);
  private medicosService = inject(MedicosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  private authService = inject(AuthService);
  private reportesService = inject(Reportes);

  nino = signal<NinoDetailResponse | null>(null);
  prescripciones = signal<PrescripcionResponse[]>([]);
  totalPrescripciones = signal(0);
  filterOptions = signal<Record<string, string[]>>({});
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'desc', columnFilters: {} };

  columnas: DatatableColumn<PrescripcionResponse>[] = [
    {
      key: 'fechaCreacion', label: 'Fecha', sortable: true,
      render: (row) => renderFechaHora(row.fechaCreacion),
      exportValue: (row) => formatearFecha(row.fechaCreacion)
    },
    {
      key: 'nombreMedico', label: 'Médico', sortable: true, filterable: true,
      render: (row) => `
        <div>
          <p class="font-medium text-[var(--color-text-primary)]">Dr(a). ${row.nombreMedico}</p>
          ${row.especialidadMedico ? `<p class="text-xs text-muted">${row.especialidadMedico}</p>` : ''}
        </div>`
    },
    {
      key: 'medicamentos', label: 'Medicamentos',
      render: (row) => {
        const nombres = row.medicamentos.map(m => m.nombre).join(', ');
        return `<p class="text-sm text-[var(--color-text-primary)] max-w-md truncate" title="${nombres}">${row.medicamentos.length} medicamento(s): ${nombres}</p>`;
      },
      exportValue: (row) => row.medicamentos.map(m => `${m.nombre} (${m.dosis}, ${m.frecuencia})`).join('; ')
    },
    {
      key: 'indicaciones', label: 'Indicaciones',
      render: (row) => row.indicaciones
        ? `<p class="text-xs text-[var(--color-text-secondary)] max-w-xs truncate" title="${row.indicaciones}">${row.indicaciones}</p>`
        : '<span class="text-muted">—</span>'
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
    this.cargarFilterOptions();
  }

  cargarFilterOptions(): void {
    this.medicosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          this.filterOptions.set({
            nombreMedico: r.data.map(m => `${m.nombre} ${m.apellido}`),
          });
        }
      }
    });
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

  exportarPdf(): void {
    const n = this.nino();
    const u = this.authService.currentUser();
    const titulo = n ? `Prescripciones — ${n.nombre} ${n.apellido}` : 'Historial de Prescripciones';
    const params = { ninoId: this.ninoId, titulo, usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/prescripciones-pdf', params).subscribe({
      next: (blob) => { this.descargarBlob(blob, `prescripciones-${hoy()}.pdf`); },
      error: () => { }
    });
  }

  descargarExcel(): void {
    this.prescripcionesService.exportarExcelPorNino(parseInt(this.ninoId)).subscribe({
      next: (blob) => this.descargarBlob(blob, `prescripciones-${hoy()}.xlsx`),
      error: () => { }
    });
  }

  private descargarBlob(blob: Blob, nombre: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = nombre;
    link.click();
    window.URL.revokeObjectURL(url);
  }
}

function hoy(): string { return new Date().toISOString().split('T')[0]; }