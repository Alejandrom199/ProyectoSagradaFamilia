import { Component, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroMagnifyingGlass, heroTableCells, heroEye } from '@ng-icons/heroicons/outline';

import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { SistemaService } from '../../../../core/services/sistema';
import { AuditoriaResponse } from '../../../../shared/interfaces/sistema.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';

// Tablas auditables comunes — pueden ampliarse según el backend
const TABLAS_AUDITABLES = [
  'Padres',
  'Medicos',
  'Ninos',
  'Medidas',
  'Citas',
  'Prescripciones',
  'Usuarios',
  'Alimentos'
];

@Component({
  selector: 'app-listar-auditoria',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, Breadcrumb, ReactiveFormsModule],
  viewProviders: [provideIcons({ heroMagnifyingGlass, heroTableCells, heroEye })],
  templateUrl: './listar-auditoria.html',
  styleUrl: './listar-auditoria.css',
})
export class ListarAuditoria {
  private fb = inject(FormBuilder);
  private sistemaService = inject(SistemaService);
  private loadingBar = inject(LoadingBar);

  formFiltro: FormGroup;
  registros = signal<AuditoriaResponse[]>([]);
  busqueda = signal(false);
  registroDetalle = signal<AuditoriaResponse | null>(null);

  tablas = TABLAS_AUDITABLES;

  columnas: DatatableColumn<AuditoriaResponse>[] = [
    {
      key: 'fecha', label: 'Fecha', sortable: true,
      render: (row) => `<span class="text-sm text-gray-700">${formatearFecha(row.fecha)}</span>`
    },
    {
      key: 'accion', label: 'Acción', sortable: true, filterable: true,
      render: (row) => this.badgeAccion(row.accion)
    },
    {
      key: 'usuarioEmail', label: 'Usuario', sortable: true, filterable: true,
      render: (row) => `<span class="text-sm text-gray-700">${row.usuarioEmail || 'Sistema'}</span>`
    },
    {
      key: 'clavePrimaria', label: 'Registro',
      render: (row) => `<code class="text-xs bg-gray-100 px-2 py-1 rounded">${row.tabla} #${row.clavePrimaria}</code>`
    },
    {
      key: 'ipAddress', label: 'IP',
      render: (row) => row.ipAddress ? `<span class="text-xs text-gray-500">${row.ipAddress}</span>` : '<span class="text-gray-400">—</span>'
    }
  ];

  acciones: DatatableAction<AuditoriaResponse>[] = [
    {
      type: 'ver',
      label: 'Ver cambio',
      onClick: (row) => this.abrirDetalle(row)
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Sistema' },
    { label: 'Auditoría' },
  ];

  constructor() {
    this.formFiltro = this.fb.group({
      tabla: ['Padres', Validators.required],
      pk: ['', Validators.required]
    });
  }

  get f() {
    return this.formFiltro.controls;
  }

  buscar(): void {
    if (this.formFiltro.invalid) {
      this.formFiltro.markAllAsTouched();
      return;
    }

    const { tabla, pk } = this.formFiltro.value;
    this.busqueda.set(true);
    this.loadingBar.show();

    this.sistemaService.obtenerAuditoriaPorTabla(tabla, pk).subscribe({
      next: (r) => {
        if (r.success) this.registros.set(r.data);
        this.loadingBar.complete();
      },
      error: () => {
        this.registros.set([]);
        this.loadingBar.complete();
      }
    });
  }

  abrirDetalle(row: AuditoriaResponse): void {
    this.registroDetalle.set(row);
  }

  private badgeAccion(accion: string): string {
    const mapa: Record<string, string> = {
      'INSERT': 'bg-green-100 text-green-700',
      'UPDATE': 'bg-blue-100 text-blue-700',
      'DELETE': 'bg-red-100 text-red-700'
    };
    const clase = mapa[accion.toUpperCase()] || 'bg-gray-100 text-gray-700';
    return `<span class="px-2 py-1 rounded-full text-[10px] font-bold uppercase ${clase}">${accion}</span>`;
  }
}
