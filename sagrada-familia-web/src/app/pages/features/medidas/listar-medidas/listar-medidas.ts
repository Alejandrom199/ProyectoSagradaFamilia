import { Component, OnInit, signal, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { CommonModule } from '@angular/common';

import { Datatable, DatatableAction, DatatableColumn, ServerQuery } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { NinosService } from '../../../../core/services/ninos';
import { MedidasService } from '../../../../core/services/medidas';
import { AuthService } from '../../../../core/services/auth';
import { Reportes } from '../../../../core/services/reportes';
import { MedidaResponse } from '../../../../shared/interfaces/medida.interface';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ESTADO_NUTRICIONAL_COLOR, ESTADO_NUTRICIONAL_LABEL, EstadoNutricional } from '../../../../shared/constants/estado-nutricional.constants';
import { badgeHtml } from '../../../../shared/utils/badge.util';

@Component({
  selector: 'listar-medidas',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIcon, Datatable, Button, ConfirmModal, Breadcrumb],
  templateUrl: './listar-medidas.html',
})
export class ListarMedidas implements OnInit {
  id!: string;
  private route            = inject(ActivatedRoute);
  private medidasService   = inject(MedidasService);
  private ninosService     = inject(NinosService);
  private authService      = inject(AuthService);
  private reportesService  = inject(Reportes);

  medidas = signal<MedidaResponse[]>([]);
  totalMedidas = signal(0);
  nino = signal<NinoDetailResponse | null>(null);

  readonly filterOptions: Record<string, string[]> = {
    estadoNutricional: ['Normal', 'BajoPeso', 'BajoPesoSevero', 'Sobrepeso', 'Obesidad'],
  };
  private queryActual: ServerQuery = { page: 1, pageSize: 10, search: '', sortBy: '', sortDir: 'desc', columnFilters: {} };
  migajas = signal<BreadcrumbItem[]>([]);

  modalAbierto = signal(false);
  editando = signal<MedidaResponse | null>(null);
  medidaAEliminar = signal<MedidaResponse | null>(null);
  guardando = signal(false);
  errorModal = signal('');

  hoy = new Date().toISOString().split('T')[0];
  readonly formatearFecha = formatearFecha;

  form = { fechaMedicion: '', peso: 0, talla: 0 };

  columnas: DatatableColumn<MedidaResponse>[] = [
    {
      key: 'fechaMedicion', label: 'Fecha', sortable: true,
      render: (r) => formatearFecha(r.fechaMedicion)
    },
    {
      key: 'peso', label: 'Peso', sortable: true,
      render: (r) => `<span class="font-medium text-slate-700 dark:text-zinc-300">${r.peso} kg</span>`
    },
    {
      key: 'talla', label: 'Talla', sortable: true,
      render: (r) => `<span class="font-medium text-slate-700 dark:text-zinc-300">${r.talla} cm</span>`
    },
    {
      key: 'estadoNutricional', label: 'Estado', sortable: true, filterable: true,
      render: (r) => {
        const estado = r.estadoNutricional as EstadoNutricional;
        return badgeHtml(ESTADO_NUTRICIONAL_LABEL[estado] ?? r.estadoNutricional, ESTADO_NUTRICIONAL_COLOR[estado] ?? 'muted');
      }
    },
    {
      key: 'percentilPeso', label: 'P. Peso', sortable: true,
      render: (r) => `<span class="text-xs text-muted">P${r.percentilPeso}</span>`
    },
    {
      key: 'percentilTalla', label: 'P. Talla', sortable: true,
      render: (r) => `<span class="text-xs text-muted">P${r.percentilTalla}</span>`
    },
  ];

  acciones: DatatableAction<MedidaResponse>[] = [
    { type: 'editar', onClick: (r) => this.abrirModal(r) },
    { type: 'eliminar', onClick: (r) => this.medidaAEliminar.set(r) },
  ];

