import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroArrowLeft, heroPlus, heroPencil, heroTrash } from '@ng-icons/heroicons/outline';
import { formatearEdad, formatearFecha } from '../../../../shared/utils/date.utils';
import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { PadresService } from '../../../../core/services/padres';
import { NinosService } from '../../../../core/services/ninos';
import { PadreResponse } from '../../../../shared/interfaces/padre.interface';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';


@Component({
  selector: 'app-detalle-padre',
  imports: [NgIcon, RouterLink, Datatable, Breadcrumb],
  viewProviders: [provideIcons({ heroArrowLeft, heroPlus, heroPencil, heroTrash })],
  templateUrl: './detalle-padre.html',
  styleUrl: './detalle-padre.css',
})
export class DetallePadre implements OnInit {
  @Input() id!: string;

  private padresService = inject(PadresService);
  private ninosService = inject(NinosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);

  padre = signal<PadreResponse | null>(null);
  hijos = signal<NinoResponse[]>([]);
  hijoAEliminar = signal<NinoResponse | null>(null);

  formatearFecha = formatearFecha;

  columnas: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre', label: 'Hijo/a', sortable: true, filterable: true,
      render: (row) => `
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-full ${row.sexo === 'M' ? 'bg-blue-100 text-blue-600' : 'bg-pink-100 text-pink-600'} flex items-center justify-center text-xs font-semibold">
            ${row.nombreCompleto.charAt(0)}${row.nombreCompleto.charAt(1)}
          </div>
          <div>
            <p class="font-medium">${row.nombreCompleto}</p>
            <p class="text-xs text-[var(--color-text-secondary)]">${row.sexo === 'M' ? 'Varón' : 'Niña'}</p>
          </div>
        </div>`
    },
    {
      key: 'edadMeses', label: 'Edad', sortable: true,
      render: (row) => formatearEdad(row.edadMeses)
    },
    {
      key: 'fechaNacimiento', label: 'Nacimiento', sortable: true,
      render: (row) => formatearFecha(row.fechaNacimiento)
    },
    {
      key: 'fechaCreacion', label: 'Registro', sortable: true,
      render: (row) => formatearFecha(row.fechaCreacion)
    }
  ];

  acciones: DatatableAction<NinoResponse>[] = [
    {
      type: 'editar',
      onClick: (row) => this.router.navigate(['/padres', this.id, 'hijos', row.id, 'editar'])
    },
    {
      type: 'eliminar',
      onClick: (row) => this.hijoAEliminar.set(row)
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Padres', ruta: '/padres' },
    { label: 'Detalle del Padre' },
  ];

  ngOnInit() {
    this.cargarDatos();
  }

  cargarDatos() {
    this.loadingBar.show();

    this.padresService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          const p = r.data.find(x => x.id === parseInt(this.id));
          if (p) this.padre.set(p);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });

    this.cargarHijos();
  }

  cargarHijos() {
    this.ninosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          const hijosDelPadre = r.data.filter(n => n.padreId === parseInt(this.id));
          this.hijos.set(hijosDelPadre);
        }
      }
    });
  }

  confirmarEliminar() {
    const hijo = this.hijoAEliminar();
    if (!hijo) return;

    this.loadingBar.show();
    this.ninosService.eliminar(hijo.id).subscribe({
      next: (r) => {
        if (r.success) {
          this.cargarHijos();
          this.cargarDatos();
        }
        this.hijoAEliminar.set(null);
        this.loadingBar.complete();
      },
      error: () => {
        this.hijoAEliminar.set(null);
        this.loadingBar.complete();
      }
    });
  }
}
