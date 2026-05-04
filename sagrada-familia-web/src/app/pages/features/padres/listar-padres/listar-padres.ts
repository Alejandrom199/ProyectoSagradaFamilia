import { Component, OnInit, signal, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroPlus, heroPencil, heroTrash, heroUsers } from '@ng-icons/heroicons/outline';

import { PadreResponse } from '../../../../shared/interfaces/responses/padre.response';
import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';

import { formatearFecha } from '../../../../shared/utils/date.utils';
import { Padres } from '../../../../core/services/padres';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { Auth } from '../../../../core/services/auth';
import { ConfirmModal } from "../../../../shared/components/confirm-modal/confirm-modal";
import { Button } from '../../../../shared/components/button/button';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';

@Component({
  selector: 'app-listar-padres',
  standalone: true,
  imports: [NgIcon, RouterLink, Datatable, ConfirmModal, Button, Breadcrumb],
  viewProviders: [provideIcons({ heroPlus, heroPencil, heroTrash, heroUsers })],
  templateUrl: './listar-padres.html',
  styleUrl: './listar-padres.css',
})
export class ListarPadres implements OnInit {
  private padresService = inject(Padres);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  readonly auth = inject(Auth);

  padres = signal<PadreResponse[]>([]);
  padreAEliminar = signal<PadreResponse | null>(null);

  // AQUÍ ESTÁ LA CLAVE: filterable: true en todas las columnas
  columnas: DatatableColumn<PadreResponse>[] = [
    {
      key: 'nombre',
      label: 'Padre/Madre',
      sortable: true,
      filterable: true, // Filtro 1
      render: (row) => `
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-full bg-[var(--color-primary-light)] flex items-center justify-center text-xs font-semibold text-[var(--color-primary)]">
            ${row.nombre.charAt(0)}${row.apellido.charAt(0)}
          </div>
          <div>
            <p class="font-medium">${row.nombre} ${row.apellido}</p>
            <p class="text-xs text-[var(--color-text-secondary)]">${row.email}</p>
          </div>
        </div>`
    },
    {
      key: 'telefono',
      label: 'Teléfono',
      sortable: true,
      filterable: true, // Filtro 2
      render: (row) => row.telefono || '<span class="text-gray-400">Sin registrar</span>'
    },
    {
      key: 'totalHijos',
      label: 'Hijos',
      sortable: true,
      filterable: true, // Filtro 3
      render: (row) => `<span class="badge badge-primary">${row.totalHijos ?? 0}</span>`
    },
    {
      key: 'fechaCreacion',
      label: 'Registro',
      sortable: true,
      filterable: true, // Filtro 4
      render: (row) => formatearFecha(row.fechaCreacion)
    }
  ];

  acciones: DatatableAction<PadreResponse>[] = [
    {
      type: 'ver',
      label: 'Ver hijos',
      onClick: (row) => this.router.navigate(['/padres', row.id, 'hijos'])
    },
    {
      type: 'editar',
      onClick: (row) => this.router.navigate(['/padres', row.id, 'editar'])
    },
    {
      type: 'eliminar',
      onClick: (row) => this.padreAEliminar.set(row)
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Padres' },
  ];

  ngOnInit(): void {
    this.cargarPadres();
  }

  cargarPadres(): void {
    this.loadingBar.show();
    this.padresService.obtenerTodos().subscribe({
      next: (response) => {
        if (response.success) {
          this.padres.set(response.data);
        } else {
          console.error(response.message, response.errors);
        }
        this.loadingBar.complete();
      },
      error: (err) => {
        console.error('Error al cargar padres', err);
        this.loadingBar.complete();
      }
    });
  }

  confirmarEliminar(): void {
    const padre = this.padreAEliminar();
    if (!padre) return;

    this.loadingBar.show();
    this.padresService.eliminar(padre.id).subscribe({
      next: (response) => {
        if (response.success) {
          this.padres.update(actuales => actuales.filter(p => p.id !== padre.id));
        } else {
          console.error(response.message);
        }
        this.padreAEliminar.set(null);
        this.loadingBar.complete();
      },
      error: (err) => {
        console.error('Error al eliminar padre', err);
        this.padreAEliminar.set(null);
        this.loadingBar.complete();
      }
    });
  }
}