  ngOnInit() {
    const idUrl = this.route.snapshot.paramMap.get('id');
    if (!idUrl) return;

    this.id = idUrl;
    const ninoId = parseInt(this.id, 10);

    this.ninosService.obtenerPorId(ninoId).subscribe(r => {
      if (r.success && r.data) {
        this.nino.set(r.data);
        this.migajas.set([
          { label: 'Pacientes', ruta: '/pacientes' },
          { label: `${r.data.nombre} ${r.data.apellido}`, ruta: `/pacientes/${this.id}` },
          { label: 'Medidas' },
        ]);
      }
    });

    this.cargarMedidas();
  }

  cargarMedidas() {
    const { page, pageSize, search, sortBy, sortDir } = this.queryActual;
    this.medidasService.obtenerPaginadoPorNino(parseInt(this.id, 10), page, pageSize, search, sortBy, sortDir === 'asc').subscribe(r => {
      if (r.success) {
        this.medidas.set(r.data);
        this.totalMedidas.set(r.totalItems);
      }
    });
  }

  onServerQuery(query: ServerQuery): void {
    this.queryActual = query;
    this.cargarMedidas();
  }

  abrirModal(medida?: MedidaResponse) {
    this.editando.set(medida ?? null);
    this.errorModal.set('');
    this.form = medida
      ? { fechaMedicion: medida.fechaMedicion, peso: medida.peso, talla: medida.talla }
      : { fechaMedicion: this.hoy, peso: 0, talla: 0 };
    this.modalAbierto.set(true);
  }

  cerrarModal() {
    this.modalAbierto.set(false);
    this.editando.set(null);
  }

  guardar() {
    if (!this.form.fechaMedicion || !this.form.peso || !this.form.talla) {
      this.errorModal.set('Completá todos los campos obligatorios.');
      return;
    }

    this.guardando.set(true);
    this.errorModal.set('');

    const medida = this.editando();
    const obs = medida
      ? this.medidasService.actualizar(medida.id, this.form)
      : this.medidasService.crear({ ...this.form, ninoId: parseInt(this.id, 10) });

    obs.subscribe({
      next: (r) => {
        if (r.success) {
          this.cargarMedidas();
          this.cerrarModal();
        } else {
          this.errorModal.set(r.message ?? 'Error al guardar.');
        }
        this.guardando.set(false);
      },
      error: (err) => {
        // Maneja tanto ApiResponse.Fail ({ message }) como ValidationProblemDetails ({ errors, title })
        const msg =
          err.error?.message ??
          (err.error?.errors
            ? (Object.values(err.error.errors) as string[][]).flat().join(' ')
            : null) ??
          err.error?.title ??
          'Error al guardar. Intentá nuevamente.';
        this.errorModal.set(msg);
        this.guardando.set(false);
      },
    });
  }

  exportarPdf(): void {
    const n = this.nino();
    const u = this.authService.currentUser();
    const titulo = n ? `Medidas - ${n.nombre} ${n.apellido}` : 'Historial de Medidas';
    const params = { ninoId: this.id, titulo, usuario: u ? `${u.nombre} ${u.apellido}` : '' };
    this.reportesService.descargarReportePdf('reportes/medidas-pdf', params).subscribe({
      next: (blob) => { this.descargarBlob(blob, `medidas-${hoy()}.pdf`); },
      error: () => {}
    });
  }

  descargarExcel(): void {
    this.medidasService.exportarExcelPorNino(parseInt(this.id, 10)).subscribe({
      next: (blob) => this.descargarBlob(blob, `medidas-${hoy()}.xlsx`),
      error: () => {}
    });
  }

  private descargarBlob(blob: Blob, nombre: string): void {
    const url  = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = nombre;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  confirmarEliminar() {
    const m = this.medidaAEliminar();
    if (!m) return;
    this.medidasService.eliminar(m.id).subscribe(r => {
      if (r.success) this.cargarMedidas();
      this.medidaAEliminar.set(null);
    });
  }
}

function hoy(): string { return new Date().toISOString().split('T')[0]; }
