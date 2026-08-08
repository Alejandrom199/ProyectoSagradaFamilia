import { Component, Input, OnInit, inject, signal, computed, viewChild } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIcon } from '@ng-icons/core';

import { NgApexchartsModule, ChartComponent } from 'ng-apexcharts';
import type {
  ApexAxisChartSeries, ApexChart, ApexXAxis, ApexYAxis,
  ApexStroke, ApexDataLabels, ApexTooltip, ApexLegend,
  ApexMarkers, ApexFill, ApexAnnotations, ApexTheme
} from 'ng-apexcharts';

import { NinosService } from '../../../../core/services/ninos';
import { CitasService } from '../../../../core/services/citas';
import { PrescripcionesService } from '../../../../core/services/prescripciones';
import { MedidasService } from '../../../../core/services/medidas';
import { AuthService } from '../../../../core/services/auth';
import { ThemeService } from '../../../../core/services/theme';
import { Reportes } from '../../../../core/services/reportes';
import { NinoDetailResponse } from '../../../../shared/interfaces/nino.interface';
import { CitaResponse } from '../../../../shared/interfaces/cita.interface';
import { PrescripcionResponse } from '../../../../shared/interfaces/prescripcion.interface';
import { MedidaResponse } from '../../../../shared/interfaces/medida.interface';
import { BreadcrumbItem, Breadcrumb } from '../../../../shared/components/breadcrumb/breadcrumb';
import { LoadingBar } from '../../../../core/services/loading-bar';
import { formatearFecha, formatearEdad, renderFechaHora } from '../../../../shared/utils/date.utils';
import { DatatableColumn, Datatable } from '../../../../shared/components/datatable/datatable';

type ChartOpts = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  theme?: ApexTheme;
  xaxis: ApexXAxis;
  yaxis: ApexYAxis | ApexYAxis[];
  stroke: ApexStroke;
  dataLabels: ApexDataLabels;
  tooltip: ApexTooltip;
  legend: ApexLegend;
  markers: ApexMarkers;
  fill?: ApexFill;
  annotations?: ApexAnnotations;
  colors: string[];
};

@Component({
  selector: 'detalle-paciente',
  standalone: true,
  imports: [RouterLink, NgIcon, Breadcrumb, Datatable, NgApexchartsModule],
  templateUrl: './detalle-paciente.html',
})
export class DetallePaciente implements OnInit {
  @Input() id!: string;

  private ninosService = inject(NinosService);
  private citasService = inject(CitasService);
  private prescripcionesService = inject(PrescripcionesService);
  private medidasService = inject(MedidasService);
  private authService = inject(AuthService);
  private readonly themeService = inject(ThemeService);
  private reportesService = inject(Reportes);
  private loadingBar = inject(LoadingBar);

  private readonly chartPesoTallaRef = viewChild<ChartComponent>('chartPesoTallaRef');
  private readonly chartImcRef = viewChild<ChartComponent>('chartImcRef');

  nino = signal<NinoDetailResponse | null>(null);
  citas = signal<CitaResponse[]>([]);
  prescripciones = signal<PrescripcionResponse[]>([]);
  medidas = signal<MedidaResponse[]>([]);

  formatearFecha = formatearFecha;
  formatearEdad = formatearEdad;

  migajas: BreadcrumbItem[] = [
    { label: 'Pacientes', ruta: '/pacientes' },
    { label: 'Ficha del paciente' },
  ];

