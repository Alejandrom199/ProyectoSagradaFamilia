import { Component, OnInit, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgIcon } from '@ng-icons/core';
import { NgApexchartsModule } from 'ng-apexcharts';

import { Predicciones as PrediccionesService } from '../../../../core/services/predicciones';
import { Datatable, DatatableColumn } from '../../../../shared/components/datatable/datatable';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { SpinnerModal } from '../../../../shared/components/spinner-modal/spinner-modal';
import { PredictionChart } from '../../../../shared/components/prediction-chart/prediction-chart';
import { Breadcrumb, BreadcrumbItem } from '../../../../shared/components/breadcrumb/breadcrumb';
import { NinosService } from '../../../../core/services/ninos';
import { AuthService } from '../../../../core/services/auth';
import { NinoResponse } from '../../../../shared/interfaces/nino.interface';
import { CurvasOmsResponse, PrediccionResponse, PuntoPrediccion } from '../../../../shared/interfaces/prediccion.interface';
import { formatearFecha } from '../../../../shared/utils/date.utils';

@Component({
  selector: 'app-listar-predicciones',
  imports: [
    CommonModule,
    FormsModule,
    NgIcon,
    NgApexchartsModule,
    Datatable,
    StatusBadge,
    SpinnerModal,
    PredictionChart,
    Breadcrumb
  ],
  templateUrl: './listar-predicciones.html',
  styleUrl: './listar-predicciones.css',
})
export class ListarPredicciones implements OnInit {
  private ninosService       = inject(NinosService);
  private prediccionesService = inject(PrediccionesService);
  private loadingBar         = inject(LoadingBar);
  readonly auth              = inject(AuthService);

  ninos             = signal<NinoResponse[]>([]);
  ninoSeleccionado  = signal<NinoResponse | null>(null);
  prediccion        = signal<PrediccionResponse | null>(null);
  curvasOms         = signal<CurvasOmsResponse | null>(null);
  cargando          = signal(false);
  estadoMotor       = signal<'comprobando' | 'online' | 'offline'>('comprobando');
  versionMotor      = signal<string>('');

  readonly formatearFecha = formatearFecha;

  // ── Cards de resumen (meses 3, 6, 12) ──────────────────────────────────────
  resumenPredicciones = computed<{ mes: number; punto: PuntoPrediccion }[]>(() => {
    const pts = this.prediccion()?.predicciones ?? [];
    return [3, 6, 12]
      .map(mes => ({ mes, punto: pts.find(p => p.meses === mes) ?? null }))
      .filter((x): x is { mes: number; punto: PuntoPrediccion } => x.punto !== null);
  });

  // ── Gráfica precisión histórica ─────────────────────────────────────────────
  chartPrecision = computed<Record<string, any> | null>(() => {
    const pts = (this.prediccion()?.predicciones ?? []).filter(p => p.pesoReal !== null);
    if (pts.length < 1) return null;
    const ts = (f: string) => new Date(f + 'T00:00:00').getTime();
    return {
      series: [{
        name: 'Real − predicho (kg)',
        data: pts.map(p => ({
          x: ts(p.fechaObjetivo),
          y: parseFloat(((p.pesoReal ?? 0) - p.pesoPredicho).toFixed(2))
        }))
      }],
      chart: { type: 'bar', height: 267, toolbar: { show: false }, fontFamily: 'inherit' },
      colors: ['#2563eb'],
      plotOptions: { bar: { columnWidth: '50%', borderRadius: 2 } },
      dataLabels: {
        enabled: true,
        formatter: (v: number) => (v > 0 ? '+' : '') + v + ' kg',
        style: { fontSize: '10px', colors: ['#1e40af'], fontFamily: 'inherit' },
        background: { enabled: false }
      },
      xaxis: {
        type: 'datetime',
        labels: { datetimeUTC: false, style: { colors: '#64748b', fontSize: '11px' } }
      },
      yaxis: {
        title: { text: 'Diferencia (kg)', style: { color: '#64748b', fontWeight: 600 } },
        labels: {
          style: { colors: ['#64748b'], fontSize: '11px' },
          formatter: (v: number) => (v > 0 ? '+' : '') + v
        }
      },
      annotations: {
        yaxis: [{ y: 0, borderColor: '#cbd5e1', borderWidth: 1 }]
      },
      tooltip: { x: { format: 'MMM yyyy' } },
      legend: { show: false }
    };
  });

  columnas: DatatableColumn<PuntoPrediccion>[] = [
    {
      key: 'meses', label: 'Mes', sortable: true,
      render: (r) => `Mes ${r.meses}`
    },
    {
      key: 'fechaObjetivo', label: 'Fecha objetivo', sortable: true,
      render: (r) => formatearFecha(r.fechaObjetivo)
    },
    {
      key: 'pesoPredicho', label: 'Peso predicho (kg)', sortable: true,
      render: (r) => `<span class="font-bold text-blue-600">${r.pesoPredicho.toFixed(2)} kg</span>`
    },
    {
      key: 'pesoMinimo', label: 'Rango de confianza',
      render: (r) => `<span class="text-xs text-gray-500">${r.pesoMinimo.toFixed(2)} — ${r.pesoMaximo.toFixed(2)} kg</span>`
    },
    {
      key: 'pesoReal', label: 'Peso real',
      render: (r) => r.pesoReal !== null
        ? `<span class="px-2.5 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800">${r.pesoReal.toFixed(2)} kg</span>`
        : '<span class="text-gray-400 font-medium">Aún sin medir</span>'
    }
  ];

  migajas: BreadcrumbItem[] = [{ label: 'Predicciones' }];

  ngOnInit() {
    this.cargarNinos();
    this.comprobarEstadoMotor();
  }

  comprobarEstadoMotor() {
    this.prediccionesService.obtenerEstadoServicio().subscribe({
      next: (r) => {
        if (r.success && r.data.status === 'ok') {
          this.estadoMotor.set('online');
          this.versionMotor.set(r.data.version);
        } else {
          this.estadoMotor.set('offline');
        }
      },
      error: () => this.estadoMotor.set('offline')
    });
  }

  cargarNinos() {
    this.loadingBar.show();
    const obs = this.auth.esMedico()
      ? this.ninosService.obtenerTodos()
      : this.ninosService.obtenerMisNinos();
    obs.subscribe({
      next: (r) => { if (r.success) this.ninos.set(r.data); this.loadingBar.complete(); },
      error: () => this.loadingBar.complete()
    });
  }

  seleccionarNino(ninoId: number) {
    const nino = this.ninos().find(n => n.id === ninoId);
    if (!nino) { this.prediccion.set(null); this.ninoSeleccionado.set(null); return; }

    this.ninoSeleccionado.set(nino);
    this.cargando.set(true);
    this.curvasOms.set(null);
    this.loadingBar.show();

    // Cargamos predicciones y curvas OMS en paralelo
    this.prediccionesService.obtenerPorNino(ninoId).subscribe({
      next: (r) => {
        if (r.success) this.prediccion.set(r.data);
        this.cargando.set(false);
        this.loadingBar.complete();
      },
      error: () => { this.cargando.set(false); this.loadingBar.complete(); }
    });

    this.prediccionesService.obtenerCurvasOms(ninoId).subscribe({
      next: (r) => { if (r.success) this.curvasOms.set(r.data); },
      error: () => {} // silencioso: el gráfico funciona sin las curvas OMS
    });
  }
}
