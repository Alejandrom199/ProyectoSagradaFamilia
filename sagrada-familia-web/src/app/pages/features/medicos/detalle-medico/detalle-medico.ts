import { Component, OnInit, Input, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { NgIcon } from '@ng-icons/core';


import { formatearEdad, formatearFecha, renderFechaHora } from '../../../../shared/utils/date.utils';
import { DatatableAction, DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { NinosService } from '../../../../core/services/ninos';
import { MedicoResponse } from '../../../../shared/interfaces/medico.interface';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { MedicosService } from '../../../../core/services/medicos';
import { AuthService } from '../../../../core/services/auth';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { finalize, pipe } from 'rxjs';

@Component({
  selector: 'app-detalle-medico',
  standalone: true,
  imports: [NgIcon, Datatable, Breadcrumb, ConfirmModal],

  templateUrl: './detalle-medico.html',
  styleUrl: './detalle-medico.css',
})
export class DetalleMedico implements OnInit {
  @Input() id!: string;

  private medicosService = inject(MedicosService);
  private ninosService = inject(NinosService);
  private router = inject(Router);
  private loadingBar = inject(LoadingBar);
  auth = inject(AuthService);

  medico = signal<MedicoResponse | null>(null);
  pacientes = signal<NinoResponse[]>([]);
  mostrarModalReset = signal(false);
  resetExito = signal(false);
  resetError = signal('');

  formatearFecha = formatearFecha;

  columnas: DatatableColumn<NinoResponse>[] = [
    {
      key: 'nombre', label: 'Paciente', sortable: true, filterable: true,
      render: (row) => row.nombre + ' ' + row.apellido
    },
    {
      key: 'edadMeses', label: 'Edad', sortable: true,
      render: (row) => formatearEdad(row.edadMeses)
    },
    {
      key: 'nombrePadre', label: 'Representante', sortable: true, filterable: true
    },
    {
      key: 'fechaCreacion', label: 'Registro', sortable: true,
      render: (row) => renderFechaHora(row.fechaCreacion)
    }
  ];

  acciones: DatatableAction<NinoResponse>[] = [
    {
      type: 'ver',
      onClick: (row) => this.router.navigate(['/pacientes', row.id])
    },
    {
      type: 'medidas',
      onClick: (row) => this.router.navigate(['/medidas', row.id])
    }
  ];

  migajas: BreadcrumbItem[] = [
    { label: 'Médicos', ruta: '/medicos' },
    { label: 'Detalle del Médico' },
  ];

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos(): void {
    this.loadingBar.show();

    this.medicosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          const m = r.data.find(x => x.id === parseInt(this.id));
          if (m) this.medico.set(m);
        }
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });

    this.cargarPacientes();
  }

  cargarPacientes(): void {
    this.ninosService.obtenerTodos().subscribe({
      next: (r) => {
        if (r.success) {
          const pacientesDelMedico = r.data.filter(n => n.medicoId === parseInt(this.id));
          this.pacientes.set(pacientesDelMedico);
        }
      }
    });
  }

  confirmarRestablecerPassword(): void {
    this.loadingBar.show();
    this.medicosService.restablecerPassword(parseInt(this.id))
      .pipe(finalize(() => this.loadingBar.complete()))
      .subscribe({
        next: () => {
          this.mostrarModalReset.set(false);
          this.resetExito.set(true);
          this.resetError.set('');
        },
        error: (err) => {
          this.mostrarModalReset.set(false);
          this.resetError.set(err?.error?.message ?? 'No se pudo enviar el correo de restablecimiento.');
        }
      });
  }
}