  // ──────────────────────────────────────────────
  // Gráfica 1: Curva Peso + Talla
  // ──────────────────────────────────────────────
  chartPesoTalla = computed<Partial<ChartOpts> | null>(() => {
    const m = this.medidasOrdenadas();
    if (m.length < 2) return null;
    const oscuro = this.themeService.oscuro();
    const colorEje = oscuro ? '#a1a1aa' : '#64748b';
    const ts = (f: string) => new Date(f + 'T00:00:00').getTime();
    return {
      series: [
        { name: 'Peso (kg)', data: m.map(x => ({ x: ts(x.fechaMedicion), y: x.peso })) },
        { name: 'Talla (cm)', data: m.map(x => ({ x: ts(x.fechaMedicion), y: x.talla })) },
      ],
      chart: { type: 'line', height: 280, toolbar: { show: false }, fontFamily: 'inherit', zoom: { enabled: false }, background: 'transparent', foreColor: colorEje },
      theme: { mode: oscuro ? 'dark' : 'light' },
      colors: ['#2563eb', '#7c3aed'],
      stroke: { curve: 'smooth', width: [2.5, 2.5] },
      dataLabels: { enabled: false },
      markers: { size: 4, strokeWidth: 0 },
      xaxis: {
        type: 'datetime',
        labels: { datetimeUTC: false, style: { colors: colorEje, fontSize: '11px' } }
      },
      yaxis: [
        {
          title: { text: 'Peso (kg)', style: { color: '#2563eb', fontWeight: 600 } },
          labels: { style: { colors: ['#2563eb'], fontSize: '11px' } }
        },
        {
          opposite: true,
          title: { text: 'Talla (cm)', style: { color: '#7c3aed', fontWeight: 600 } },
          labels: { style: { colors: ['#7c3aed'], fontSize: '11px' } }
        },
      ],
      tooltip: { theme: oscuro ? 'dark' : 'light', x: { format: 'dd MMM yyyy' } },
      legend: { show: true, position: 'top', fontSize: '12px', fontFamily: 'inherit', labels: { colors: colorEje } },
    };
  });

  // ──────────────────────────────────────────────
  // Gráfica 2: Índice de Masa Corporal
  // ──────────────────────────────────────────────
  chartIMC = computed<Partial<ChartOpts> | null>(() => {
    const m = this.medidasOrdenadas();
    if (m.length < 2) return null;
    const oscuro = this.themeService.oscuro();
    const colorEje = oscuro ? '#a1a1aa' : '#64748b';
    const colorAnotacion18 = oscuro ? '#fbbf24' : '#92400e';
    const colorAnotacion25 = oscuro ? '#fb923c' : '#7c2d12';
    const ts = (f: string) => new Date(f + 'T00:00:00').getTime();
    const imc = (x: MedidaResponse) =>
      parseFloat((x.peso / Math.pow(x.talla / 100, 2)).toFixed(1));
    return {
      series: [{ name: 'IMC', data: m.map(x => ({ x: ts(x.fechaMedicion), y: imc(x) })) }],
      chart: { type: 'area', height: 280, toolbar: { show: false }, fontFamily: 'inherit', zoom: { enabled: false }, background: 'transparent', foreColor: colorEje },
      theme: { mode: oscuro ? 'dark' : 'light' },
      colors: ['#0891b2'],
      fill: { opacity: 0.12 },
      stroke: { curve: 'smooth', width: 2.5 },
      dataLabels: {
        enabled: true,
        style: { fontSize: '10px', colors: ['#0891b2'], fontFamily: 'inherit' },
        background: { enabled: false }
      },
      markers: { size: 0 },
      xaxis: {
        type: 'datetime',
        labels: { datetimeUTC: false, style: { colors: colorEje, fontSize: '11px' } }
      },
      yaxis: {
        title: { text: 'IMC (kg/m²)', style: { color: colorEje, fontWeight: 600 } },
        labels: { style: { colors: [colorEje], fontSize: '11px' } },
        min: 10
      },
      annotations: {
        yaxis: [
          {
            y: 18.5,
            borderColor: '#f59e0b', borderWidth: 1,
            label: { text: '18.5', position: 'left', style: { color: colorAnotacion18, fontSize: '10px', background: 'transparent', padding: { left: 0, right: 4, top: 1, bottom: 1 } } }
          },
          {
            y: 25,
            borderColor: '#f97316', borderWidth: 1,
            label: { text: '25', position: 'left', style: { color: colorAnotacion25, fontSize: '10px', background: 'transparent', padding: { left: 0, right: 4, top: 1, bottom: 1 } } }
          },
        ]
      },
      tooltip: { theme: oscuro ? 'dark' : 'light', x: { format: 'dd MMM yyyy' } },
      legend: { show: false },
    };
  });

  private medidasOrdenadas = computed(() =>
    [...this.medidas()].sort((a, b) =>
      new Date(a.fechaMedicion).getTime() - new Date(b.fechaMedicion).getTime()
    )
  );

