import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { LogSistemaResponse } from '../../../../shared/interfaces/sistema.interface';
import { renderFechaHora } from '../../../../shared/utils/date.utils';
import { SistemaService } from '../../../../core/services/sistema';
import { AuthService } from '../../../../core/services/auth';
import { Reportes } from '../../../../core/services/reportes';

@Component({
  selector: 'listar-logs',
  standalone: true,
  imports: [Datatable, Breadcrumb],
  templateUrl: './listar-logs.html',
  styleUrl: './listar-logs.css',
})
export class ListarLogs implements OnInit {
  private sistemaService = inject(SistemaService);
  private loadingBar = inject(LoadingBar);
  private authService = inject(AuthService);
  private reportesService = inject(Reportes);

  logs = signal<LogSistemaResponse[]>([]);
  totalLogs = signal(0);
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'desc', columnFilters: {} };

  columnas: DatatableColumn<LogSistemaResponse>[] = [
    {
      key: 'fechaHora', label: 'Fecha', sortable: true, filterable: true,
      render: (row) => renderFechaHora(row.fechaHora)
    },
    {
      key: 'nivel', label: 'Nivel', sortable: true, filterable: true,
      render: (row) => this.badgeNivel(row.nivel)
    },
    {
      key: 'mensaje', label: 'Mensaje', sortable: true,
      render: (row) => `<p class="text-sm text-gray-800 max-w-md truncate" title="${row.mensaje.replace(/"/g, '&quot;')}">${row.mensaje}</p>`
    },
    {
      key: 'endpoint', label: 'Endpoint',
      render: (row) => row.endpoint
        ? `<code class="text-xs bg-gray-100 px-2 py-1 rounded">${row.endpoint}</code>`
        : '<span class="text-gray-400">—</span>'
    },
    {
      key: 'usuarioId', label: 'Usuario', filterable: true,
      render: (row) => row.usuarioId
        ? `<span class="text-xs text-gray-600">ID ${row.usuarioId}</span>`
        : '<span class="text-gray-400">Sistema</span>'
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Eventos del sistema' },
  ];

  ngOnInit(): void {
    this.cargarLogs();
  }

  cargarLogs(): void {
    const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
    this.loadingBar.show();
    this.sistemaService.obtenerLogsPaginado(page, pageSize, search, sortBy, sortDir === 'asc').subscribe({
      next: (r) => {
        if (r.success) {
          this.logs.set(r.data);
          this.totalLogs.set(r.totalItems);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarLogs();
  }

  exportarPdf(): void {
    this.loadingBar.show();
    const u = this.authService.currentUser();
    const params = { titulo: 'Eventos del Sistema', usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/logs-pdf', params).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `logs-${new Date().toISOString().split('T')[0]}.pdf`;
        link.click();
        window.URL.revokeObjectURL(url);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  descargarExcel(): void {
    this.sistemaService.exportarLogsExcel().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `eventos-sistema-${new Date().toISOString().split('T')[0]}.xlsx`;
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {}
    });
  }

  private badgeNivel(nivel: string): string {
    const mapa: Record<string, string> = {
      'Error': 'bg-red-100 text-red-700',
      'Warning': 'bg-yellow-100 text-yellow-700',
      'Information': 'bg-blue-100 text-blue-700',
      'Critical': 'bg-purple-100 text-purple-700'
    };
    const clase = mapa[nivel] || 'bg-gray-100 text-gray-700';
    return `<span class="px-2 py-1 rounded-full text-[10px] font-bold uppercase ${clase}">${nivel}</span>`;
  }
}
