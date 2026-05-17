import { Component, OnInit, signal, inject } from '@angular/core';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgIcon, provideIcons } from '@ng-icons/core';
import { heroArrowLeft, heroPlus, heroTrash, heroPencil } from '@ng-icons/heroicons/outline';
import { CommonModule } from '@angular/common';

import { Datatable, DatatableAction, DatatableColumn } from '../../../../shared/components/datatable/datatable';
import { Button } from '../../../../shared/components/button/button';
import { NinosService } from '../../../../core/services/ninos';
import { MedidasService } from '../../../../core/services/medidas';
import { MedidaResponse } from '../../../../shared/interfaces/medida.interface';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface'; // 💡 Actualizado a DetailResponse
import { formatearFecha } from '../../../../shared/utils/date.utils';


@Component({
  selector: 'listar-medidas',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIcon, Datatable, Button],
  viewProviders: [provideIcons({ heroArrowLeft, heroPlus, heroTrash, heroPencil })],
  templateUrl: './listar-medidas.html',
})
export class ListarMedidas implements OnInit {
  id!: string;
  private route = inject(ActivatedRoute);
  private medidasService = inject(MedidasService);
  private ninosService = inject(NinosService);

  medidas = signal<MedidaResponse[]>([]);
  nino = signal<NinoDetailResponse | null>(null); // 💡 Actualizado

  modalAbierto = signal(false);
  editando = signal<MedidaResponse | null>(null);
  medidaAEliminar = signal<MedidaResponse | null>(null);
  guardando = signal(false);
  errorModal = signal('');

  hoy = new Date().toISOString().split('T')[0];
  readonly formatearFecha = formatearFecha;

  form = { fechaMedicion: '', peso: 0, talla: 0 };

  columnas: DatatableColumn<MedidaResponse>[] = [
    { key: 'fechaMedicion', label: 'Fecha', sortable: true, render: (r) => formatearFecha(r.fechaMedicion) },
    { key: 'peso', label: 'Peso', sortable: true, render: (r) => `<span class="font-medium text-gray-700">${r.peso} kg</span>` },
    { key: 'talla', label: 'Talla', sortable: true, render: (r) => `<span class="font-medium text-gray-700">${r.talla} cm</span>` },
    {
      key: 'estadoNutricional', label: 'Estado', sortable: true, filterable: true,
      render: (r) => {
        const map: Record<string, string> = {
          'Normal': 'bg-green-100 text-green-700 border-green-200',
          'BajoPeso': 'bg-yellow-100 text-yellow-700 border-yellow-200',
          'BajoPesoSevero': 'bg-red-100 text-red-700 border-red-200',
          'Sobrepeso': 'bg-orange-100 text-orange-700 border-orange-200',
          'Obesidad': 'bg-red-100 text-red-700 border-red-200'
        };
        const labels: Record<string, string> = {
          'Normal': 'Normal', 'BajoPeso': 'Bajo peso',
          'BajoPesoSevero': 'Bajo peso severo', 'Sobrepeso': 'Sobrepeso', 'Obesidad': 'Obesidad'
        };
        const defaultClass = 'bg-gray-100 text-gray-700 border-gray-200';
        return `<span class="px-3 py-1 rounded-full text-xs font-bold border ${map[r.estadoNutricional] ?? defaultClass}">${labels[r.estadoNutricional] ?? r.estadoNutricional}</span>`;
      }
    },
    // 💡 Separado en dos columnas según el DTO actual
    { key: 'percentilPeso', label: 'P. Peso', sortable: true, render: (r) => `<span class="text-gray-500 font-medium">P${r.percentilPeso}</span>` },
    { key: 'percentilTalla', label: 'P. Talla', sortable: true, render: (r) => `<span class="text-gray-500 font-medium">P${r.percentilTalla}</span>` }
  ];

  acciones: DatatableAction<MedidaResponse>[] = [
    { type: 'editar', onClick: (r) => this.abrirModal(r) },
    { type: 'eliminar', onClick: (r) => this.medidaAEliminar.set(r) }
  ];

  ngOnInit() {
    const idUrl = this.route.snapshot.paramMap.get('id');
    if (idUrl) {
      this.id = idUrl;
      const ninoId = parseInt(this.id, 10);
      this.ninosService.obtenerPorId(ninoId).subscribe(r => { if (r.success && r.data) this.nino.set(r.data); });
      this.cargarMedidas();
    }
  }

  cargarMedidas() {
    this.medidasService.obtenerPorNino(parseInt(this.id, 10)).subscribe(r => {
      if (r.success) this.medidas.set(r.data);
    });
  }

  abrirModal(medida?: MedidaResponse) {
    this.editando.set(medida ?? null);
    this.errorModal.set('');
    this.form = medida
      ? { fechaMedicion: medida.fechaMedicion, peso: medida.peso, talla: medida.talla }
      : { fechaMedicion: this.hoy, peso: 0, talla: 0 };
    this.modalAbierto.set(true);
  }

  cerrarModal() { this.modalAbierto.set(false); this.editando.set(null); }

  guardar() {
    if (!this.form.fechaMedicion || !this.form.peso || !this.form.talla) {
      this.errorModal.set('Completá todos los campos.'); return;
    }
    this.guardando.set(true);
    const medida = this.editando();
    const obs = medida
      ? this.medidasService.actualizar(medida.id, this.form)
      : this.medidasService.crear({ ...this.form, ninoId: parseInt(this.id, 10) });

    obs.subscribe({
      next: (r) => { if (r.success) { this.cargarMedidas(); this.cerrarModal(); } this.guardando.set(false); },
      error: (err) => { this.errorModal.set(err.error?.message ?? 'Error al guardar.'); this.guardando.set(false); }
    });
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