  columnasCitas: DatatableColumn<CitaResponse>[] = [
    {
      key: 'fechaHora', label: 'Fecha', sortable: true,
      render: (row) => renderFechaHora(row.fechaHora)
    },
    {
      key: 'motivo', label: 'Motivo',
      render: (row) => row.motivo || '<span class="text-muted">-</span>'
    },
    {
      key: 'estado', label: 'Estado',
      render: (row) => {
        const mapa: Record<string, string> = {
          'Pendiente': 'bg-warning-soft text-warning',
          'EnCurso': 'bg-info-soft text-info',
          'Completada': 'bg-success-soft text-success',
          'Cancelada': 'bg-danger-soft text-danger',
          'NoAsistio': 'bg-[var(--color-surface-alt)] text-muted',
          'Reagendada': 'bg-violet-100 dark:bg-violet-500/15 text-violet-700 dark:text-violet-400',
        };
        const clase = mapa[row.estado] || 'bg-[var(--color-surface-alt)] text-muted';
        const label = row.estado === 'NoAsistio' ? 'No asistió'
          : row.estado === 'EnCurso' ? 'En curso' : row.estado;
        return `<span class="px-2 py-1 rounded-full text-xs font-bold ${clase}">${label}</span>`;
      }
    }
  ];

  columnasPrescripciones: DatatableColumn<PrescripcionResponse>[] = [
    {
      key: 'fechaCreacion', label: 'Fecha', sortable: true,
      render: (row) => renderFechaHora(row.fechaCreacion)
    },
    {
      key: 'medicamentos', label: 'Medicamentos',
      render: (row) => {
        const nombres = row.medicamentos.map(m => m.nombre).join(', ');
        return `<p class="text-sm text-[var(--color-text-primary)] max-w-xs truncate" title="${nombres}">${row.medicamentos.length} medicamento(s): ${nombres}</p>`;
      }
    },
    {
      key: 'nombreMedico', label: 'Médico',
      render: (row) => `<span class="text-sm text-[var(--color-text-secondary)]">Dr(a). ${row.nombreMedico}</span>`
    }
  ];

  ngOnInit(): void {
    this.cargarDatos();
  }

  cargarDatos(): void {
    const id = parseInt(this.id);
    this.loadingBar.show();

    this.ninosService.obtenerPorId(id).subscribe({
      next: (r) => {
        if (r.success) {
          this.nino.set(r.data);
          this.migajas = [
            { label: 'Pacientes', ruta: '/pacientes' },
            { label: `${r.data.nombre} ${r.data.apellido}` },
          ];
        }
      }
    });

    this.citasService.obtenerPorNino(id).subscribe({
      next: (r) => { if (r.success) this.citas.set(r.data.slice(0, 5)); }
    });

    this.prescripcionesService.obtenerHistorialPorNino(id).subscribe({
      next: (r) => { if (r.success) this.prescripciones.set(r.data.slice(0, 5)); }
    });

    this.medidasService.obtenerPorNino(id).subscribe({
      next: (r) => {
        if (r.success) this.medidas.set(r.data);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  async descargarHistoriaClinica(): Promise<void> {
    this.loadingBar.show();
    const n = this.nino();
    const u = this.authService.currentUser();
    const titulo = n ? `Historia Clínica - ${n.nombre} ${n.apellido}` : 'Historia Clínica';

    const [graficaCrecimientoBase64, graficaImcBase64] = await Promise.all([
      this.capturarGrafica(this.chartPesoTallaRef()),
      this.capturarGrafica(this.chartImcRef()),
    ]);

    this.reportesService.descargarHistoriaClinicaPdf({
      ninoId: parseInt(this.id),
      titulo,
      usuario: u ? `${u.nombre} ${u.apellido}` : '',
      graficaCrecimientoBase64,
      graficaImcBase64,
    }).subscribe({
      next: (blob) => {
        this.descargarBlob(blob, `historia-clinica-${hoy()}.pdf`);
        this.loadingBar.complete();
      },
      error: () => this.loadingBar.complete()
    });
  }

  private async capturarGrafica(chart: ChartComponent | undefined): Promise<string | null> {
    if (!chart) return null;
    try {
      const resultado = await chart.dataURI() as { imgURI?: string };
      return resultado.imgURI ?? null;
    } catch {
      return null;
    }
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
