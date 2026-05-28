import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { DatatableColumn, Datatable, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { LogSistemaResponse } from '../../../../shared/interfaces/sistema.interface';
import { SistemaService } from '../../../../core/services/sistema';

type NivelFiltro = 'Todo' | 'Information' | 'Warning' | 'Error';

@Component({
  selector: 'app-listar-logs',
  standalone: true,
  imports: [RouterLink, Datatable, Breadcrumb],
  templateUrl: './listar-logs.html',
  styleUrl: './listar-logs.css',
})
export class ListarLogs implements OnInit {
  private sistemaService = inject(SistemaService);
  private loadingBar = inject(LoadingBar);

  logs = signal<LogSistemaResponse[]>([]);
  totalLogs = signal(0);
  nivelActivo = signal<NivelFiltro>('Todo');
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'desc' };

  readonly niveles: NivelFiltro[] = ['Todo', 'Information', 'Warning', 'Error'];

  columnas: DatatableColumn<LogSistemaResponse>[] = [
    {
      key: 'fechaHora', label: 'Fecha', sortable: true,
      render: (row) => {
        const fecha = new Date(row.fechaHora);
        return `
          <div>
            <p class="font-medium text-gray-800 text-sm">${fecha.toLocaleDateString('es-EC')}</p>
            <p class="text-xs text-gray-500">${fecha.toLocaleTimeString('es-EC')}</p>
          </div>`;
      }
    },
    {
      key: 'nivel', label: 'Nivel', sortable: true,
      render: (row) => this.badgeNivel(row.nivel)
    },
    {
      key: 'mensaje', label: 'Mensaje', filterable: true,
      render: (row) => `<p class="text-sm text-gray-800 max-w-md truncate" title="${row.mensaje.replace(/"/g, '&quot;')}">${row.mensaje}</p>`
    },
    {
      key: 'endpoint', label: 'Endpoint',
      render: (row) => row.endpoint
        ? `<code class="text-xs bg-gray-100 px-2 py-1 rounded">${row.endpoint}</code>`
        : '<span class="text-gray-400">—</span>'
    },
    {
      key: 'usuarioId', label: 'Usuario',
      render: (row) => row.usuarioId
        ? `<span class="text-xs text-gray-600">ID ${row.usuarioId}</span>`
        : '<span class="text-gray-400">Sistema</span>'
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Sistema' },
    { label: 'Logs del sistema' },
  ];

  ngOnInit(): void {
    this.cargarLogs();
  }

  claseTab(nivel: NivelFiltro): string {
    const base = 'px-4 py-1.5 rounded-xl text-sm font-bold transition-all';
    return this.nivelActivo() === nivel
      ? `${base} bg-blue-600 text-white shadow-sm`
      : `${base} text-gray-600 bg-white border border-gray-200 hover:bg-gray-50`;
  }

  cargarLogs(): void {
    const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
    const nivel = this.nivelActivo();
    const searchParam = nivel !== 'Todo' ? nivel : search;
    this.loadingBar.show();
    this.sistemaService.obtenerLogsPaginado(page, pageSize, searchParam, sortBy, sortDir === 'asc').subscribe({
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

  cambiarNivel(nivel: NivelFiltro): void {
    this.nivelActivo.set(nivel);
    this.queryActual = { ...this.queryActual, page: 1, search: '' };
    this.cargarLogs();
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
